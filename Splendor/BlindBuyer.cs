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
            foreach (Card c in buyOrder)
            {
                if (!Splendor.boardCards.Contains(c))
                {
                    buyOrder.Remove(c);
                } else
                {
                    new BuySeeker(c).getMove().takeAction();
                    return;
                }
            }
        }

    }
}
