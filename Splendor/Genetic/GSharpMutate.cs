using System;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;

namespace Splendor.Genetic
{
    class GSharpMutate : MutationBase
    {

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            //GSharpChromosome c = (GSharpChromosome)chromosome;
            //if (Splendor.random.NextDouble() < probability)
            //{
            //    int pointOfMutation = Splendor.random.Next(c.legalLength);
            //    c.legalLength = pointOfMutation;
            //    for (; pointOfMutation < c.Length; pointOfMutation++)
            //    {
            //        c.ReplaceGene(pointOfMutation, c.GenerateGene(pointOfMutation));
            //    }
            //}
        }
    }
}
