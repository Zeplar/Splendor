using System.Collections.Generic;
using System.Windows;
using System;
namespace Splendor
{


    public class Deck
    {

        public Vector position;
        private List<Card> cards = new List<Card>();

        public void OnGUI()
        {
            if (cards.Count > 0)
            {
                showCards();
                showDeck();
            }
        }

        public void removeCard(Card c)
        {
            cards.Remove(c);
        }

        //Necessary for RecordHistory to get initial deck configuration
        public List<Card> getAllCards()
        {
            return cards;
        }

        //Marks the selected card so the human player can decide whether to buy or reserve it.
        void selectCard(Card c)
        {
            Splendor.selected = c;
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


        //Displays the first four faceup cards.
        private void showCards()
        {
            int i = 0;
            foreach (Card c in viewableCards)
            {
                if (c.showCard(position + (1 + i) * (Card.dimension.X + 10) * new Vector(1,0), "front"))
                {
                    selectCard(c);
                }
                i++;
            }
        }
        //!!! look for a better shuffling algorithm
        public void shuffle()
        {
            int i = 0;
            for (; i < cards.Count; i++)
            {
                int j = Splendor.random.Next(i, cards.Count);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        //Displays the facedown deck.
        private void showDeck()
        {
            if (cards.Count <= 4)
            {
                return;
            }
            Card c = cards[4];
            if (c.showCard(position, "back"))
            {
                selectCard(c);
            }
        }

    }

}
