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
        private BuySeeker buyer = new BuySeeker();

        public BuyFit(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        private bool predictWin(Board current)
        {
            if (!current.viewableCards.Exists(x => x.Deck != Card.Decks.nobles)) return true;
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
                i++;
                if (predictWin(current)) break;
                current = simulateMyTurn(c, current);
                score += scoringFunction.evaluate(current);
                if (predictWin(current)) break;
                current = current.generate(simulateGreedyTurn(current));
            }
            return Math.Max(1, score / i);


        }

        private Move simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, greedy);
            return m;
        }

        private bool containsBuy(PermutationChromosome c, int i, Board current, out Card card)
        {
            int value = -1;
            card = null;
            try { value = c.Value[i]; } catch (IndexOutOfRangeException) { Console.WriteLine("Value out of range: " + i);
                Console.WriteLine("Cardlist: " + cards.String());
                Console.WriteLine("Real cards: " + current.viewableCards.String()); }
            try { card = cards[value]; } catch (IndexOutOfRangeException) { Console.WriteLine("Card out of range: " + value); }
            return current.viewableCards.Contains(card);
        }

        public Board simulateMyTurn(PermutationChromosome c, Board current)
        {
            int nextBuy = 0;
            Card card;
            while (!containsBuy(c, nextBuy, current, out card))
            {
                nextBuy++;
            }

            Move nextMove = buyer.getMove(current, card);
            return current.generate(nextMove);
        }
    }
}
