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

        //static Dictionary<int, Color> dict = new Dictionary<int, Color>();

        public Card()
        {
            //frontImage = Texture2D.whiteTexture;
            //if (dict.ContainsKey(0))
            //    return;
            //dict.Add(0, Color.white);
            //dict.Add(1, Color.blue);
            //dict.Add(2, Color.black);
            //dict.Add(3, Color.red);
            //dict.Add(4, Color.green);
            //dimension = new Vector2(80, 120);
        }

        public override string ToString()
        {
            return cost.ToString();
        }
        void showCost()
        {
            //Vector2 position = new Vector2(5, dimension.y - 25);
            //Vector2 dim = new Vector2(25, 25);
            //int i;
            //for (i = 0; i < 5; i++)
            //{
            //    if (cost[i] != 0)
            //    {
            //        GUI.Label(new Rect(position, dim), global::Splendor.gem(i));
            //        GUIStyle g = new GUIStyle();
            //        g.fontStyle = FontStyle.BoldAndItalic;
            //        g.fontSize = 22;
            //        GUI.Label(new Rect(position.x + 3, position.y, dim.x, dim.y), cost[i].ToString(), g);
            //        g.fontSize = 20;
            //        g.normal.textColor = Color.white;
            //        GUI.Label(new Rect(position.x + 4, position.y + 3, dim.x, dim.y), cost[i].ToString(), g);
            //        position = position - 30 * Vector2.up;
            //    }
            //}
        }

        void showType()
        {
            //Vector2 position = new Vector2(dimension.x - 20, 0);
            //Vector2 dim = new Vector2(25, 25);
            //GUI.BeginGroup(new Rect(position, dim));
            //GUI.Label(new Rect(Vector2.zero, dim), global::Splendor.gem(color));
            //GUI.Label(new Rect(5 * Vector2.right, dim), points.ToString());
            //GUI.EndGroup();
        }

        public void getBought()
        {
            deck.removeCard(this);
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
