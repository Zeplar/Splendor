using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
namespace Splendor
{
    class Program
    {
        static Queue<string> commands = new Queue<string>();
        static int ties = 0;
        static int p1Wins = 0;
        static int stalemates = 0;
        static List<Player> PLAYERS = new List<Player>();
        static List<Command> games = new List<Command>();

        static void getStats()
        {
            bool tied, stalemated;
            Player winner = GameController.getMaxPlayer(out tied, out stalemated);
            if (stalemated) stalemates++;
            else if (tied) ties++;
            else if (winner == GameController.players[0]) p1Wins++;
            recordScore();
        }

        private struct Command
        {
            public int repeats;
            public List<Player> players;
            bool record;

            public Command(List<Player> players, int repeats, bool record)
            {
                this.players = new List<Player>(players);
                this.repeats = repeats;
                this.record = record;
            }

            public void run()
            {
                GameController.Start(players);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int j = 0; j < repeats; j++)
                {
                    RecordHistory.current = new RecordHistory(record);
                    if (j > 0)
                    {
                        double minutes = (watch.Elapsed.TotalMinutes / j) * (repeats - j);
                        CONSOLE.Overwrite(7, "Repeat " + j + "/" + repeats + ", ETA " + minutes.ToString().Substring(0, 4) + " minutes");
                    }

                    GameController.replayGame();
                    getStats();
                }
                watch.Stop();
                CONSOLE.Overwrite(10, "P1 wins : " + p1Wins + "     Ties: " + ties + "      Stalemates: " + stalemates);
            }
        }

        static void dequeue()
        {
            int i;
            bool record;
            switch (commands.Dequeue())
            {
                case "repeat":
                    Console.WriteLine("Repeat.");
                   if (PLAYERS.Count < 2)
                    {
                        Console.WriteLine("Error: Not enough players.");
                        return;
                    }
                    i = int.Parse(commands.Dequeue());
                    if (commands.Count > 0)
                    {
                        record = true;
                        commands.Dequeue();
                    }
                    else record = false;
                    CONSOLE.Overwrite(5, "Adding game: " + i + record);
                    games.Add(new Command(PLAYERS, i, record));
                    PLAYERS.Clear();
                    return;

                case "reset":
                    games.Clear();
                    PLAYERS.Clear();
                    p1Wins = 0;
                    Console.Clear();
                    Console.Write("Reset\n");
                    return;
                case "games":
                    foreach (Command c in games) Console.WriteLine(c.players[0] + " vs " + c.players[1] + " for " + c.repeats + " games.");
                    return;
                case "remove":
                    i = int.Parse(commands.Dequeue());
                    if (i < games.Count) games.RemoveAt(i);
                    else Console.WriteLine("There are not " + (i + 1) + " games in the queue.");
                    return;

                case "run":
                    foreach (Command c in games)
                    {
                        CONSOLE.Overwrite(5, "Running a game.");
                        c.run();
                    }
                    return;

                case "score":
                    recordScore();
                    return;
                //case "debug":
                //    debugGame();
                //    recordScore();
                //    return;

                case "clear":
                    Console.Clear();
                    return;

                //case "runseed":
                //    i = int.Parse(commands.Dequeue());
                //    if (i % 2 == 1) GameController.players = new Player[2] { GameController.players[1], GameController.players[0] };
                //    GameController.__random__ = new AForge.ThreadSafeRandom(i);
                //    GameController.replayGame();
                //    return;

                default:
                    return;
            }
        }

        static void recordScore()
        {
            int i = 5;
            foreach (Command c in games)
            {
                CONSOLE.Overwrite(i, new string(' ', Console.WindowWidth - 1));
                StringBuilder s = new StringBuilder();
                for (int j = 0; j < c.players.Count; j++)
                    s.AppendFormat("{0}: {1} --- ", c.players[j], c.players[j].Wins);
                CONSOLE.Overwrite(i, s.ToString());
                i++;
            }
        }

        static void debugGame()
        {
            GameController.replayGame();

        }

        //static void findDiscrepancy(int tries)
        //{
        //    Greedy g1 = new Greedy();
        //    Greedy g2 = new Greedy();
        //    Greedy g3 = new Greedy();
        //    Minimax m = new Minimax(1, Heuristic.Lead + Heuristic.WinLoss);
        //    int[] winArray = new int[tries];

        //   GameController.Start(g1, g2, 100);
        //    for (int i = 0; i < tries; i++)
        //    {
        //        GameController.replayGame();
        //        Console.Write("" + i);
        //        Console.CursorLeft = 0;
        //        winArray[i] = g1.Wins;
        //    }
        //    GameController.Start(g3, m, 100);
        //    for (int i = 0; i < tries; i++)
        //    {
        //        GameController.replayGame();
        //        Console.Write("" + i);
        //        Console.CursorLeft = 0;
        //        if (winArray[i] != g3.Wins) throw new Exception("Games diverged at i= " + i);
        //    }
        //}

        static bool save(string[] s)
        {
            if (s[0] == "save")
            {
                try
                {
                    Heuristic fn = Heuristic.parse(s, 2);
                    Heuristic.Save(s[1], fn);
                }
                catch
                {
                    Console.WriteLine("Error.");
                    return false;
                }
                return true;
            }
            return false;
        }

        static bool makePlayer(string[] s)
        {
            if (PlayerFactory.Exists(s[0]))
            {
                Player temp = null;
                try
                {
                    temp = PlayerFactory.CreateNew(s);
                    if (PLAYERS.Count < 4) PLAYERS.Add(temp);
                    return true;
                }
                catch (FormatException z)
                {
                    Console.WriteLine(z.Message);
                    return false;
                }
            }
            Console.WriteLine("That player type does not exist.");
            return false;
        }

        static void help(string[] s)
        {
            if (Heuristic.dictionary.ContainsKey(s[0])) CONSOLE.WriteLine(Heuristic.dictionary[s[0]].ToString());
        }

        /// <summary>
        /// Sets up the game through the online client
        /// </summary>
        static void SetupGame()
        {
            SynchronousSocketListener.StartListening();

            List<Player> players = SynchronousSocketListener.parseStartGame();
            Console.WriteLine("Players: " + players.String());
            games.Add(new Command(players, 1, true));
            SynchronousSocketListener.Reply(true);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Configuration: Selfish depth 3, track.txt");
            PlayerFactory.Load();
            Heuristic.register();
            while (true)
            try {
                SetupGame();
                Console.WriteLine("Starting game.");
                games[0].run();
                SynchronousSocketListener.StopListening();
            } catch
            {
                    SynchronousSocketListener.StopListening();
                    continue;
            }
            


            //Console.WriteLine("Available players: " + PlayerFactory.listAll);
            //Console.WriteLine("Scoring functions: " + Heuristic.listAll);
            //while (true)
            //{
            //    string[] s = Console.ReadLine().Replace("(", "( ").Replace(")", " )").Replace("+", " + ").Replace("-"," - ").Replace("*"," * ").Replace("/"," / ").Split();
            //    help(s);
            //    if (makePlayer(s)) { }
            //    else if (save(s)) { }
            //    else
            //    {
            //        foreach (string x in s)
            //            commands.Enqueue(x);
            //        while (commands.Count > 0) dequeue();
            //    }
            //}
        }
    }
}
