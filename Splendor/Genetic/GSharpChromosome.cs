using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using System.Diagnostics;

namespace Splendor.Genetic
{
    class GSharpChromosome : ChromosomeBase
    {
        public class gene
        {
            public Board beforeState;
            public Move move;

            public gene()
            {
                Move m = Move.getRandomMove();
                Debug.Assert(m != null);
                beforeState = Board.current;
                move = m;
            }

            public gene(gene prior)
            {
                if (prior.move == null)
                {
                    move = null;
                    beforeState = prior.beforeState;
                }
                else
                {
                    beforeState = prior.beforeState.generate(Greedy.getGreedyMove(prior.beforeState));
                    move = Move.getRandomMove(beforeState);
                }
            }

        }

        public int legalLength = 0;

        public GSharpChromosome(int length) : base(length)
        {
            for (int i = 0; i < length; i++)
            {
                GetGenes()[i] = GenerateGene(i);
            }
        }

        public override IChromosome CreateNew()
        {
            Console.CursorLeft = 0; Console.Write("Creating a chromosome.");
            var ret = new GSharpChromosome(this.Length);
            Console.CursorLeft = 0; Console.Write("Created a chromosome of legal length {0}.", ret.legalLength);
            return ret;
        }

        public override Gene GenerateGene(int geneIndex)
        {
            if (geneIndex == 0)
            {
                legalLength++;
                return new Gene(new gene());
            }
            else
            {
                var ret = new Gene(new gene((gene)GetGene(geneIndex - 1).Value));
                if (((gene)ret.Value).move != null) legalLength++;
                return ret;
            }
        }
    }
}
