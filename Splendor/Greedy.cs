using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Greedy : Player
    {
        private Func<Board, int> scoringFunction;


        public Greedy()
        {
            scoringFunction = ScoringMethods.combine(ScoringMethods.DeltaPoints, ScoringMethods.WinLoss);
            name = "Greedy Delta";
        }
        public Greedy(Func<Board,int> scoringFunction)
        {
            this.scoringFunction = scoringFunction;
            this.name = "Greedy Custom";
        }

        public override string ToString()
        {
            return this.name;
        }


        public static Move getGreedyMove(Board b, Func<Board,int> scoringFunction)
        {
            Move bestMove = null;
            int bestScore = int.MinValue;

            foreach (Move m in b.legalMoves)
            {
                int temp = scoringFunction(b.generate(m));
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
            showState();
            Debug.Assert(m != null, "Greedy couldn't even find a random move.");
        }
    }
}
