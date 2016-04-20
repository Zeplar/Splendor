using System;
using System.Collections.Generic;
using AForge.Genetic;
using System.Threading;

namespace Splendor.BuyOrder
{
    public class BuyFit : IFitnessFunction
    {
        public List<Card> cards;
        private Heuristic scoringFunction;
        private static object Lock = new object();
        public int timesEvaluated = 0;
        private int depth = 6;
        public bool willWin; //Whether the last evaluation determined a win was imminent.
        public bool willLose;
        public Move predictedMove; //Move the algorithm predicts Greedy will take.

        private bool deny = false; //Flag whether the current target is a deny target

        public BuyFit(Heuristic scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        
        /// <summary>
        /// Used to halt evaluation if the board state is empty or a player has won.
        /// Sets flags which can be used for data collection.
        /// </summary>
        private bool predictWin(Board current)
        {
            willWin = willLose = false;
            if (!current.viewableCards.Exists(x => x.Deck != Card.Decks.nobles)) return true;
            if (current.gameOver)
            {
                if (current.winner == 0) willWin = true;
                else if (current.winner == 1) willLose = true;
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
                score -= scoringFunction.evaluate(current);
            }
            score = Math.Max(1, score);

            Interlocked.Increment(ref timesEvaluated);

            if (c.parentFitness > 0 && score > c.parentFitness)
            {
                BuyOrderChromosome.crossOverImprovements += 1;
            }
            else if (c.parentFitness < 0 && score > -c.parentFitness)
            {
                BuyOrderChromosome.mutationImprovements += 1;
            }

            return score;


        }

        private Move simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, scoringFunction);
            return m;
        }

        /// <summary>
        /// If the ith card is still available, return True and set card = that card. Else return False.
        /// </summary>
        private bool containsBuy(BuyOrderChromosome c, int i, Board current, out Card card)
        {
            int id = c.Value[i] % cards.Count;
            deny = (c.Value[i] >= cards.Count); //Set the deny flag
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
            Move nextMove;
            if (deny && new Move.RESERVE(card[0]).isLegal(current))
            {
                nextMove = new Move.RESERVE(card[0]);
            }
            else nextMove = BuySeeker.getMove(current, card, new int[] { 2, 1 });
            c.depth = nextBuy;
            return current.generate(nextMove);
        }
    }
}
