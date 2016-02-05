using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Splendor
{
    public class GreedyBuyer : Player
    {
        private List<Card> buyOrder;
        private BuySeeker buyer = new BuySeeker();

        public override void takeTurn()
        {
            Board current = Board.current;
            buyOrder = current.viewableCards.FindAll(x => x.Deck != Card.Decks.nobles);
            buyOrder.OrderBy(x => random.Next());
            foreach (Card c in buyOrder)
            {
                if (canBuy(current, c))
                {
                    takeAction(new Move.BUY(c));
                    return;
                }
            }
            Move m = buyer.getMove(Board.current, buyOrder[0]);
            takeAction(m);
            return;
        }

    }
}
