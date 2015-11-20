using System;
using System.Diagnostics;
using Splendor.Genetic;
using System.Collections.Generic;

namespace Splendor
{
    public class Gene_Grey : Player
    {
        int populationSize = 100;
        int generations = 10;
        int depth = 10;

        public override void takeTurn()
        {
            Console.WriteLine();
            SplendorFit fit = new SplendorFit();
            SplendorPop cur = new SplendorPop(populationSize, new SplendorGene(depth), fit, new AForge.Genetic.EliteSelection());
            SplendorPop next = new SplendorPop(populationSize, new SplendorGene(depth), fit, new AForge.Genetic.EliteSelection());
            for (int i=0; i < generations; i++)
            {
                cur.Crossover();
                cur.Mutate();
                next.Crossover();
                next.Mutate();

                //!!! Crossover and Mutate add chromosomes to the population. Therefore Selection has to come before runTournament to prune the population.
                //!!! But runTournament is required to obtain the values for Selection.
                cur.Selection();
                next.Selection();
                fit.runTournament(cur.population, next.population);
                Console.CursorLeft = 0;
                Console.Write("" + i);
            }
            cur.FindBestChromosome();
            Debug.Assert(cur.FitnessMax != 0, "" + cur.FitnessMax); // Pause if we get a good result
            SplendorGene g = (SplendorGene)cur.BestChromosome;
            for (int i=0; i < g.length; i++)
            {
                Move m = fit.getMoveByIndex(g.moveTypes[i], g.moveValues[i], Board.current);
                if (m != null)
                {
                    m.takeAction();
                    return;
                }
            }
            Debug.Assert(false, "Failed to find a legal move.");
        }
    }

}
