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

        public static bool operator ==(Card a, Card b)
        {
            return a.id == b.id;
        }
        public static bool operator !=(Card a, Card b)
        {
            return a.id != b.id;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Card)) return this == (Card)obj;
            return false;
        }
        public override int GetHashCode()
        {
            return this.id;
        }
    }

}
