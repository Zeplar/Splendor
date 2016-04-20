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
        public Minimax(int i, Heuristic f) : base("Minimax " + i + "(" + f.ToString() + ")")
        {
            treeDepth = i;
            scoringFunction = f;
        }

        private Heuristic scoringFunction;

        public static Move minimax(Board startingPoint, int depth, Heuristic scoringFunction, out double score, Func<Board, bool> endCondition=null)
        {
            bool myTurn = startingPoint.Turn % 2 == 0;  //Indicates that it's Minimax's turn.
            Move bestMove = null;
            List<Move> legalMoves = startingPoint.legalMoves;
            double[] bestScore = new double[legalMoves.Count];
            //int val;
            Func<double, double, bool> comp = (x,y) => (x > y);

            if (depth == 0 || legalMoves.Count == 0 || (endCondition != null && endCondition(startingPoint)))
            {
                //If the root is currently myTurn, then the opponent generated this board; therefore send the negation of the score. Otherwise send the score.
                score = scoringFunction.evaluate(startingPoint) * (myTurn ? -1 : 1);
                return null;
            }

            if (!myTurn)
            {
                //If it's the opponent's turn, they will select for the minimal score.
                for (int i=0; i < bestScore.Length; i++) bestScore[i] = double.MaxValue;
                comp = (x, y) => (x < y);
            }
            else for (int i = 0; i < bestScore.Length; i++) bestScore[i] = double.MinValue;


            Parallel.For(0, legalMoves.Count, (x) =>
           {
               Move m = legalMoves[x];
               minimax(startingPoint.generate(m), depth - 1, scoringFunction, out bestScore[x]);
           });
            //At this point bestscore[x] is the cumulative score of lower branches guaranteed by choosing the xth move.
            bestMove = legalMoves[0];
            //Minimax finds the maximum of these scores, or the opponent finds the minimum of these scores.
            for (int i=1; i < bestScore.Length; i++)
            {
                if (comp(bestScore[i], bestScore[0]))
                {
                    bestScore[0] = bestScore[i];
                    bestMove = legalMoves[i];
                }
            }
            //If it's Minimax's turn, the opponent generated this board; therefore the score is subtracted. Otherwise the score is added.
            score = myTurn ? bestScore[0] - scoringFunction.evaluate(startingPoint) : bestScore[0] + scoringFunction.evaluate(startingPoint);
            if (startingPoint.Equals(Board.current))
            {
                Array scores = bestScore.Set();
                var best = new List<double>(bestScore).FindAll(x => x == bestScore[0]);
                RecordHistory.current.writeToFile("minimax.txt", "Diversity: " + scores.Length + "   " + "BestScores:  " + best.Count + Environment.NewLine);
            }
            return bestMove;
        }


        public override void takeTurn()
        {
            turnTimer.Restart();
            Board b = Board.current;
            double x;
            Move k = minimax(b, treeDepth, scoringFunction, out x);
            turnTimer.Stop();
            RecordHistory.current.record(this + " made move " + k.ToString());
            takeAction(k);
        }

    }

}
