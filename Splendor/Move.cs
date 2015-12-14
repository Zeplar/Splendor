using System.Diagnostics;
using System.Collections.Generic;
using System.Text;


namespace Splendor
{

    public abstract class Move
    {

        /// <summary>
        /// TAKE2, TAKE3, BUY, RESERVE
        /// </summary>
        public int moveType;

        public static string ListToString(List<Move> m)
        {
            StringBuilder s = new StringBuilder();
            foreach (Move move in m)
            {
                s.Append(move.ToString() + ", ");
            }
            return s.ToString();
        }

        public abstract bool isLegal();
        public abstract bool isLegal(Board b);

        public virtual void takeAction()
        {
            switch (moveType)
            {
                case 0:
                    ((TAKE2)this).takeAction();
                    return;
                case 1:
                    ((TAKE3)this).takeAction();
                    return;
                case 2:
                    ((BUY)this).takeAction();
                    return;
                case 3:
                    ((RESERVE)this).takeAction();
                    return;
                default:
                   Debug.Fail("Illegal Move");
                    return;
            }
        }

        public override string ToString()
        {
            switch (moveType)
            {
                case 0:
                    return ((TAKE2)this).ToString();
                case 1:
                    return ((TAKE3)this).ToString();
                case 2:
                    return ((BUY)this).ToString();
                case 3:
                    return ((RESERVE)this).ToString();
                default:
                    return "Illegal Move";
            }
        }

        public static List<Move> getAllLegalMoves()
        {
            return getAllLegalMoves(Board.current);
        }

        
        /// <summary>
        /// Gets all legal moves sorted BUY-TAKE3-TAKE2-RESERVE
        /// </summary>
        /// <param name="bd">The board</param>
        /// <returns></returns>
        public static List<Move> getAllLegalMoves(Board bd)
        {

            List<TAKE2> a = TAKE2.getLegalMoves(bd);
            List<TAKE3> b = TAKE3.getLegalMoves(bd);
            List<BUY> c = BUY.getLegalMoves(bd);
            List<RESERVE> d = RESERVE.getLegalMoves(bd);
            List<Move> l = new List<Move>();

            l.AddRange(c.ConvertAll(x => (Move)x));
            l.AddRange(b.ConvertAll(x => (Move)x));
            l.AddRange(a.ConvertAll(x => (Move)x));
            l.AddRange(d.ConvertAll(x => (Move)x));
            return l;
        }

        public static Move getRandomMove()
        {
            return getRandomMove(Board.current);
        }

        /// <summary>
        /// Returns a random legal move for the given board, or null if there are none.
        /// </summary>
        /// <param name="bd"></param>
        /// <returns></returns>
        public static Move getRandomMove(Board bd)
        {
            List<Move> m = getAllLegalMoves(bd);
            return (m.Count > 0) ? m[Splendor.random.Next(m.Count)] : null;
        }

        public class TAKE2 : Move
        {

            public int color;

            public TAKE2(int i)
            {
                color = i;
                moveType = 0;
            }

            public override string ToString()
            {
                Gem v = new Gem();
                v[color] = 2;
                return v.ToString();
            }


            /// <summary>
            /// Returns True if currentPlayer could take these gems on the current board.
            /// </summary>
            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            /// <summary>
            /// Returns True if currentPlayer could take these gems on the given board. 	
            /// </summary>
            public override bool isLegal(Board b)
            {

                bool haveRoom = b.currentPlayer.gems.magnitude <= 8;
                bool avail = b.gems[color] >= 4;
                return (haveRoom && avail);
            }

            public override void takeAction()
            {
     //           Console.WriteLine(Splendor.currentPlayer + " chose move " + this.ToString());
                if (isLegal())
                {
                    Gem c = new Gem();
                    c[color] = 2;
                    Splendor.currentPlayer.gems += c;
                    Gem.board -= c;
                    Splendor.nextTurn();
                }
                else
                {
                    Debug.Fail("Illegal move! " + this);
                }
            }

            public static List<TAKE2> getLegalMoves()
            {
                return getLegalMoves(Board.current);
            }

            public static List<TAKE2> getLegalMoves(Board b)
            {
                List<TAKE2> l = new List<TAKE2>();
                TAKE2 temp;
                for (int i = 0; i < 5; i++)
                {
                    temp = new TAKE2(i);
                    if (temp.isLegal(b))
                    {
                        l.Add(temp);
                    }
                }
                return l;
            }
        }

        public class TAKE3 : Move
        {

            public Gem colors;

            public TAKE3(Gem x)
            {

                moveType = 1;
                colors = x;
            }

