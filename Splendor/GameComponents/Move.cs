using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Splendor
{

    public abstract class Move
    {

        /// <summary>
        /// TAKE2, TAKE3, BUY, RESERVE
        /// </summary>
        public Type moveType;
        private static Dictionary<int, Move> dictIntMove;
        private static Dictionary<Move, int> dictMoveInt;

        public enum Type { TAKE2, TAKE3, BUY, RESERVE };

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

        public static Move getMove(int i)
        {
            return dictIntMove[i];
        }

        public static int getInt(Move m)
        {
            return dictMoveInt[m];
        }



        /// <summary>
        /// Decodes a 2-7 byte array, returning a move. The first byte signals the move type.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Move Decode(byte[] array)
        {
            IEnumerable<byte> temp = array.AsEnumerable().TakeWhile(x => x != 255);
            int moveType = temp.First();
            temp = temp.Skip(1);
            switch (moveType)
            {
                case 0:
                    return new TAKE2(new Gem(temp));
                case 1:
                    return new TAKE3(new Gem(temp));
                case 2:
                    return new BUY(Card.allCardsByID[temp.First()]);
                case 3:
                    return new BUY(Card.allCardsByID[temp.First()]);
                default:
                    throw new KeyNotFoundException("Bad decode data");
            }
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
            return (m.Count > 0) ? m[GameController.random.Next(m.Count)] : null;
        }

        public class TAKE2 : Move
        {

            public Gem color;
            public int index;

            /// <summary>
            /// TAKE2s that are normally available
            /// </summary>
            private static List<TAKE2> allReg;
            /// <summary>
            /// TAKE2s that become available at 9 or 10 gems
            /// </summary>
            private static List<TAKE2> allCap;

            public TAKE2(int i)
            {
                color = new Gem();
                color[i] = 2;
                index = i;
                moveType = Type.TAKE2;
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
                moveType = Type.TAKE2;
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
                    GameController.currentPlayer.gems += color;
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


            public static List<TAKE2> reg
            {
                get
                {
                    if (allReg == null) generate();
                    return allReg;
                }
            }
            public static List<TAKE2> cap
            {
                get
                {
                    if (allCap == null) generate();
                    return allCap;
                }
            }

            public static void generate()
            {
                allReg = new List<TAKE2>();
                for (int i = 0; i < 5; i++)
                {
                    allReg.Add(new TAKE2(i));
                }
                allCap = new List<TAKE2>();
                foreach (Gem g in Gem.ExchangeTwo)
                {
                    allCap.Add(new TAKE2(g));
                }
            }

            
            public static List<TAKE2> getLegalMoves(Board b)
            {
                return (b.currentPlayer.gems.magnitude < 9) ? reg.FindAll(x => x.isLegal(b)) : cap.FindAll(x => x.isLegal(b));
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
            private static List<TAKE3> allCap;
            private static List<TAKE3> allReg;

            public TAKE3(Gem x)
            {

                moveType = Type.TAKE3;
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

            public static void generate()
            {
               allReg = new List<TAKE3>();
                foreach (Gem g in Gem.ThreeNetThree)
                {
                    allReg.Add(new TAKE3(g));
                }
                allCap = new List<TAKE3>();
                foreach (Gem g in Gem.ExchangeThree)
                {
                    allCap.Add(new TAKE3(g));
                }
            }

            public static List<TAKE3> reg { get
                {
                    if (allReg == null) generate();
                    return allReg;
                }
            }
            public static List<TAKE3> cap { get
                {
                    if (allCap == null) generate();
                    return allCap;
                }
            }

            public static List<TAKE3> getLegalMoves(Board b)
            {
                return (b.currentPlayer.gems.magnitude < 8) ? reg.FindAll(x => x.isLegal(b)) : cap.FindAll(x => x.isLegal(b));
            }

            public override bool isLegal(Board b)
            {
                Gem after = b.currentPlayer.gems + colors;
                bool avail = b.gems > colors;
                bool haveRoom = after.magnitude <= 10 && after.deficit == 0;
                return avail && haveRoom && (colors[5] == 0);
            }

            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            public override void takeAction()
            {
                if (isLegal())
                {
                    GameController.currentPlayer.takeGems(colors);
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
            private static List<BUY> all;

            public BUY(Card c)
            {
                this.card = c;
                moveType = Type.BUY;
            }

            public override string ToString()
            {
                return "BUY " + card.id;
            }

            public static void generate()
            {
                List<BUY> l = new List<BUY>();
                foreach (Card c in Card.allCardsByID)
                {
                    l.Add(new BUY(c));
                }
                all = l;
            }

            public static List<BUY> allBUY { get { return all; } }

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
                exists &= (card.deck != GameController.nobles);
                bool affordable = (b.currentPlayer.gems + b.currentPlayer.discount - card.cost).deficit <= b.currentPlayer.gems[5];
                return exists && affordable;
            }

            public override bool isLegal()
            {
                return isLegal(Board.current);
            }

            public override void takeAction()
            {
                Debug.Assert(Board.current.currentPlayer == GameController.currentPlayer, "Board is misrepresenting players: " + Board.current.currentPlayer.detailedInfo + " != " + GameController.currentPlayer.detailedInfo);

                if (isLegal())
                {
                    GameController.currentPlayer.Buy(card);
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
            private static List<RESERVE> all;

            public RESERVE(Card c)
            {
                this.card = c;
                moveType = Type.RESERVE;
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
                    if (c.id < 90)
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

            public static void generate()
            {
                List<RESERVE> l = new List<RESERVE>();
                foreach (Card c in Card.allCardsByID)
                {
                    l.Add(new RESERVE(c));
                }
                all = l;
            }

            public static List<RESERVE> allRESERVE { get { return all; } }

            public override bool isLegal(Board b)
            {

                bool canReserve = b.currentPlayer.reserve.Count < 3;
                if (!canReserve)
                {
                    return false;
                }
                bool isAvail = b.boardCards.Contains(card) && card.deck != GameController.nobles;
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
                    GameController.currentPlayer.Reserve(card);
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
