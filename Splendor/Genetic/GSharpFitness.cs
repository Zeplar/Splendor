using System;
using System.Diagnostics;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Splendor.Genetic
{
    class GSharpFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            Console.CursorLeft = 0;
            Console.Write("Getting a fitness...");
            GSharpChromosome c = (GSharpChromosome)chromosome;
            GSharpChromosome.gene gene = c.GetGene(c.legalLength - 1).Value as GSharpChromosome.gene;
            Console.CursorLeft = 0; Console.Write("We got this far....");
            return score(gene.beforeState);
        }

        private double score(Board b)
        {
            Debug.Assert(b != null);
            if (b.gameOver)
            {
                if (b.maximizingPlayer.points > b.minimizingPlayer.points) return 100;
                else return 1;
            }
            return 1 + b.maximizingPlayer.points;
        }
    }
}
