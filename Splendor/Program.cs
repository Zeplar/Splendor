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
        static List<Player> PLAYERS;

        static void getStats()
        {
            bool tied, stalemated;
            Player winner = GameController.getMaxPlayer(out tied, out stalemated);
            if (stalemated) stalemates++;
            else if (tied) ties++;
            else if (winner == GameController.players[0]) p1Wins++;
            recordScore();
        }

        static object dequeue()
        {
            int i;
            switch (commands.Dequeue())
            {
                case "repeat":
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    i = int.Parse(commands.Dequeue());
                    for (; i > 0; i--)
                    {
                        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                        Console.Write("Repeat " + i);
                        GameController.replayGame();
                        getStats();
                       
                    }
                    watch.Stop();
                    Console.WriteLine("" + watch.Elapsed);
                    Console.WriteLine("P1 wins : " + p1Wins + "     Ties: " + ties + "      Stalemates: " + stalemates);
                    return null;
                case "score":
                    recordScore();
                    return null;
                case "debug":
                    debugGame();
                    recordScore();
                    return null;

                case "clear":
                    Console.Clear();
                    return null;
                case "record":
                    GameController.recording = !GameController.recording;
                    return null;
                case "runseed":
                    GameController.Start(PLAYERS[0], PLAYERS[1], 100);
                    return null;
                case "description":
                    RecordHistory.plot(new List<string>(commands).String() + Environment.NewLine);
                    commands.Clear();
                    return null;
                default:
                    return null;
            }
        }

        static void recordScore()
        {
            int i = Console.CursorTop;
            Console.SetCursorPosition(0, 8);
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
            Console.Write(PLAYERS[0] + ": " + PLAYERS[0].wins + " --- " + PLAYERS[1] + ": " + PLAYERS[1].wins);
            Console.SetCursorPosition(0, i);

        }

        static void debugGame()
        {
            Console.WriteLine("Go");
            GameController.replayGame();

        }

        static void findDiscrepancy(int tries)
        {
            Greedy g1 = new Greedy();
            Greedy g2 = new Greedy();
            Greedy g3 = new Greedy();
            Minimax m = new Minimax(1, ScoringMethods.combine(ScoringMethods.DeltaPoints, ScoringMethods.WinLoss));
            int[] winArray = new int[tries];

           GameController.Start(g1, g2, 100);
            for (int i = 0; i < tries; i++)
            {
                GameController.replayGame();
                Console.Write("" + i);
                Console.CursorLeft = 0;
                winArray[i] = g1.wins;
            }
            GameController.Start(g3, m, 100);
            for (int i = 0; i < tries; i++)
            {
                GameController.replayGame();
                Console.Write("" + i);
                Console.CursorLeft = 0;
                Debug.Assert(winArray[i] == g3.wins, "Games diverged at i= " + i);
            }
        }


        static void Main(string[] args)
        {
            List<Player> players = new List<Player>();
            PlayerFactory.Load();
            ScoringMethods.register();
            Console.WriteLine("Available players: " + PlayerFactory.listAll);
            Console.WriteLine("Scoring functions: " + ScoringMethods.listAll);
            while (true)
            {
                string[] s = Console.ReadLine().Replace('(', ' ').Replace(')', ' ').Replace(',', ' ').Split();
                if (PlayerFactory.Exists(s[0]))
                {
                    var parameters = new List<string>(s);
                    parameters.RemoveAt(0);
                    players.Add(PlayerFactory.CreateNew(s[0], parameters));
                    if (players.Count == 2)
                    {
                        GameController.Start(players[0], players[1]);
                        PLAYERS = players.GetRange(0, 2);
                        players.Clear();
                    }
                }
                else
                {
                    foreach (string x in s)
                    commands.Enqueue(x);
                }
                if (commands.Count > 0)
                {
                    dequeue();
                }
            }
        }
    }
}
