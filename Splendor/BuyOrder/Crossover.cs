using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
namespace Splendor.BuyOrder
{
    public class Crossover : CrossoverBase
    {

        public Crossover(int parentsNumber, int childrenNumber) : base(parentsNumber, childrenNumber) { }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            throw new NotImplementedException();
        }
    }
}
