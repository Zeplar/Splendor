using System;
using System.Diagnostics;
using Splendor.Genetic;
using System.Collections.Generic;

namespace Splendor
{
    public class Gene_Grey : Player
    {
        int size = 50;

        public override void takeTurn()
        {
            SplendorFit fit = new SplendorFit();
            Population cur = new Population(size, new SplendorGene(50), fit, new AForge.Genetic.EliteSelection());
            Population next = new Population(size, new SplendorGene(50), fit, new AForge.Genetic.EliteSelection());
            for (int i=0; i < 100; i++)
            {
                cur.Crossover();
                cur.Mutate();
                next.Crossover();
                next.Mutate();

                //!!! Crossover and Mutate add chromosomes to the population. Therefore Selection has to come before runTournament to prune the population.
                //!!! But runTournament is required to the values for Selection.
                cur.Selection();
                next.Selection();
                fit.runTournament(cur.population, next.population);
                Console.WriteLine("" + i);
            }
            Debug.Assert(false);


        }
    }

}
