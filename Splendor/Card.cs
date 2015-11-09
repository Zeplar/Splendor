using System.Collections.Generic;
using System.Windows;

namespace Splendor
{


    public class Card
    {

        //Basic class for cards

        public static Vector dimension;
        //public Texture2D frontImage;
        //public Texture2D backImage;
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
            string name = "Card " + id.ToString() + ": " + cost.ToString() + ":" + color.ToString();
            return name;
        }




        public bool showCard(Vector pos, string side = "front")
        {
            bool wasClicked = false;
            //GUI.BeginGroup(new Rect(pos, dimension));

            //switch (side)
            //{
            //    case "front":
            //        wasClicked = GUI.Button(new Rect(Vector2.zero, dimension), frontImage);
            //        showCost();
            //        showType();
            //        break;

            //    case "back":
            //        wasClicked = GUI.Button(new Rect(Vector2.zero, dimension), backImage);
            //        break;

            //    default:
            //        Debug.LogError("Card side " + side + " does not exist");
            //        Debug.Break();
            //        return false;
            //}
            //GUI.EndGroup();
            return wasClicked;

        }
    }

}
