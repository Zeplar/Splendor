using System;
using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;
using System.Text;

namespace Splendor.BuyOrder
{
    public class BuyFit : IFitnessFunction
    {
        public List<Card> cards;
        private Func<Board, int> scoringFunction;

        public BuyFit(Func<Board,int> scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }


        public double Evaluate(IChromosome chromosome)
        {
            PermutationChromosome c = (PermutationChromosome)chromosome;
            Board current = Board.current;
            int i = 0;
            while (i < 10)
            {
                if (current.gameOver) break;
                current = simulateMyTurn(c, current);
                if (current.gameOver) break;
                current = simulateGreedyTurn(current);
                i++;
            }
            Debug.Assert(current.gameOver ? true : (current != Board.current), "Boardstate did not evolve.");
            return scoringFunction(current);


        }

        private Board simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, ScoringMethods.DeltaPoints);
            return current.generate(m);
        }

        private Board simulateMyTurn(PermutationChromosome c, Board current)
        {
            int nextBuy = 0;
            if (current.gameOver) return current;
            while (!current.viewableCards.Contains(cards[c.Value[nextBuy]]))
            {
                nextBuy++;
                if (nextBuy >= current.viewableCards.Count) return current;
            }
            
            Move nextMove = new BuySeeker(cards[c.Value[nextBuy]], current).getMove();
            Debug.Assert(nextMove != null, "BuySeeker shouldn't return null on !gameOver boards");
            return current.generate(nextMove);
        }
    }
}
