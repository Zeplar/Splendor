using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;

namespace Splendor.Genetic
{
    public class SplendorFit : IFitnessFunction
    {
        /// <summary>
        /// Scores chromosomes by pitting them against all enemy chromosomes and averaging the delta point value.
        /// </summary>
        public void runTournament(List<IChromosome> cur, List<IChromosome> next)
        {
            Debug.Assert(cur.Count == next.Count, cur.Count + " " + next.Count);

            foreach (SplendorGene g in cur)
            {
                g.updateMoves();
            }
            foreach (SplendorGene g in next)
            {
                g.updateMoves();
            }

            int count = cur.Count;
            
            for (int i=0; i < count; i++)
            {
                for (int j=0; j < count; j++)
                {
                    SplendorGene winner;
                    bool curWon = score(cur[i], next[i]) > 0;//!!!
                    if (curWon)
                    {
                        winner = (SplendorGene)cur[i];
                    } else
                    //!!! This gives ties to the opponent, but I doubt that throws it off too much (if scoring accounts for tiebreakers).
                    {
                        winner = (SplendorGene)next[i];
                    }
                    winner.score += 1;
                }
            }
            Debug.Assert(cur.Count == next.Count, "Something changed in the tournament");

        }

        public double Evaluate(IChromosome chromosome)
        {
            SplendorGene g = (SplendorGene)chromosome;
            return g.score;
        }

        private int score(IChromosome max, IChromosome min)
        {
            SplendorGene m = (SplendorGene)max;
            SplendorGene n = (SplendorGene)min;
            return score(m, n);
        }

        private int score(SplendorGene max, SplendorGene min)
        {
            Board b = Board.current;
            int i = 0;
            int j = 0;
            while (i < max.length && j < max.length)
            {
                while (i < max.length)
                {
                    if (max.moves[i].isLegal(b))
                    {
                        b = b.generate(max.moves[i]);
                        i++;
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                while (j < max.length)
                {
                    if (min.moves[j].isLegal(b))
                    {
                        b = b.generate(min.moves[j]);
                        j++;
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
                if (b.gameOver)
                {
                    return 1000 * (b.maximizingPlayer.points - b.minimizingPlayer.points);
                }
            }
            return (b.maximizingPlayer.points - b.minimizingPlayer.points);
        }
    }
}
