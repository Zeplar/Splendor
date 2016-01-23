using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Greedy : Player
    {
        private ScoringMethods.Function scoringFunction;


        public Greedy()
        {
            scoringFunction = ScoringMethods.DeltaPoints + ScoringMethods.WinLoss;
            name = "Greedy Delta";
        }
        public Greedy(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
            this.name = "Greedy Custom";
        }

        public override string ToString()
        {
            return this.name;
        }


        public static Move getGreedyMove(Board b, ScoringMethods.Function scoringFunction)
        {
            Move bestMove = null;
            double bestScore = int.MinValue;

            foreach (Move m in b.legalMoves)
            {
                double temp = scoringFunction.evaluate(b.generate(m));
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
