using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Splendor
{

public class Board
    {

        public List<Player> players;
        public Gem gems;
        public List<Card> viewableCards;
        public int turn;
        public int numPlayers;

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
            viewableCards = cards.ToList();
            this.gems = gems;
            this.turn = turn;
            this.numPlayers = players.Count;
        }

        private Board()
        {
            this.players = Splendor.players.ToList();
            int i = Splendor.currentPlayer.turnOrder;
            players = players.Skip(i).Concat(players.Take(i)).ToList();
            this.gems = Gem.board;
            this.turn = 0;
            this.numPlayers = players.Count;
            this.viewableCards = Splendor.viewableCards;
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
                return players[turn % numPlayers];
            }
        }

        public Player notCurrentPlayer
        {
            get
            {
                return players[(turn + 1) % numPlayers];
            }
        }

        /// <summary>
        /// Returns the player who was active when this board was generated.
        /// </summary>
        public Player startingPlayer
        {
            get
            {
                return players[0];
            }
        }

        public Player notStartingPlayer
        {
            get
            {
                return players[1];
            }
        }

        public bool canGetNoble(out Card noble)
        {
            foreach (Card c in viewableCards.FindAll(x => x.deck == Splendor.nobles))
            {
                if (currentPlayer.discount > c.cost)
                {
                    noble = c;
                    return true;
                }
            }
            noble = new Card();
            return false;
        }

        public Board generate(Move m)
        {

            Board b = new Board(players, turn, viewableCards, gems);
            b.gems = gems;
            Player p = b.currentPlayer;

            switch (m.moveType)
            {
                case 0:
                    Move.TAKE2 x = (Move.TAKE2)m;
                    b.gems[x.color] -= 2;
                    p.gems[x.color] += 2;
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
                    b.viewableCards.Remove(z.card);
                    p.reserve.Remove(z.card);
                    Card noble;
                    if (canGetNoble(out noble))
                    {
                        p.field.Add(noble);
                        b.viewableCards.Remove(noble);
                    }
                    break;
                case 3:
                    Move.RESERVE w = (Move.RESERVE)m;
                    p.gems[5] += 1;
                    b.viewableCards.Remove(w.card);
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
            Greedy x = new Greedy();
            x.gems = p.gems;
            x.field = new List<Card>();
            x.field.AddRange(p.field);
            x.reserve = new List<Card>();
            x.reserve.AddRange(p.reserve);
            return x;
        }



    }

}
