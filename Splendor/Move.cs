﻿using System.Diagnostics;
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

        public abstract void takeAction();

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

            public Gem color;
            public int index;

            public TAKE2(int i)
            {
                color = new Gem();
                color[i] = 2;
                index = i;
                moveType = 0;
            }
            public TAKE2(Gem g)
            {
                color = g;
                for (int i=0; i < 5; i++)
                {
                    if (g[i] > 0)
                    {
                        index = i;
                        break;
                    }
                }
                moveType = 0;
            }

            public override string ToString()
            {
                return color.ToString();
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
                Gem after = b.currentPlayer.gems + color;
                bool haveRoom = after.magnitude <= 10 && after.deficit == 0;
                bool avail = b.gems[index] >= 4;
                return (haveRoom && avail);
            }

            public override void takeAction()
            {
     //           Console.WriteLine(Splendor.currentPlayer + " chose move " + this.ToString());
                if (isLegal())
                {
                    Splendor.currentPlayer.gems += color;
                    Gem.board -= color;
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
                if (b.currentPlayer.gems.magnitude <= 8)
                {
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

                foreach (Gem g in Gem.ExchangeTwo)
                {
                    temp = new TAKE2(g);
                    if (temp.isLegal(b)) l.Add(temp);
                }
                return l;
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Move.TAKE2)) return false;
                return ((Move.TAKE2)obj).color == color;
            }

            public override int GetHashCode()
            {
                return color.GetHashCode();
            }
        }

        public class TAKE3 : Move
        {

            public Gem colors;

            private TAKE3(Gem x)
            {

                moveType = 1;
                colors = x;
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

                if (b.currentPlayer.gems.magnitude > 7)
                {
                    foreach (Gem g in Gem.ExchangeThree)
                    {
                        if (g.magnitude + b.currentPlayer.gems.magnitude <= 10)
                        {
                            TAKE3 t = new TAKE3(g);
                            if (t.isLegal(b)) l.Add(t);
                        }
                    }
                    return l;
                }

                foreach (Gem g in Gem.ThreeNetThree)
                {
                    TAKE3 t = new TAKE3(g);
                    if (t.isLegal(b)) l.Add(t);
                }
                return l;
            }

            public override bool isLegal(Board b)
            {
                Gem after = b.currentPlayer.gems + this.colors;
                bool avail = b.gems > colors;
                bool haveRoom = after.magnitude <= 10 && after.deficit == 0;
                if (!haveRoom || !avail) return false;
                return avail && haveRoom && (colors[5] == 0);
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
                }
                else
                {
                    Debug.Fail("Illegal move! " + this);
                }
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Move.TAKE3)) return false;
                return ((Move.TAKE3)obj).colors == colors;
            }

            public override int GetHashCode()
            {
                return colors.GetHashCode();
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

                foreach (Card c in b.boardCards)
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
                bool exists = b.boardCards.Contains(card) || b.currentPlayer.reserve.Contains(card);
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
                }
                else
                {
                   Debug.Fail("Illegal move! " + this);
                }
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Move.BUY)) return false;
                return ((Move.BUY)obj).card == card;
            }

            public override int GetHashCode()
            {
                return card.GetHashCode();
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

                foreach (Card c in b.boardCards)
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
                bool isAvail = b.boardCards.Contains(card) && card.deck != Splendor.nobles;
                return isAvail;
            }

            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            public override void takeAction()
            {
                if (isLegal())
                {
                    Splendor.currentPlayer.Reserve(card);
                }
                else
                {
                    Debug.Fail("Illegal move! " + this);
                }
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != typeof(Move.RESERVE)) return false;
                return ((Move.RESERVE)obj).card == card;
            }

            public override int GetHashCode()
            {
                return card.GetHashCode();
            }

        }

    }

}
