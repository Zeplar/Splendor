using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Splendor_E
{
    class Program
    {
        static Queue<string> commands = new Queue<string>();
        const string run = "run", repeat = "repeat", greedy = "greedy", minimax = "minimax", greedymax = "greedymax";
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
                case greedy:
                    return new Splendor.Greedy();
                case minimax:
                    i = int.Parse(commands.Dequeue());
                    return new Splendor.Minimax(i);
                case greedymax:
                    i = int.Parse(commands.Dequeue());
                    return new Splendor.Greedymax(i);
                case "clear":
                    Console.Clear();
                    return null;
                case "runseed":
                    Console.Clear();
                    i = int.Parse(commands.Dequeue());
                    p1 = dequeue() as Splendor.Player;
                    p2 = dequeue() as Splendor.Player;
                    currentGame = new Splendor.Splendor(p1, p2, i);
                    return null;
                default:
                    Trace.TraceError("Bad queue command");
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

        static void Main(string[] args)
        {
            while (true) { 
                
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
