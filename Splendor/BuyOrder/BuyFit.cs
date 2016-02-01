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
        private ScoringMethods.Function scoringFunction;
        private ScoringMethods.Function greedy = ScoringMethods.minPoints;

        public BuyFit(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        private bool predictWin(Board current)
        {
            if (current.gameOver)
            {
                if (current.winner == 0) RecordHistory.record("!!! " + GameController.currentPlayer + " now thinks it's going to win!");
                else if (current.winner == 1) RecordHistory.record("!!! " + GameController.currentPlayer + " now thinks it's going to lose!");
                return true;
            }
            return false;
        }

        public double Evaluate(IChromosome chromosome)
        {
            PermutationChromosome c = (PermutationChromosome)chromosome;
            Board current = Board.current;
            int i = 0;
            double score = 0;
            while (i < 10)
            {

                if (predictWin(current)) break;
                current = simulateMyTurn(c, current);
                score += scoringFunction.evaluate(current);
                if (predictWin(current)) break;
                if (i == 0) SelfishGene.predicted = simulateGreedyTurn(current);
                current = current.generate(simulateGreedyTurn(current));
                i++;
            }
            return Math.Max(1, score / i);


        }

        private Move simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, greedy);
            return m;
        }

        public Board simulateMyTurn(PermutationChromosome c, Board current)
        {
            int nextBuy = 0;
            while (!current.viewableCards.Contains(cards[c.Value[nextBuy]]))
            {
                nextBuy++;
                if (current.viewableCards.Count == 0) throw new IndexOutOfRangeException("Got through all of the cards.");
            }
            
            Move nextMove = new BuySeeker(cards[c.Value[nextBuy]], current).getMove();
            if (nextMove == null) throw new Exception("BuySeeker shouldn't return null on !gameOver boards");
            return current.generate(nextMove);
        }
    }
}
