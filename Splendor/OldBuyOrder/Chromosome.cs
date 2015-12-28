using AForge.Genetic;
using System;

namespace Splendor.OldBuyOrder
{
    public class Chromosome : PermutationChromosome
    {
        private static int maxLength
        {
            get { return Board.current.viewableCards.Count; }
        }

        public Chromosome() : base(maxLength)
        {
        }

        public override IChromosome Clone()
        {
            return new Chromosome();
        }

        public override IChromosome CreateNew()
        {
            return new Chromosome();
        }

    }
}