            public TAKE3(int i, int j, int k)
            {
                moveType = 1;
                colors[i] = colors[j] = colors[k] = 1;
            }

            public override string ToString()
            {
                return colors.ToString();
            }

            public static List<TAKE3> getLegalMoves()
            {
                return getLegalMoves(Board.current);
            }

            public static List<TAKE3> getLegalMoves(Board b)
            {
                List<TAKE3> l = new List<TAKE3>();
                foreach (Gem g in Gem.AllThree)
                {
                    TAKE3 t = new TAKE3(g);
                    if (t.isLegal(b)) l.Add(new TAKE3(g));
                }
                return l;
            }

            public override bool isLegal(Board b)
            {
                bool haveRoom = b.currentPlayer.gems.magnitude <= 7;
                bool avail = b.gems > colors;
                if (!haveRoom || !avail)
                for (int i = 0; i < 5; i++)
                {
                    if (colors[i] < 0 || colors[i] > 1)
                    {
                        return false;
                    }
                }
                return avail && haveRoom && (colors.magnitude == 3) && (colors[5] == 0);
            }

            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            public override void takeAction()
            {
         //       Console.WriteLine(Splendor.currentPlayer + " chose move " + this.ToString());
                if (isLegal())
                {
                    Splendor.currentPlayer.takeGems(colors);
                    Splendor.nextTurn();
                }
                else
                {
                    Debug.Fail("Illegal move! " + this);
                }
            }

        }

        public class BUY : Move
        {

            public Card card;

            public BUY(Card c)
            {
                this.card = c;
                moveType = 2;
            }

            public override string ToString()
            {
                return "BUY " + card.id;
            }

            public static List<BUY> getLegalMoves(Board b)
            {

                List<BUY> l = new List<BUY>();
                BUY temp;

                foreach (Card c in b.viewableCards)
                {
                    temp = new BUY(c);
                    if (temp.isLegal(b))
                    {
                        l.Add(temp);
                    }
                }
                foreach (Card c in b.currentPlayer.reserve)
                {
                    temp = new BUY(c);
                    if (temp.isLegal(b))
                    {
                        l.Add(temp);
                    }
                }
                return l;
            }

            public static List<BUY> getLegalMoves()
            {
                return getLegalMoves(Board.current);
            }

            public override bool isLegal(Board b)
            {
                bool exists = b.viewableCards.Contains(card) || b.currentPlayer.reserve.Contains(card);
                exists &= (card.deck != Splendor.nobles);
                bool affordable = (b.currentPlayer.gems + b.currentPlayer.discount - card.cost).deficit <= b.currentPlayer.gems[5];
                return exists && affordable;
            }

            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            public override void takeAction()
            {
                Debug.Assert(Board.current.currentPlayer == Splendor.currentPlayer, "Board is misrepresenting players: " + Board.current.currentPlayer.detailedInfo + " != " + Splendor.currentPlayer.detailedInfo);

                if (isLegal())
                {
                    Splendor.currentPlayer.Buy(card);
                    Splendor.nextTurn();
                }
                else
                {
                   Debug.Fail("Illegal move! " + this);
                }
            }
        }

        public class RESERVE : Move
        {

            public Card card;

            public RESERVE(Card c)
            {
                this.card = c;
                moveType = 3;
            }

            public override string ToString()
            {
                return "RESERVE " + card.id;
            }

            public static List<RESERVE> getLegalMoves(Board b)
            {

                List<RESERVE> l = new List<RESERVE>();

                if (b.currentPlayer.reserve.Count >= 3)
                {
                    return l;
                }

                foreach (Card c in b.viewableCards)
                {
                    if (c.id >= 0)
                    {
                        l.Add(new RESERVE(c));
                    }
                }

                return l;
            }

            public static List<RESERVE> getLegalMoves()
            {
                return getLegalMoves(Board.current);
            }

            public override bool isLegal(Board b)
            {

                bool canReserve = b.currentPlayer.reserve.Count < 3;
                if (!canReserve)
                {
                    return false;
                }
                bool isAvail = b.viewableCards.Contains(card) && card.deck != Splendor.nobles;
                return isAvail;
            }

            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            public override void takeAction()
            {
          //      Console.WriteLine(Splendor.currentPlayer + " chose move " + this.ToString());

                if (isLegal())
                {
                    Splendor.currentPlayer.Reserve(card);
                    Splendor.nextTurn();
                }
                else
                {
                    Debug.Fail("Illegal move! " + this);
                }
            }
        }

    }

}
