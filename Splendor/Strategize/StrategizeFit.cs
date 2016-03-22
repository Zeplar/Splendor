using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;
using System;

namespace Splendor.Strategize
{
    public class StrategizeFit : IFitnessFunction
    {
        private ScoringMethods.Function scoringFunction;
        private ScoringMethods.Function greedy = ScoringMethods.Points;
        public int timesEvaluated;

        public StrategizeFit(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        public double Evaluate(IChromosome chromosome)
        {
            timesEvaluated++;
            return Math.Max(1, score((StrategizeChromosome)chromosome));

        }

        /// <summary>
        /// Generates the next boardstate
        /// </summary>
        private Board generate(StrategizeChromosome max, Board b)
        {
            Debug.Assert(b.Turn % 2 == 0);
            Move next = Greedy.getGreedyMove(b, max.getFn);
            return b.generate(next);
        }

        private bool predictWin(Board current, Move pred)
        {
            if (current.gameOver)
            {
                if (current.winner == 0)
                {
                    RecordHistory.current.record("!!! " + GameController.currentPlayer + " now thinks it's going to win! Pred. Greedy move " + pred);
                }
                else if (current.winner == 1)
                {
                    RecordHistory.current.record("!!! " + GameController.currentPlayer + " now thinks it's going to lose!");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates a chromosome against Greedy
        /// </summary>
        private double score(StrategizeChromosome max)
        {
            Board current = Board.current;
            Board next;
            Move nextMove;
            Move pred = null; //Predicted move next turn for Greedy
            double score = 0;
            int i = 0;
            while (current.Turn < max.len)
            {
                if (predictWin(current, pred)) break;
                next = generate(max, current);
                if (next == null) break;
                score += scoringFunction.evaluate(next);
                i++;
                current = next;
                if (predictWin(current, pred)) break;
                //Do the "Generate-next-move" loop for greedy
                nextMove = Greedy.getGreedyMove(current, greedy);
                if (i == 1) pred = nextMove;
                if (nextMove != null)
                {
                    current = current.generate(nextMove);
                }
                else
                {
                    return score / i;
                }
            }
            return score / i;
        }

    }
}
