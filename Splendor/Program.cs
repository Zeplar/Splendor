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
        static List<Player> tempPlayers;

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
                    for (int j=0; j < i; j++)
                    {
                        setPlot();
                        if (j > 0)
                        {
                            double minutes = (watch.Elapsed.TotalMinutes / j) * (i - j);
                            CONSOLE.Overwrite(7, "Repeat " + j + "/" + i + ", ETA " + minutes.ToString().Substring(0,4) + " minutes");
                        }
                        //GameController.__random__ = new AForge.ThreadSafeRandom(j);
                        //CONSOLE.Overwrite(12, "Seed: " + j);
                        GameController.replayGame();
                        getStats();
                    }
                    watch.Stop();
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
                    CONSOLE.WriteLine("Recording = " + GameController.recording);
                    return null;
                case "plot":
                    RecordHistory.plotting = !RecordHistory.plotting;
                    Console.WriteLine("Plotting = " + RecordHistory.plotting);
                    return null;
                case "runseed":
                    i = int.Parse(commands.Dequeue());
                    if (i % 2 == 1) GameController.players = new Player[2] { GameController.players[1], GameController.players[0] };
                    GameController.__random__ = new AForge.ThreadSafeRandom(i);
                    GameController.replayGame();
                    return null;
                case "description":
                    RecordHistory.plot(new List<string>(commands).String() + Environment.NewLine);
                    commands.Clear();
                    return null;
                default:
                    return null;
            }
        }

        static void setPlot()
        {
            RecordHistory.clearPlot();
            RecordHistory.plot(PLAYERS[0] + "    " + PLAYERS[1] + Environment.NewLine);
            RecordHistory.plot(Environment.NewLine);
            RecordHistory.snapshot(new string[] { PLAYERS[0] + "    " + PLAYERS[1], " "});
        }

        static void recordScore()
        {
            CONSOLE.Overwrite(8, new string(' ', Console.WindowWidth - 1));
            CONSOLE.Overwrite(8, PLAYERS[0] + ": " + PLAYERS[0].Wins + " --- " + PLAYERS[1] + ": " + PLAYERS[1].Wins);
        }

        static void debugGame()
        {
            GameController.replayGame();

        }

        static void findDiscrepancy(int tries)
        {
            Greedy g1 = new Greedy();
            Greedy g2 = new Greedy();
            Greedy g3 = new Greedy();
            Minimax m = new Minimax(1, ScoringMethods.Lead + ScoringMethods.WinLoss);
            int[] winArray = new int[tries];

           GameController.Start(g1, g2, 100);
            for (int i = 0; i < tries; i++)
            {
                GameController.replayGame();
                Console.Write("" + i);
                Console.CursorLeft = 0;
                winArray[i] = g1.Wins;
            }
            GameController.Start(g3, m, 100);
            for (int i = 0; i < tries; i++)
            {
                GameController.replayGame();
                Console.Write("" + i);
                Console.CursorLeft = 0;
                if (winArray[i] != g3.Wins) throw new Exception("Games diverged at i= " + i);
            }
        }

        static bool save(string[] s)
        {
            if (s[0] == "save")
            {
                try
                {
                    ScoringMethods.Function fn = ScoringMethods.parse(s, 2);
                    ScoringMethods.Save(s[1], fn);
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
            try {
                if (PlayerFactory.Exists(s[0]))
                {
                    Player temp = null;
                    try
                    {
                        temp = PlayerFactory.CreateNew(s);
                    }
                    catch (FormatException z)
                    {
                        Console.WriteLine(z.Message);
                    }
                    finally
                    {
                        if (temp != null) tempPlayers.Add(temp);
                    }
                    if (tempPlayers.Count == 2)
                    {
                        GameController.Start(tempPlayers[0], tempPlayers[1]);
                        PLAYERS = tempPlayers.GetRange(0, 2);
                        tempPlayers.Clear();
                    }
                    return true;
                }
                return false;
            } catch { Console.WriteLine("Error."); return false; }
        }

        static void help(string[] s)
        {
            if (ScoringMethods.dictionary.ContainsKey(s[0])) CONSOLE.WriteLine(ScoringMethods.dictionary[s[0]].ToString());
        }


        static void Main(string[] args)
        {
            //findDiscrepancy(500);
            tempPlayers = new List<Player>();
            PlayerFactory.Load();
            ScoringMethods.register();
            Console.WriteLine("Available players: " + PlayerFactory.listAll);
            Console.WriteLine("Scoring functions: " + ScoringMethods.listAll);
            while (true)
            {
                string[] s = Console.ReadLine().Replace("(", "( ").Replace(")", " )").Replace("+", " + ").Replace("-"," - ").Replace("*"," * ").Replace("/"," / ").Split();
                help(s);
                if (makePlayer(s)) { }
                else if (save(s)) { }
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
