using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
    public class BlindBuyer : Player
    {
        private List<Card> buyOrder = new List<Card>();

        public override void takeTurn()
        {
            foreach (Card c in Splendor.boardCards)
            {
                if (!buyOrder.Contains(c)) buyOrder.Add(c);
            }
            while (buyOrder.Count > 0)
            {
                if (!Board.current.boardCards.Contains(buyOrder[0])) buyOrder.RemoveAt(0);
                else
                {
                    Move m = new BuySeeker(buyOrder[0]).getMove();
                    m.takeAction();
                    return;
                }
            }
        }

    }
}
