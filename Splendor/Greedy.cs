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
            scoringFunction = ScoringMethods.Lead + ScoringMethods.WinLoss;
            fn = scoringFunction;
            name = "Greedy Delta";
        }
        public Greedy(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
            name = "Greedy Custom";
            fn = scoringFunction;
        }

        public override string ToString()
        {
            return "Greedy " + scoringFunction;
        }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static Greedy Create(string[] args)
        {
            ScoringMethods.Function f;
            try
            {
                f = ScoringMethods.parse(args);
            } catch (FormatException z)
            {
                throw z;
            }
            return new Greedy(f);
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
            if (m == null) throw new Exception("Greedy couldn't even find a random move.");
        }
    }
}
