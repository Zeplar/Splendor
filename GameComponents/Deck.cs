using System.Collections.Generic;
using System.Windows;
using System;
using System.Diagnostics;
namespace Splendor
{


    public class Deck
    {
        private List<Card> cards = new List<Card>();


        internal void removeCard(Card c)
        {
            cards.Remove(c);
        }

        /// <summary>
        ///Necessary for RecordHistory to get initial deck configuration. This is the actual cardlist; do not modify!!!
        /// </summary>
        /// <returns></returns>
        internal List<Card> getAllCards()
        {
            return cards;
        }

        //Returns the first (up to 4) visible cards.
        public List<Card> viewableCards
        {
            get
            {
                int size = Math.Min(4, cards.Count);
                return cards.GetRange(0, size);
            }
        }

        internal void shuffle()
        {
            cards.shuffle();
        }
        
        
        

    }

}
