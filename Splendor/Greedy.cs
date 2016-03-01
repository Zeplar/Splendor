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
            name = "Greedy Delta";
        }
        public Greedy(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
            name = "Greedy " + scoringFunction.ToString();
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


        /// <summary>
        /// Gets the greedy move based on the given function. Ignores Reserves to save computation.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="scoringFunction"></param>
        /// <returns></returns>
        public static Move getGreedyMove(Board b, ScoringMethods.Function scoringFunction)
        {
            Move bestMove = null;
            double bestScore = int.MinValue;

            foreach (Move m in b.legalMoves)
            {
               //if (m.moveType == Move.Type.RESERVE) continue;
                double temp = scoringFunction.evaluate(b.generate(m));
                if (temp > bestScore)
                {
                    bestScore = temp;
                    bestMove = m;
                }
            }
            if (bestMove == null) throw new Exception("GreedyMove returned a null move.");
            return bestMove;
        }

        public override void takeTurn()
        {

            Move m = getGreedyMove(Board.current, scoringFunction);
            if (m != null)
            {
                takeAction(m);
                RecordHistory.record(this + " took move " + m);
                return;
            }
            else throw new Exception("Greedy couldn't even find a random move.");
        }
    }
}
