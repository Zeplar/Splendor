using System;
using CH.Combinatorics;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace Splendor.BuyOrder
{
    public class Chromosome : ChromosomeBase
    {
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene();
        }

        public override IChromosome CreateNew()
        {
            return new Chromosome(this.Length);
        }

        public Chromosome(int length) : base(length)
        {
            List<Gene> genes = new List<Gene>();
            foreach (Card c in Board.current.viewableCards)
            {
                genes.Add(new Gene(c));
            }
            genes.shuffle();
            var agenes = genes.ToArray();
            ReplaceGenes(0, agenes);
        }

        
    }
}
