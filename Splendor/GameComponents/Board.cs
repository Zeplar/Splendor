using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Splendor
{

public class Board
    {

        public List<Player> players;
        public Gem gems;
        public List<Card> boardCards;
        public int turn;
        /// <summary>
        /// The board that generated this board.
        /// </summary>
        public Board prevBoard;
        /// <summary>
        /// The move that generated this board.
        /// </summary>
        public Move prevMove;

        private List<Move> moves;
        private bool gameOverValue;
        private bool GameOverFlag = false;

        public List<Card> viewableCards {
            get {
                List<Card> ret = new List<Card>();
                ret.AddRange(boardCards);
                ret.AddRange(currentPlayer.reserve);
                return ret;
            }
        }

        /// <summary>
        /// Creates a deep copy of the given board. Turn indicates the current player as well as the "starting" player.
        /// </summary>
        public Board(List<Player> players, int turn, List<Card> cards, Gem gems)
        {
            this.players = new List<Player>();
            foreach (Player p in players)
            {
                this.players.Add(copyPlayer(p));
            }
            boardCards = cards.ToList();
            this.gems = gems;
            this.turn = turn;
            this.prevBoard = null;
            this.prevMove = null;
        }

        private Board()
        {
            this.players = GameController.players.ToList();
            if (this.players[0] != GameController.currentPlayer)
            {
                this.players.Reverse();
            }
            this.gems = Gem.board;
            this.turn = 0;
            this.boardCards = GameController.boardCards;
            this.prevBoard = null;
            this.prevMove = null;
        }

        private int EncodeHelper(byte[] array, int index, List<Card> list)
        {
            int j = index;
            for (int i=0; i < list.Count; i++)
            {
                array[j] = (byte)list[i].id;
                j++;
            }
            array[j] = 255;
            j++;
            return j;
        }

        public byte[] Encode()
        {
            byte[] ret = new byte[1024];
            for (int i = 0; i < 6; i++) ret[i] = (byte)currentPlayer.gems[i];
            for (int i = 6; i < 12; i++) ret[i] = (byte)notCurrentPlayer.gems[i - 6];
            for (int i = 12; i < 18; i++) ret[i] = (byte)gems[i - 12];
            ret[18] = 255;
            int j = EncodeHelper(ret, 19, boardCards);
            j = EncodeHelper(ret, j, currentPlayer.field);
            j = EncodeHelper(ret, j, currentPlayer.reserve);
            j = EncodeHelper(ret, j, notCurrentPlayer.field);
            j = EncodeHelper(ret, j, notCurrentPlayer.reserve);
            return ret;
        }
        
        private static List<Card> DecodeSection(byte[] array, int section)
        {
            IEnumerable<byte> temp = array.AsEnumerable();
            for (int i=0; i < section; i++)
            {
                temp = temp.SkipWhile(x => x != 255);
                temp = temp.Skip(1);
            }
            temp = temp.TakeWhile(x => x != 255);
            return new List<Card>(temp.Select(x => Card.allCardsByID[x]));
        }

        public static Board Decode(byte[] array)
        {
            List<byte> data = new List<byte>(array);
            Gem cGem = new Gem(data.GetRange(0, 6));
            Gem nGem = new Gem(data.GetRange(6, 6));
            Gem bGem = new Gem(data.GetRange(12, 6));
            List<Card> boardCards = DecodeSection(array, 1);
            List<Card> cCards = DecodeSection(array, 2);
            List<Card> cReserve = DecodeSection(array, 3);
            List<Card> nCards = DecodeSection(array, 4);
            List<Card> nReserve = DecodeSection(array, 5);

            Board b = new Board();
            b.boardCards = boardCards;
            b.gems = bGem;
            b.currentPlayer.gems = cGem;
            b.notCurrentPlayer.gems = nGem;
            b.currentPlayer.field = cCards;
            b.currentPlayer.reserve = cReserve;
            b.notCurrentPlayer.field = nCards;
            b.notCurrentPlayer.reserve = nReserve;

            return b;
        }


        /// <summary>
        /// Returns the current gamestate as a Board   	
        /// </summary>
        public static Board current
        {
            get
            {
                return new Board();
            }
        }

        /// <summary>
        /// Returns True if it's Player 1's turn and the game is over (ie the game ended in the previous round).
        /// </summary>
        public bool gameOver
        {
            get
            {
                if (GameOverFlag) return gameOverValue;
                gameOverValue = legalMoves.Count == 0 || (currentPlayer.turnOrder == 0 && (maximizingPlayer.points >= 15 || minimizingPlayer.points >= 15));
                GameOverFlag = true;
                return gameOverValue;
            }
        }

        /// <summary>
        /// Returns a list of legal moves for the board.	
        /// </summary>
        public List<Move> legalMoves
        {
            get
            {
                if (moves == null) moves = Move.getAllLegalMoves(this);
                return moves;
            }
        }

        /// <summary>
        /// Returns the player currently taking their turn.	
        /// </summary>
        public Player currentPlayer
        {
            get
            {
                return players[turn % players.Count];
            }
        }

        public Player notCurrentPlayer
        {
            get
            {
                return players[(turn + 1) % players.Count];
            }
        }

        /// <summary>
        /// Returns the player who was active when this board was generated.
        /// </summary>
        public Player maximizingPlayer
        {
            get
            {
                return players[0];
            }
        }

        public Player minimizingPlayer
        {
            get
            {
                return players[1];
            }
        }

        public Board generate(Move m)
        {
            Debug.Assert(m.isLegal(this), "Illegal generating move!");
            Board b = new Board(players, turn, boardCards, gems);
            b.gems = gems;
            Player p = b.currentPlayer;
            b.prevMove = m;
            b.prevBoard = this;

            switch (m.moveType)
            {
                case Move.Type.TAKE2:
                    Move.TAKE2 x = (Move.TAKE2)m;
                    b.gems -= x.color;
                    p.gems += x.color;
                    break;
                case Move.Type.TAKE3:
                    Move.TAKE3 y = (Move.TAKE3)m;
                    b.gems -= y.colors;
                    p.gems += y.colors;
                    break;
                case Move.Type.BUY:
                    Move.BUY z = (Move.BUY)m;
                    Gem temp = p.gems.takeaway((z.card.cost - p.discount));
                    b.gems += (p.gems - temp);
                    p.gems = temp;
                    p.field.Add(z.card);
                    b.boardCards.Remove(z.card);
                    p.reserve.Remove(z.card);
                    Card noble;
                    if (p.canGetNoble(out noble))
                    {
                        p.field.Add(noble);
                        b.boardCards.Remove(noble);
                    }
                    break;
                case Move.Type.RESERVE:
                    Move.RESERVE w = (Move.RESERVE)m;
                    p.gems[5] += 1;
                    b.boardCards.Remove(w.card);
                    p.reserve.Add(w.card);
                    break;
                default:
                    Debug.Fail("Invalid simulated move");
                    return b;
            }
            b.turn += 1;
            return b;
        }

        Player copyPlayer(Player p)
        {
            Player x = new Player();
            x.gems = p.gems;
            x.field = new List<Card>();
            x.field.AddRange(p.field);
            x.reserve = new List<Card>();
            x.reserve.AddRange(p.reserve);
            x.turnOrder = p.turnOrder;
            return x;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (Board))
            {
                return false;
            }
            Board other = (Board)obj;
            bool same = this.boardCards.addUp() == other.boardCards.addUp();
            same &= this.turn == other.turn;
            same &= this.gems == other.gems;
            same &= this.currentPlayer.field.addUp() == other.currentPlayer.field.addUp();
            same &= this.currentPlayer.gems.Equals(other.currentPlayer.gems);
            same &= this.notCurrentPlayer.field.addUp() == this.notCurrentPlayer.field.addUp();
            return same;
        }

        public override int GetHashCode()
        {
            int i = 0;
            foreach (Card c in boardCards)
            {
                i += c.id;
            }
            i *= gems.GetHashCode();
            return i;
        }

    }

}
