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
        }

        private Board()
        {
            this.players = Splendor.players.ToList();
            if (this.players[0] != Splendor.currentPlayer)
            {
                this.players.Reverse();
            }
            this.gems = Gem.board;
            this.turn = 0;
            this.boardCards = Splendor.boardCards;
            
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
                return (Move.getRandomMove(this) == null || (currentPlayer.turnOrder == 0) && (maximizingPlayer.points >= 15 || minimizingPlayer.points >= 15));
            }
        }

        /// <summary>
        /// Returns a list of legal moves for the board.	
        /// </summary>
        public List<Move> legalMoves
        {
            get
            {
                return Move.getAllLegalMoves(this);
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

            switch (m.moveType)
            {
                case 0:
                    Move.TAKE2 x = (Move.TAKE2)m;
                    b.gems -= x.color;
                    p.gems += x.color;
                    break;
                case 1:
                    Move.TAKE3 y = (Move.TAKE3)m;
                    b.gems -= y.colors;
                    p.gems += y.colors;
                    break;
                case 2:
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
                case 3:
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
            bool same = true;
            same = this.boardCards.Zip(other.boardCards, (x, y) => x.id - y.id).Count(x => x != 0) == 0; //Zips the difference of each card ID and counts the number of nonzeroes.
            same &= this.gems.Equals(other.gems);
            same &= this.currentPlayer.field.Zip(other.currentPlayer.field, (x, y) => x.id - y.id).Count(x => x != 0) == 0;
            same &= this.currentPlayer.gems.Equals(other.currentPlayer.gems);
            same &= this.notCurrentPlayer.field.Zip(other.notCurrentPlayer.field, (x, y) => x.id - y.id).Count(x => x != 0) == 0;
            same &= this.notCurrentPlayer.gems.Equals(other.notCurrentPlayer.gems);
            return same;
        }

        public override int GetHashCode()
        {
            int i = 0;
            foreach (Card c in boardCards)
            {
                i += c.id;
            }
            i *= gems.magnitude;
            i *= currentPlayer.GetHashCode() + notCurrentPlayer.GetHashCode();
            return i;
        }

    }

}
