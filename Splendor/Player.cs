using System.Collections.Generic;
using System;
using System.Diagnostics;

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

        protected bool takeRandomTurn()
        {
 //           if (Splendor.turn >= 47) Debug.Assert(false);
            List<Move> moves = Move.getAllLegalMoves();
            if (moves.Count > 0)
            {
                moves[random.Next(moves.Count)].takeAction();
                return true;
            }
            Gem priorGems = this.gems;
            this.gems = Gem.zero;


            moves = Move.TAKE3.getLegalMoves().ConvertAll(x => (Move)x);
            if (moves.Count > 0)
            {
                moves[0].takeAction();
                this.gems += priorGems;
                while (this.gems.magnitude > 10)
                {
                    returnRandomGems();
                }
                return true;
            }

            moves = Move.TAKE2.getLegalMoves().ConvertAll(x => (Move)x);
            if (moves.Count > 0)
            {
                moves[0].takeAction();
                this.gems += priorGems;
                while (this.gems.magnitude > 10)
                {
                    returnRandomGems();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a random gem to the pile.
        /// </summary>
        private void returnRandomGems()
        {
            int k = 0;

            while (true)
            {
                k = random.Next(5);
                if (gems[k] > 0)
                {
                    Gem.board[k] += 1;
                    gems[k] -= 1;
                    return;
                }
            }
        }

    }
}
