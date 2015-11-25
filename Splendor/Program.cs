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
            int i;
            switch (commands.Dequeue())
            {
                case run:
                    Console.Clear();
                    p1 = dequeue() as Player;
                    p2 = dequeue() as Player;
                    currentGame = new Splendor(p1, p2);
                    return null;
                case repeat:
                    i = int.Parse(commands.Dequeue());
                    for (; i > 0; i--)
                    {
                        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                        Console.Write("Repeat " + i);
                        Splendor.replayGame();
                        recordScore();
                    }
                    return null;
                case "debug":
                    debugGame();
                    recordScore();
                    return null;
                case greedy:
                    return new Greedy();
                case "gene":
                    return new Genetic.Gene();
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
                    Console.Clear();
                    //    i = int.Parse(commands.Dequeue());
                    p1 = dequeue() as Player;
                    p2 = dequeue() as Player;
                    currentGame = new Splendor(p1, p2, 100);
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

                foreach (string s in Console.ReadLine().Split())
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
