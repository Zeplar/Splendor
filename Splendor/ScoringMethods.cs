﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Splendor
{

    /// <summary>
    /// Scores the board from the perspective of the generating player	
    /// </summary>
    public static class ScoringMethods
    {

        public static Dictionary<string,Func<Board,int>> dictionary = new Dictionary<string, Func<Board,int>>();

        private static bool register { get
            {
                dictionary.Add("deltapoints", DeltaPoints);
                dictionary.Add("points", Points);
                dictionary.Add("winloss", WinLoss);
                dictionary.Add("prestige", Prestige);
                dictionary.Add("legalmoves", LegalMoves);
                return true;
            }
        }



        /// <summary>
        /// Scores difference in points.
        /// </summary>
        public static int DeltaPoints(Board b)
        {
            return b.maximizingPlayer.points - b.minimizingPlayer.points;
        }

        /// <summary>
        /// Scores maxPoints for a win, minPoints for a loss, else zero. Tiebreaks on prestige.
        /// </summary>
        public static int WinLoss(Board b)
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
        public static int Points(Board b)
        {
            return b.maximizingPlayer.points;
        }

        /// <summary>
        /// Scores only prestige.
        /// </summary>
        public static int Prestige(Board b)
        {
            return b.maximizingPlayer.field.Count;
        }

        /// <summary>
        /// Scores number of legal Buys and Reserves available.
        /// Takes may be bad since trades vastly outnumber pure takes.
        /// </summary>
        public static int LegalMoves(Board b)
        {
            return b.legalMoves.Count(x => x.moveType == 2 || x.moveType == 3);
        }
        

        /// <summary>
        /// Converts a "self-only" function into a delta function
        /// </summary>
        /// <returns></returns>
        public static Func<Board,int> deltafy(Func<Board, int> scoringFunction)
        {
            return (b) =>
            {
                int self = scoringFunction(b);
                b.players.Reverse();
                int other = scoringFunction(b);
                b.players.Reverse();
                return self - other;
            };
        }

        public static Func<Board,int> combine(Func<Board,int> f1, Func<Board,int> f2)
        {
            return (b) =>
            {
                return f1(b) + f2(b);
            };
        }

        private static bool reg1 = register;

    }
}
