using System.Collections.Generic;
using System.Numerics;
using System;

namespace Splendor
{


    public class Player
    {

        public int turnOrder;
        public Gem gems;
        public List<Card> reserve;
        public List<Card> field;
        public int wins;
        public string name;

        protected Random random {get {return Splendor.random;}}

        public virtual void takeTurn() { }

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
                if (discount >= c.cost)
                {
                    noble = c;
                    return true;
                }
            }
            noble = null;
            return false;
        }


        public void Buy(Card c)
        {
            Gem newGems = gems.takeaway((c.cost - discount));
            Gem.board += (gems - newGems);
            gems = newGems;
            field.Add(c);
            c.deck.removeCard(c);
            reserve.Remove(c);
            Card noble;
            if (canGetNoble(out noble))
            {
                noble.deck.removeCard(noble);
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
            c.deck.removeCard(c);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }
}
