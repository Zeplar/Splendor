using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Splendor
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
        static int gamesPlayed = 0;
        public static Random random;
        public static Splendor self;
        public static Stopwatch timer;
        public static bool recording = false;


        public static Player currentPlayer
        {
            get
            {
                return players[turn % 2];
            }
        }

        public static List<Card> viewableCards
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
            turn += 1;
            Gem.Reset();
            //gemPiles.updateTexts();
            if (Board.current.gameOver)
            {
                endGame();
                return;

            }
            takingTurn = false;
        }
        //Stop the AIs and close the recording tool.
        static void endGame()
        {
            gameOver = true;
            bool tied;
            Player winner = getMaxPlayer(out tied);
            Player loser = winner;
            foreach (Player p in players)
            {
                if (p != winner)
                    loser = p;
            }
            if (tied)
            {
                RecordHistory.record("Tie game!");
                RecordHistory.close();
                return;
            }
            winner.wins += 1;
            if (gamesPlayed > 0) { 
                Console.Title = ("   Game " + gamesPlayed + " / 100     Average Time: " + (timer.ElapsedMilliseconds / gamesPlayed));
            }
            RecordHistory.record(winner + " won with " + winner.points);
            RecordHistory.record(loser + " lost with " + loser.points);
            RecordHistory.close();
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
                p.turnOrder = (p.turnOrder + 1) % 2;
            }
            players = new Player[2] { players[1], players[0] };
            Splendor.turn = 0;
            self.getCards();
            Gem.board = new Gem(4, 4, 4, 4, 4, 8);
            RecordHistory.initialize ();
            gameOver = false;
            takingTurn = false;
            while (!gameOver)
            {
                Update();
            }
        }

        //Returns the player with the highest score, with ties broken by least gem mines.
        public static Player getMaxPlayer(out bool tied)
        {
            tied = false;
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


        //Load the cards from the .csv file
        void getCards()
        {
            StreamReader file = new StreamReader(File.OpenRead(@"..\..\..\splendor_cards.csv"));
            file.ReadLine();
            int id = 0;
            while (!file.EndOfStream)
            {
                string[] vals = file.ReadLine().Split(',');
                int w, u, b, r, g;
                w = string.IsNullOrEmpty(vals[3]) ? 0 : int.Parse((vals[3]));
                u = string.IsNullOrEmpty(vals[4]) ? 0 : int.Parse((vals[4]));
                g = string.IsNullOrEmpty(vals[5]) ? 0 : int.Parse((vals[5]));
                r = string.IsNullOrEmpty(vals[6]) ? 0 : int.Parse((vals[6]));
                b = string.IsNullOrEmpty(vals[7]) ? 0 : int.Parse((vals[7]));
                Gem cost = new Gem(w, u, b, r, g, 0);
                int tier = int.Parse(vals[0]);
                int points = string.IsNullOrEmpty(vals[2]) ? 0 : int.Parse((vals[2]));
                int color = 0;
                switch (vals[1])
                {
                    case "white":
                        color = 0;
                        break;
                    case "blue":
                        color = 1;
                        break;
                    case "black":
                        color = 2;
                        break;
                    case "red":
                        color = 3;
                        break;
                    case "green":
                        color = 4;
                        break;
                }
                Card c = new Card(id);
                c.color = color;
                c.cost = cost;
                c.points = points;
                id++;

                switch (tier)
                {
                    case 1:
                        c.deck = decks[0];
                        decks[0].getAllCards().Add(c);
                        break;
                    case 2:
                        c.deck = decks[1];
                        decks[1].getAllCards().Add(c);
                        break;
                    case 3:
                        c.deck = decks[2];
                        decks[2].getAllCards().Add(c);
                        break;
                }
            }
            file.Close();
            decks[0].shuffle();
            decks[1].shuffle();
            decks[2].shuffle();
            populate(nobles);

        }

        void populate(Deck nobles)
        {
            StreamReader file = new StreamReader(File.OpenRead(@"..\..\..\splendor_nobles.csv"));
            file.ReadLine();
            int i = -1;
            while (!file.EndOfStream)
            {
                string[] vals = file.ReadLine().Split(',');
                int w, u, b, r, g;
                w = string.IsNullOrEmpty(vals[0]) ? 0 : int.Parse((vals[0]));
                u = string.IsNullOrEmpty(vals[1]) ? 0 : int.Parse((vals[1]));
                g = string.IsNullOrEmpty(vals[2]) ? 0 : int.Parse((vals[2]));
                r = string.IsNullOrEmpty(vals[3]) ? 0 : int.Parse((vals[3]));
                b = string.IsNullOrEmpty(vals[4]) ? 0 : int.Parse((vals[4]));
                Card c = new Card(i);
                c.cost = new Gem(w, u, b, r, g, 0);
                c.points = 3;
                c.deck = nobles;
                nobles.getAllCards().Add(c);
                i--;
            }
            nobles.shuffle();
            nobles.getAllCards().RemoveRange(4, 6);
        }

        //Initialize the cards, players, and recording tool. Then start the game.
        public Splendor(Player p1, Player p2, int randomSeed)
        {
            Console.WriteLine("Initializing game...");
            decks = new Deck[3] { new Deck(), new Deck(), new Deck() };
            nobles = new Deck();
            players = new Player[2] { p1, p2 };
            random = new Random(randomSeed);
            self = this;
            p1.turnOrder = 0;
            p2.turnOrder = 1;
            getCards();
            gameOver = false;
            timer = Stopwatch.StartNew();

        }

        public Splendor(Player p1, Player p2) : this(p1, p2, new Random().Next())
        {

        }

        public static void Update()
        {
            if (!gameOver && !takingTurn)
            {
                RecordHistory.record(currentPlayer + " has " + currentPlayer.points + " points");
                StringWriter s = new StringWriter();
                s.Write(currentPlayer + " has field: ");
                foreach (Card c in currentPlayer.field)
                {
                    s.Write(c.id + ", ");
                }
                RecordHistory.record(s.ToString());
                currentPlayer.takeTurn();
            }         
        }

        public void select(int i)
        {
            Gem.selected[i] += 1;
            Gem.tryTake();
        }
        public void reset()
        {
            Gem.Reset();
        }

    }

}
