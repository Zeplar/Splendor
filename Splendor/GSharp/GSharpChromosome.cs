using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using System.Diagnostics;

namespace Splendor.GSharp
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
                    beforeState = prior.beforeState.generate(Greedy.getGreedyMove(prior.beforeState, ScoringMethods.DeltaPoints));
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
            var ret = new GSharpChromosome(this.Length);
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
                var prior = (gene)GetGene(geneIndex - 1).Value;
                if (prior.beforeState.gameOver) return GetGene(geneIndex - 1);
                var ret = new Gene(new gene(prior));
                if (((gene)ret.Value).move != null) legalLength++;
                return ret;
            }
        }
    }
}
