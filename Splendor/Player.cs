using System.Collections.Generic;
using System.Numerics;

namespace Splendor
{


    public abstract class Player
    {

        public int turnOrder;
        public Gem gems;
        public List<Card> reserve;
        public List<Card> field;
        public int wins;

        public abstract void takeTurn();

        public string detailedInfo
        {
            get
            {
                return "turnOrder: " + turnOrder + " Gems: " + gems + " Card Count: " + field.Count;
            }
        }

        public Player()
        {
            gems = new Gem();
            reserve = new List<Card>();
            field = new List<Card>();
        }

        public void takeGems(Gem x)
        {
            Gem.board -= x;
            gems += x;
        }

        //public virtual void display()
        //{
        //    if (turnOrder == 0)
        //    {
        //        GUI.BeginGroup(new Rect(0, 15, Screen.width, Card.dimension.y * 2));
        //        showField();
        //        showReserve();
        //        GUI.BeginGroup(new Rect(0, Card.dimension.y + 10, Screen.width, Card.dimension.y));
        //        showGems();
        //        GUI.EndGroup();
        //        GUI.EndGroup();
        //    }
        //    else
        //    {
        //        GUI.BeginGroup(new Rect(0, Screen.height - 2 * Card.dimension.y, Screen.width, Card.dimension.y * 2));
        //        showGems();
        //        GUI.BeginGroup(new Rect(0, Card.dimension.y - 15, Screen.width, Card.dimension.y));
        //        showField();
        //        showReserve();
        //        GUI.EndGroup();
        //        GUI.EndGroup();

        //    }
        //}

        public int points
        {
            get
            {
                int i = 0;
                foreach (Card c in field)
                {
                    i += c.points;
                }
                return i;
            }
        }

        public Gem discount
        {
            get
            {
                Gem d = new Gem();
                foreach (Card c in field)
                {
                    if (c.deck != Splendor.nobles)
                    {
                        d[c.color] += 1;
                    }
                }
                return d;
            }

        }

        public bool canGetNoble(out Card noble)
        {
            foreach (Card c in Splendor.nobles.getAllCards())
            {
                if (discount > c.cost)
                {
                    noble = c;
                    return true;
                }
            }
            noble = new Card();
            return false;
        }


        public void Buy(Card c)
        {
            Gem newGems = gems.takeaway((c.cost - discount));
            Gem.board += (gems - newGems);
            gems = newGems;
            field.Add(c);
            c.getBought();
            reserve.Remove(c);
            Card noble;
            if (canGetNoble(out noble))
            {
                noble.getBought();
                field.Add(noble);
            }

            //Make sure we have conservation of gems
            UnitTests.testBoard();
        }

        public void Reserve(Card c)
        {
            if (Gem.board[5] > 0)
            {
                gems[5] += 1;
                Gem.board[5] -= 1;
            }
            reserve.Add(c);
            c.getBought();
        }

        protected virtual void showReserve()
        {
            //Vector2 pos = new Vector2((2 + field.Count + reserve.Count) * Card.dimension.x, 0);
            //foreach (Card c in reserve)
            //{
            //    c.showCard(pos, "front");
            //    pos -= new Vector2(Card.dimension.x + 10, 0);
            //}
            //pos -= new Vector2(Card.dimension.x, 0);
            //GUIStyle x = new GUIStyle();
            //x.fontSize = 30;
            //x.normal.textColor = Color.white;
            //GUI.Label(new Rect(pos, new Vector2(100, 50)), "Reserve", x);
        }


        private void showField()
        {
            Vector2 pos = new Vector2(15, 0);
            foreach (Card c in field)
            {
                //c.showCard(pos, "front");
                //pos += new Vector2(Card.dimension.x + 10, 0);
            }
        }

        private void showGems()
        {
            //Vector2 position = new Vector2(15, 0);
            //Vector2 spacing = new Vector2(75, 0);
            //Vector2 dimension = new Vector2(50, 50);
            //int i;
            //GUI.BeginGroup(new Rect(position.x, position.y, 75 * 6, dimension.y));
            //position = Vector2.zero;
            //GUIStyle g = new GUIStyle();
            //for (i = 0; i < 6; i++)
            //{
            //    g.normal.textColor = Color.black;
            //    g.fontSize = 32;
            //    GUI.Label(new Rect(position, dimension), Splendor.GemIcons[i]);
            //    position += new Vector2(5, 5);
            //    GUI.Label(new Rect(position, dimension), gems[i].ToString(), g);
            //    g.fontSize = 26;
            //    g.normal.textColor = Color.white;
            //    position += new Vector2(2, 2);
            //    GUI.Label(new Rect(position, dimension), gems[i].ToString(), g);
            //    position -= new Vector2(7, 7);
            //    position += spacing;
            //}
            //GUI.color = Color.yellow;
            //GUI.Label(new Rect(position, dimension), Splendor.GemIcons[0]);
            //GUI.color = Color.white;
            //GUI.EndGroup();

        }

    }
}
