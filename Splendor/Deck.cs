﻿using System.Collections.Generic;
using System.Windows;
using System;
using System.Diagnostics;
namespace Splendor
{


    public class Deck
    {
        private List<Card> cards = new List<Card>();


        public void removeCard(Card c)
        {
            cards.Remove(c);
            Debug.Assert(!cards.Contains(c));
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

        public void shuffle()
        {
            this.cards.shuffle();
        }
        
        
        

    }

}
