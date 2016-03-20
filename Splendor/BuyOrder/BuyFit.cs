using System;
using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;

namespace Splendor.BuyOrder
{
    public class BuyFit : IFitnessFunction
    {
        public List<Card> cards;
        private ScoringMethods.Function scoringFunction;
        private ScoringMethods.Function greedy = ScoringMethods.Points;
        public int timesEvaluated = 0;
        private int depth = 1;

        public BuyFit(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        private bool predictWin(Board current)
        {
            if (!current.viewableCards.Exists(x => x.Deck != Card.Decks.nobles)) return true;
            if (current.gameOver)
            {
                if (current.winner == 0) RecordHistory.current.record("!!! " + GameController.currentPlayer + " now thinks it's going to win!");
                else if (current.winner == 1) RecordHistory.current.record("!!! " + GameController.currentPlayer + " now thinks it's going to lose!");
                return true;
            }
            return false;
        }


        public double Evaluate(IChromosome chromosome)
        {
            BuyOrderChromosome c = (BuyOrderChromosome)chromosome;
            Board current = Board.current;
            int i = 0;
            double score = 0;
            while (i < depth)
            {
                i++;
                if (predictWin(current)) break;
                current = simulateMyTurn(c, current);
                score += scoringFunction.evaluate(current);
                if (predictWin(current)) break;
                current = current.generate(simulateGreedyTurn(current));
            }
            timesEvaluated++;
            return Math.Max(1, score / i);


        }

        private Move simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, greedy);
            return m;
        }

        private bool containsBuy(BuyOrderChromosome c, int i, Board current, out Card card)
        {
            int id = c.Value[i];
            card = cards[id];
            return current.viewableCards.Contains(card);
        }

        public Board simulateMyTurn(BuyOrderChromosome c, Board current)
        {
            int nextBuy = 0;
            Card[] card = new Card[2];
            while (!containsBuy(c, nextBuy, current, out card[0]))
            {
                nextBuy++;
            }
            int nextnextBuy = nextBuy + 1;
            if (nextnextBuy < c.Length)
            {
                while (!containsBuy(c, nextnextBuy, current, out card[1]))
                {
                    nextnextBuy++;
                    //If there's only one card left, return to the old algorithm.
                    if (nextnextBuy >= c.Length)
                    {
                        card[1] = card[0];
                        break;
                    }
                }
            }
            else card[1] = card[0];
            //Move nextMove = BuySeeker.getMove(current, card);
            Move nextMove = BuySeeker.getMove(current, card, new int[] { 2, 1 });
            c.depth = nextBuy;
            return current.generate(nextMove);
        }
    }
}
