using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using AForge;

namespace Splendor
{

    public static class GameController
    {

        public static Deck[] decks;
        public static Deck nobles;
        private static int t;
        public static int turn
        {
            get { return t; }
            private set { t = value; }
        }
        public static List<Player> players;
        public static Card selected;
        public static bool gameOver = true;
        public static bool takingTurn;
        public static Stopwatch timer;
        public static bool recording = false;

        internal static ThreadSafeRandom random;
        public static ThreadSafeRandom Random { get { return random; } }
        public static ThreadSafeRandom __random__ { get { return random; } set { random = value; } }

        public static Player currentPlayer
        {
            get
            {
                return players[turn % players.Count];
            }
        }

        /// <summary>
        /// Cards and nobles currently on the board
        /// </summary>
        public static List<Card> boardCards
        {
            get
            {
                List<Card> l = new List<Card>();
                foreach (Deck d in decks)
                {
                    l.AddRange(d.viewableCards);
                }
                l.AddRange(nobles.viewableCards);
                return l;
            }
        }

   
        //Signal the current AI to stop, the turn to increment, and the scores to be checked.
        public static void nextTurn()
        {
            //RecordHistory.record((currentPlayer + " has points: " + currentPlayer.points + " and gems: " + currentPlayer.gems + " and field: " + currentPlayer.field.String()));
            //RecordHistory.record("Board: " + Board.current.boardCards.String());
            currentPlayer.takeTurn();
            //RecordHistory.record(Environment.NewLine);
            turn += 1;
            Gem.Reset();
            Board.current = new Board();
            UnitTests.testBoard();
            gameOver = Board.current.gameOver;
        }

        //Stop the AIs and close the recording tool.
        static void endGame()
        {
            gameOver = true;
            bool tied, stalemate;
            Player winner = getMaxPlayer(out tied, out stalemate);
            if (!tied && !stalemate) winner.wins++;
            foreach (Player p in players)      p.GameOver();
        }

        public static void replayGame()
        {
            foreach (Deck d in decks)
            {
                d.getAllCards().Clear();
            }
            nobles.getAllCards().Clear();
            foreach (Player p in players)
            {
                p.field.Clear();
                p.reserve.Clear();
                p.gems = new Gem();
                p.movesTaken = new int[4];
            }
            players.shuffle();
            turn = 0;
            Card.getCards();
            Gem.board = Gem.StartingGems;
            gameOver = false;
            takingTurn = false;
            Board.current = new Board();
            //UnitTests.testBoard();
            while (!gameOver) nextTurn();
            endGame();
        }

        //Returns the player with the highest score, with ties broken by least gem mines.
        public static Player getMaxPlayer(out bool tied, out bool stalemated)
        {
            tied = false;
            stalemated = Move.getRandomMove() == null;

            IEnumerable<Player> PlayersByMaxPoints = players.OrderByDescending(p => p.points).ThenBy(p => p.field.Count);
            Player first = PlayersByMaxPoints.First();
            PlayersByMaxPoints = PlayersByMaxPoints.TakeWhile(p => p.points == first.points && p.field.Count == first.field.Count);

            if (PlayersByMaxPoints.Count() == 1)
            {
                return PlayersByMaxPoints.First();
            }
            else
            {
                tied = true;
                return PlayersByMaxPoints.First();
            }
        }

        //Initialize the cards, players, and recording tool. Then start the game.
        public static void Start(List<Player> Players, int randomSeed)
        {
            Console.WriteLine("Initializing game with " + Players.String());
            decks = new Deck[3] { new Deck(), new Deck(), new Deck() };
            nobles = new Deck();
            players = new List<Player>(Players);
            random = new ThreadSafeRandom(randomSeed);
            gameOver = false;
            timer = Stopwatch.StartNew();
            switch (players.Count)
            {
                case 2:
                    Gem.StartingGems = new Gem(4, 4, 4, 4, 4, 5);
                    break;
                case 3:
                    Gem.StartingGems = new Gem(5, 5, 5, 5, 5, 5);
                    break;
                case 4:
                    Gem.StartingGems = new Gem(7, 7, 7, 7, 7, 5);
                    break;
                default:
                    throw new Exception("Bad number of players: " + players.Count);
            }
        }

        public static void Start(List<Player> Players)
        {
            Start(Players, new Random().Next());
        }

    }

}
