using System.Collections.Generic;
using System;
using AForge;

namespace Splendor
{


    public class Player
    {

        internal Gem gems;
        internal List<Card> reserve;
        internal List<Card> field;
        public string name;
        internal int wins = 0;
        internal int turnOrder;
        public int[] movesTaken = new int[4];

        public Gem Gems { get { return gems; } }

        public List<Card> Reserve { get { return new List<Card>(reserve); } }

        public List<Card> Field { get { return new List<Card>(field); } }

        public virtual void takeTurn() { }

        protected ThreadSafeRandom random { get { return GameController.Random; } }

        public int Wins {get { return wins; } }

        public override string ToString()
        {
            return name;
        }

        public bool canBuy(Board b, Card c)
        {
            try
            {
                (gems + discount).takeaway(c.cost);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        protected void showState()
        {
            Console.Clear();
            Console.WriteLine("Board:");
            Console.WriteLine(Gem.board + "    Nobles: " + GameController.nobles.viewableCards.String());
            Console.WriteLine("Tier1: " + GameController.decks[2].viewableCards.String());
            Console.WriteLine("Tier2: " + GameController.decks[1].viewableCards.String());
            Console.WriteLine("Tier3: " + GameController.decks[0].viewableCards.String());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Self: " + gems + "|Field: " + field.String() + "   |Reserve: " + reserve.String());

        }

        public Player() : this("BasePlayer")
        {

        }

        public Player(string name)
        {
            gems = new Gem();
            reserve = new List<Card>();
            field = new List<Card>();
            this.name = name;
        }
        

        internal void takeGems(Gem x)
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
                    if (c.deck != GameController.nobles)
                    {
                        d[c.color] += 1;
                    }
                }
                return d;
            }

        }

        public bool canGetNoble(out Card noble)
        {
            foreach (Card c in GameController.nobles.getAllCards())
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


        internal void Buy(Card c)
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

        public void takeAction(Move m)
        {
            if (GameController.currentPlayer == this)
            {
                movesTaken[(int)m.moveType]++;
                m.takeAction();
            }
            else throw new Exception(this + " tried to take its turn out of order!");
        }

        internal void m_reserve(Card c)
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
