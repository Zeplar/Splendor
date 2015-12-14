using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
namespace Splendor.Genetic
{
    class GSharpCrossover : CrossoverBase
    {
        public GSharpCrossover() : base(2, 2) { }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            return parents;
        }
    }
}
