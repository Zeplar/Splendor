using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Splendor
{

    /// <summary>
    /// Scores the board from the perspective of the generating player	
    /// </summary>
    public static class ScoringMethods
    {

        public static Dictionary<string,Func<Board,double>> dictionary = new Dictionary<string, Func<Board, double>>();

        public static void register()
        { 
            dictionary.Add("deltapoints", DeltaPoints);
            dictionary.Add("points", Points);
            dictionary.Add("winloss", WinLoss);
            dictionary.Add("prestige", Prestige);
            dictionary.Add("legalmoves", LegalMoves);
            dictionary.Add("all", All);
            dictionary.Add("turn", turn);
            dictionary.Add("turnsquared", turnsquared);
        }

        public static double turn(Board b)
        {
            return b.turn;
        }
        public static double turnsquared(Board b)
        {
            return b.turn * b.turn;
        }

        public static string listAll
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (string k in dictionary.Keys) s.Append(k + ", ");
                return s.ToString();
            }
        }



        /// <summary>
        /// Scores difference in points.
        /// </summary>
        public static double DeltaPoints(Board b)
        {
            return b.maximizingPlayer.points - b.minimizingPlayer.points;
        }

        /// <summary>
        /// Scores maxPoints for a win, minPoints for a loss, else zero. Tiebreaks on prestige.
        /// </summary>
        public static double WinLoss(Board b)
        {
            if (b.gameOver)
            {
                if (b.maximizingPlayer.points < 15 && b.minimizingPlayer.points < 15) return 0;
                if (b.maximizingPlayer.points < b.minimizingPlayer.points)
                {
                    return int.MinValue / 2;
                }
                if (b.maximizingPlayer.points > b.minimizingPlayer.points)
                {
                    return int.MaxValue / 2;
                }
                return (b.maximizingPlayer.field.Count > b.minimizingPlayer.field.Count) ? int.MaxValue / 2 : int.MinValue / 2;

            }
            return 0;
        }

        /// <summary>
        /// Scores only points.
        /// </summary>
        public static double Points(Board b)
        {
            return b.maximizingPlayer.points;
        }

        /// <summary>
        /// Scores only prestige.
        /// </summary>
        public static double Prestige(Board b)
        {
            return b.maximizingPlayer.field.Count;
        }

        /// <summary>
        /// Scores number of legal Buys and Reserves available.
        /// Takes may be bad since trades vastly outnumber pure takes.
        /// </summary>
        public static double LegalMoves(Board b)
        {
            return b.legalMoves.Count(x => x.moveType == Move.Type.BUY || x.moveType == Move.Type.RESERVE);
        }

        public static double All(Board b)
        {
            return 3 * (b.maximizingPlayer.points - b.minimizingPlayer.points) + 2 * (b.maximizingPlayer.field.Count - b.minimizingPlayer.field.Count) + (b.maximizingPlayer.gems.magnitude);
        }
        

        /// <summary>
        /// Converts a "self-only" function into a delta function
        /// </summary>
        /// <returns></returns>
        public static Func<Board, double> deltafy(Func<Board, double> scoringFunction)
        {
            return (b) =>
            {
                double self = scoringFunction(b);
                b.players.Reverse();
                double other = scoringFunction(b);
                b.players.Reverse();
                return self - other;
            };
        }

        public static Func<Board, double> combine(Func<Board, double> f1, Func<Board, double> f2)
        {
            return (b) =>
            {
                return f1(b) + f2(b);
            };
        }

        private static Func<Board, double> multiply(double i, Func<Board, double> f)
        {
            return (Board x) => i * f(x);
        }

        public static Func<Board, double> parseScoringFunction(string[] args, int skip)
        {
            Func<Board, double> scoringFunction = WinLoss;
            Func<Board, double> nextFunc;
            for (int j = skip; j < args.Length; j++)
            {
                int i;
                if (args[j].Equals("delta"))
                {
                    j++;
                    nextFunc = ScoringMethods.deltafy(dictionary[args[j]]);
                }
                else if (int.TryParse(args[j], out i))
                {
                    j++;
                    nextFunc = multiply(i, dictionary[args[j]]);
                }
                else
                {
                    nextFunc = dictionary[args[j]];
                }
                scoringFunction = combine(scoringFunction, nextFunc);
            }
            return scoringFunction;
        }

    }
}
