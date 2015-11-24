using System.Collections.Generic;
using System.Windows;

namespace Splendor
{


    public class Card
    {

        //Basic class for cards

        public Gem cost;
        public int color;
        public int points;
        public Deck deck;
        public int id;
        public static Card[] allCardsByID = new Card[90]; //currently not including Nobles

        public Card(int id)
        {
            this.id = id;
            if (id < 0)
            {
                return;
            }
            allCardsByID[id] = this;
        }

        public override string ToString()
        {
            string name = "C" + id.ToString() + ": " + cost.ToString() + ":" + color.ToString();
            return name;
        }
    }

}
