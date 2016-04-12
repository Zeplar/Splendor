using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Greedy : Player
    {
        private Heuristic scoringFunction;


        public Greedy()
        {
            scoringFunction = Heuristic.Lead + Heuristic.WinLoss;
            name = "Greedy Delta";
        }
        public Greedy(Heuristic scoringFunction)
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
            Heuristic f;
            try
            {
                f = Heuristic.parse(args);
            } catch (FormatException)
            {
                throw new FormatException("Usage: greedy <...scoringFunction...>");
            }
            return new Greedy(f);
        }


        /// <summary>
        /// Gets the greedy move based on the given function. Ignores Reserves to save computation.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="scoringFunction"></param>
        /// <returns></returns>
        public static Move getGreedyMove(Board b, Heuristic scoringFunction)
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
        takeAction(m);
        RecordHistory.current.record(this + " took move " + m);
        RecordHistory.current.record(this + " current state: " + points + " points and " + Field.Count + " cards and " + Gems.magnitude + " gems.");
        return;
        }
    }
}
