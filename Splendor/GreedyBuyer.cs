using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
    public class GreedyBuyer : Player
    {
        private List<Card> buyOrder;

        public override void takeTurn()
        {
            buyOrder = Board.current.viewableCards.FindAll(x => x.deck != GameController.nobles);
            buyOrder.OrderByDescending(x => x.points);
            foreach (Card c in buyOrder)
            {
                if (new Move.BUY(c).isLegal())
                {
                    new Move.BUY(c).takeAction();
                    return;
                }
            }
            Move m = new BuySeeker(buyOrder[0]).getMove();
            m.takeAction();
            return;
        }

    }
}
