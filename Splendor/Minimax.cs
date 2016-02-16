using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Splendor
{

    public class Minimax : Player
    {
        public int treeDepth;
        public Minimax(int i, ScoringMethods.Function f) : base("Minimax " + i + "(" + f.ToString() + ")")
        {
            treeDepth = i;
            scoringFunction = f;
        }

        private ScoringMethods.Function scoringFunction;

        public static Move minimax(Board startingPoint, int depth, ScoringMethods.Function scoringFunction, out double score)
        {
            Move bestMove = null;
            List<Move> legalMoves = startingPoint.legalMoves;
            double[] bestScore = new double[legalMoves.Count];
            //int val;
            Func<double, double, bool> comp = (x,y) => (x > y);

            if (depth == 0 || legalMoves.Count == 0 || startingPoint.gameOver)
            {
                score = scoringFunction.evaluate(startingPoint);
                return null;
            }

            if (startingPoint.Turn % 2 == 1)
            {
                for(int i=0; i < bestScore.Length; i++) bestScore[i] = int.MaxValue;
                comp = (x, y) => (x < y);
            }
            else for (int i = 0; i < bestScore.Length; i++) bestScore[i] = int.MinValue;


            Parallel.For(0, legalMoves.Count, (x) =>
           {
               Move m = legalMoves[x];
               minimax(startingPoint.generate(m), depth - 1, scoringFunction, out bestScore[x]);
           });
            bestMove = legalMoves[0];
            for (int i=0; i < bestScore.Length; i++)
            {
                if (comp(bestScore[i], bestScore[0]))
                {
                    bestScore[0] = bestScore[i];
                    bestMove = legalMoves[i];
                }
            }
            score = bestScore[0] + scoringFunction.evaluate(startingPoint);
            return bestMove;
        }


        public override void takeTurn()
        {
            Board b = Board.current;
            double x;
            Move k = minimax(b, treeDepth, scoringFunction, out x);
            RecordHistory.record(this + " made move " + k?.ToString());
            takeAction(k);
        }

    }

}
