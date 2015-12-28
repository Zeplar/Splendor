using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Splendor.BuyOrder
{
    public class Fitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            Chromosome c = (Chromosome)chromosome;

            Board current = Board.current;
            int i = 0;
            while (i < current.viewableCards.Count && !current.gameOver)
            {
                simulateMyTurn(c, current, i);
                simulateGreedyTurn(current);
            }
            return ScoringMethods.DeltaPoints(current);


        }

        private void simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, ScoringMethods.DeltaPoints);
            current = current.generate(m);
        }

        private void simulateMyTurn(Chromosome c, Board current, int i)
        {
            while (!current.gameOver && i < current.viewableCards.Count)
            {
                int nextBuy = (int)c.GetGene(i).Value;
                if (current.viewableCards.Contains(Card.allCardsByID[nextBuy]))
                {
                    Move nextMove = new BuySeeker(Card.allCardsByID[nextBuy], current).getMove();
                    current = current.generate(nextMove);
                    i++;
                    return;
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
