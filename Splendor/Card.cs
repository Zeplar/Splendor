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
        private static string[] colors = { "W", "U", "B", "R", "G" };

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
            return (deck == Splendor.nobles) ? "N" + (-id) + ":" + cost + points : "C" + id + cost + points + "(" + colors[color] + ")";
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
