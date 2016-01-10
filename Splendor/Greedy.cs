using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Greedy : Player
    {
        private Func<Board, double> scoringFunction;


        public Greedy()
        {
            scoringFunction = ScoringMethods.combine(ScoringMethods.DeltaPoints, ScoringMethods.WinLoss);
            name = "Greedy Delta";
        }
        public Greedy(Func<Board, double> scoringFunction)
        {
            this.scoringFunction = scoringFunction;
            this.name = "Greedy Custom";
        }

        public override string ToString()
        {
            return this.name;
        }


        public static Move getGreedyMove(Board b, Func<Board, double> scoringFunction)
        {
            Move bestMove = null;
            double bestScore = int.MinValue;

            foreach (Move m in b.legalMoves)
            {
                double temp = scoringFunction(b.generate(m));
                if (temp > bestScore)
                {
                    bestScore = temp;
                    bestMove = m;
                }
            }
            return bestMove;
        }

        public override void takeTurn()
        {

            Move m = getGreedyMove(Board.current, scoringFunction);
            if (m != null)
            {
                m.takeAction();
                RecordHistory.record(this + " took move " + m);
                return;
            }
            Debug.Assert(m != null, "Greedy couldn't even find a random move.");
        }
    }
}
