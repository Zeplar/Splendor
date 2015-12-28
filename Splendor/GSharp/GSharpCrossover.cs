using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using System.Diagnostics;
namespace Splendor.GSharp
{
    class GSharpCrossover : CrossoverBase
    {
        public GSharpCrossover() : base(2, 2) { }

        private int hash(Board b)
        {
            int i = 1;
            foreach (Move.BUY buy in Move.BUY.getLegalMoves(b))
            {
                i *= buy.card.id;
            }
            return i;
        }

        private int[] getIndices(GSharpChromosome parent1, GSharpChromosome parent2, Board[] targets)
        {
            int x = 0;
            int y = 0;
            for (; x <= parent1.legalLength; x++)
            {
                if (((GSharpChromosome.gene)parent1.GetGene(x).Value).beforeState == targets[0]) break;
            }
            for (; y <= parent2.legalLength; y++)
            {
                if (((GSharpChromosome.gene)parent2.GetGene(y).Value).beforeState == targets[1]) break;
            }
            Debug.Assert(x != parent1.legalLength); Debug.Assert(y != parent2.legalLength);
            return new int[2] { x, y };

        }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            Board[] targets = matchBuys((GSharpChromosome)parents[0], (GSharpChromosome)parents[1]);
            if (targets == null)
            {
                return parents;
            }
            int[] indices = getIndices((GSharpChromosome)parents[0], (GSharpChromosome)parents[1], targets);
            GSharpChromosome child1 = parents[0].CreateNew() as GSharpChromosome;
            child1.ReplaceGenes(0, parents[0].GetGenes());
            GSharpChromosome child2 = parents[1].CreateNew() as GSharpChromosome;
            child2.ReplaceGenes(0, parents[1].GetGenes());

            child1.ReplaceGene(indices[0], parents[1].GetGene(indices[1]));
            child1.legalLength = indices[0] + 1;
            for (int i = indices[0] + 1; i < child1.Length; i++) child1.ReplaceGene(i, child1.GenerateGene(i));
            child2.ReplaceGene(indices[1], parents[0].GetGene(indices[0]));
            child2.legalLength = indices[1] + 1;
            for (int i = indices[1] + 1; i < child2.Length; i++) child2.ReplaceGene(i, child2.GenerateGene(i));

            List<IChromosome> ret = new List<IChromosome>();
            ret.Add(child1);
            ret.Add(child2);
            return ret;
        }

        //Gets a list of BUYS for parent1 and crosses them to parent2.
        private Board[] matchBuys(GSharpChromosome parent1, GSharpChromosome parent2)
        {
            List<Board> boards1 = new List<Board>();
            List<Board> boards2 = new List<Board>();
            for (int i=0; i < parent1.legalLength; i++)
            {
                GSharpChromosome.gene gene = parent1.GetGene(i).Value as GSharpChromosome.gene;
                Debug.Assert(gene.move != null);
                if (gene.move.moveType == 2) boards1.Add(gene.beforeState);
            }
            for (int i = 0; i < parent2.legalLength; i++)
            {
                GSharpChromosome.gene gene = parent2.GetGene(i).Value as GSharpChromosome.gene;
                Debug.Assert(gene.move != null);
                if (gene.move.moveType == 2) boards2.Add(gene.beforeState);
            }
            List<int> hashes1 = boards1.ConvertAll(x => hash(x));
            List<int> hashes2 = boards2.ConvertAll(x => hash(x));
            foreach (int x in hashes1)
            {
                if (hashes2.Contains(x) && (hashes1.IndexOf(x) != hashes2.IndexOf(x)))
                {
                    return new Board[2] { boards1[hashes1.IndexOf(x)], boards2[hashes2.IndexOf(x)] };
                }
            }
            return null;
            

        }
    }
}
