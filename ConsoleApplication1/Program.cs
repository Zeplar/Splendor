using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Splendor_E
{
    class Program
    {
        static Queue<string> commands = new Queue<string>();
        const string run = "run", repeat = "repeat", greedy = "greedy", minimax = "minimax";
        static Splendor.Splendor currentGame;
        static Splendor.Player p1;
        static Splendor.Player p2;


        static object dequeue()
        {
            int i;
            switch (commands.Dequeue())
            {
                case run:
                    Console.Clear();
                    p1 = dequeue() as Splendor.Player;
                    p2 = dequeue() as Splendor.Player;
                    currentGame = new Splendor.Splendor(p1, p2);
                    return null;
                case repeat:
                    i = int.Parse(commands.Dequeue());
                    for (; i > 0; i--)
                    {
                        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                        Console.Write("Repeat " + i);
                        Splendor.Splendor.replayGame();
                        recordScore();
                    }
                    return null;
                case "debug":
                    debugGame();
                    recordScore();
                    return null;
                case greedy:
                    return new Splendor.Greedy();
                case minimax:
                    i = int.Parse(commands.Dequeue());
                    return new Splendor.Minimax(i);
                case "clear":
                    Console.Clear();
                    return null;
                case "runseed":
                    Console.Clear();
                //    i = int.Parse(commands.Dequeue());
                    p1 = dequeue() as Splendor.Player;
                    p2 = dequeue() as Splendor.Player;
                    currentGame = new Splendor.Splendor(p1, p2, 100);
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
            Splendor.Splendor.replayGame();

        }

        static void findDiscrepancy(int tries)
            //Greedy diverged from Minimax 1 at i=28 (game 29)
        {
            Splendor.Greedy g1 = new Splendor.Greedy();
            Splendor.Greedy g2 = new Splendor.Greedy();
            Splendor.Greedy g3 = new Splendor.Greedy();
            Splendor.Minimax m = new Splendor.Minimax(1);
            int[] winArray = new int[tries];

            Splendor.Splendor greedy = new Splendor.Splendor(g1, g2, 100);
            for (int i=0; i < tries; i++)
            {
                Splendor.Splendor.replayGame();
                winArray[i] = g1.wins;
            }
            Splendor.Splendor mini = new Splendor.Splendor(g3, m, 100);
            for (int i = 0; i < tries; i++)
            {
                Splendor.Splendor.replayGame();
                Debug.Assert(winArray[i] == g3.wins, "Games diverged at i= " + i);
            }


        }

        static void Main(string[] args)
        {
            //new Splendor.Splendor(new Splendor.Greedy("Bob"), new Splendor.Minimax(1), 100);
            //for (int i = 0; i < 57; i++)
            //{
            //    Splendor.Splendor.replayGame();
            //}
            //Splendor.Splendor.recording = true;
            //Splendor.Splendor.replayGame();

            findDiscrepancy(100);

            while (!true) { 
                
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
