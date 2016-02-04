using System.Collections.Generic;
using System.IO;
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
        public static Player[] players;
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
                return players[turn % 2];
            }
        }

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
            Gem.Reset();
            //RecordHistory.record((currentPlayer + " has points: " + currentPlayer.points + " and gems: " + currentPlayer.gems + " and field: " + currentPlayer.field.String()));
            //RecordHistory.record("Board: " + Board.current.boardCards.String());
            currentPlayer.takeTurn();
            //RecordHistory.record(Environment.NewLine);
            turn += 1;
            gameOver = Board.current.gameOver;
        }

        //Stop the AIs and close the recording tool.
        static void endGame()
        {
            gameOver = true;
            if (Board.current.winner == 0) currentPlayer.wins++;
            if (Board.current.winner == 1) players[turn % 2 + 1].wins++;
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
            }
            players = new Player[2] { players[1], players[0] };
            turn = 0;
            Card.getCards();
            Gem.board = new Gem(4, 4, 4, 4, 4, 8);
            RecordHistory.initialize ();
            gameOver = false;
            takingTurn = false;
            while (!gameOver)
            {
                nextTurn();
            }
            endGame();
        }

        //Returns the player with the highest score, with ties broken by least gem mines.
        public static Player getMaxPlayer(out bool tied, out bool stalemated)
        {
            tied = false;
            stalemated = Move.getRandomMove() == null;
            if (players[0].points > players[1].points)
            {
                return players[0];
            }
            else if (players[1].points > players[0].points)
            {
                return players[1];
            }
            else if (players[0].field.Count > players[1].field.Count)
            {
                return players[1];
            }
            else if (players[1].field.Count > players[0].field.Count)
            {
                return players[0];
            }
            else
                tied = true;
                return players[0];
        }

        //Initialize the cards, players, and recording tool. Then start the game.
        public static void Start(Player p1, Player p2, int randomSeed)
        {
            Console.WriteLine("Initializing game with " + p1 + " and " + p2);
            decks = new Deck[3] { new Deck(), new Deck(), new Deck() };
            nobles = new Deck();
            players = new Player[2] { p1, p2 };
            random = new ThreadSafeRandom(randomSeed);
            p1.turnOrder = 0;
            p2.turnOrder = 1;
            Card.getCards();
            gameOver = false;
            timer = Stopwatch.StartNew();
        }

        public static void Start(Player p1, Player p2)
        {
            Start(p1, p2, new Random().Next());
        }

    }

}
