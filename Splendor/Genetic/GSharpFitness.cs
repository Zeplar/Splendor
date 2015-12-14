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
            GSharpChromosome c = (GSharpChromosome)chromosome;
            GSharpChromosome.gene gene = c.GetGene(c.legalLength - 1).Value as GSharpChromosome.gene;
            return score(gene.beforeState);
        }

        private double score(Board b)
        {
            Debug.Assert(b != null);
            if (b.gameOver)
            {
                if (b.minimizingPlayer.points < b.maximizingPlayer.points) return 100;
                else return 1/100;
            }
            return Math.Max(1, 3*b.maximizingPlayer.points + b.maximizingPlayer.field.Count);
        }
    }
}
