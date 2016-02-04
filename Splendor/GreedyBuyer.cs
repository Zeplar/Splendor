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
            buyOrder = Board.current.viewableCards.FindAll(x => x.deck != GameController.nobles);
            buyOrder.OrderBy(x => random.Next());
            foreach (Card c in buyOrder)
            {
                if (new Move.BUY(c).isLegal())
                {
                    new Move.BUY(c).takeAction();
                    return;
                }
            }
            Move m = buyer.getMove(Board.current, buyOrder[0]);
            m.takeAction();
            return;
        }

    }
}
