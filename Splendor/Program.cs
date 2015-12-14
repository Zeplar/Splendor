using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Splendor
{
    class Program
    {
        static Queue<string> commands = new Queue<string>();
        const string run = "run", repeat = "repeat", greedy = "greedy", minimax = "minimax";
        static Splendor currentGame;
        static Player p1;
        static Player p2;

        static object dequeue()
        {
            int ties = 0;
            int p1Wins = 0;
            int i;
            switch (commands.Dequeue())
            {
                case run:
                    p1 = dequeue() as Player;
                    p2 = dequeue() as Player;
                    currentGame = new Splendor(p1, p2);
                    return null;
                case repeat:
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    i = int.Parse(commands.Dequeue());
                    for (; i > 0; i--)
                    {
                        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                        Console.Write("Repeat " + i);
                        Splendor.replayGame();
                        bool tied;
                        Player winner = Splendor.getMaxPlayer(out tied);
                        if (tied) ties++;
                        else if (winner == Splendor.players[0]) p1Wins++;
                        recordScore();
                    }
                    watch.Stop();
                    Console.WriteLine("" + watch.Elapsed);
                    Console.WriteLine("P1 wins : " + p1Wins + "     Ties: " + ties);
                    return null;
                case "debug":
                    debugGame();
                    recordScore();
                    return null;
                case greedy:
                    return new Greedy(commands.Dequeue());
                case "gene":
                    if (int.TryParse(commands.Peek(), out i))
                    {
                        return new Genetic.ExactGene(int.Parse(commands.Dequeue()), int.Parse(commands.Dequeue()), int.Parse(commands.Dequeue()));
                    }
                    return new Genetic.ExactGene();
                case "gsharp":
                    return new Genetic.GSharpExactGene(30, 20, 500);

                case "random":
                    return new RandomPlayer();
                case minimax:
                    i = int.Parse(commands.Dequeue());
                    return new Minimax(i);
                case "clear":
                    Console.Clear();
                    return null;
                case "record":
                    Splendor.recording = !Splendor.recording;
                    return null;
                case "runseed":
                    //    i = int.Parse(commands.Dequeue());
                    p1 = dequeue() as Player;
                    p2 = dequeue() as Player;
                    currentGame = new Splendor(p1, p2, 3);
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
            Console.Write(p1 + ": " + p1.wins + " --- " + p2 + ": " + p2.wins);
            Console.SetCursorPosition(0, i);

        }

        static void debugGame()
        {
            Console.WriteLine("Go");
            Splendor.replayGame();

        }

        static void findDiscrepancy(int tries)
        {
            Greedy g1 = new Greedy();
            Greedy g2 = new Greedy();
            Greedy g3 = new Greedy();
            Minimax m = new Minimax(1);
            int[] winArray = new int[tries];

            Splendor greedy = new Splendor(g1, g2, 100);
            for (int i = 0; i < tries; i++)
            {
                Splendor.replayGame();
                winArray[i] = g1.wins;
            }
            Splendor mini = new Splendor(g3, m, 100);
            for (int i = 0; i < tries; i++)
            {
                Splendor.replayGame();
                Debug.Assert(winArray[i] == g3.wins, "Games diverged at i= " + i);
            }


        }

        static void Main(string[] args)
        {
            //recording = true;
            //new Splendor(new Greedy(), new Genetic.GreedyGene(), 100);
            //replayGame();
            //return;

            while (true)
            {

                foreach (string s in Console.ReadLine().Replace('(', ' ').Replace(')', ' ').Replace(',', ' ').Split())
                {
                    commands.Enqueue(s);
                }
                if (commands.Count > 0)
                {
                    dequeue();
                }
            }
        }
    }
}
