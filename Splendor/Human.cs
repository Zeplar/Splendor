using System;
using System.Collections.Generic;

namespace Splendor
{
    public class Human : Player
    {
        public override void takeTurn()
        {
            printInfo();
            while (true)
            {
                Move m = SynchronousSocketListener.parseRequest();
                if (m == null) continue;
                if (m.isLegal())
                {
                    Console.WriteLine("Legal move.");
                    SynchronousSocketListener.Reply(true);
                    takeAction(m);
                    return;
                }
                else
                {
                    Console.WriteLine("Illegal move: " + m);
                    SynchronousSocketListener.Reply(false);
                }
            }
        }

        public override void GameOver()
        {
            try
            {
                SynchronousSocketListener.EndGame();
            }
            catch { };
        }

        private void printInfo()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            if (Board.current.PrevMove != null) Console.WriteLine("AI's last move: " + Board.current.PrevMove);
            Console.WriteLine("Board cards: " + Board.current.BoardCards.String());
            Console.WriteLine("Board gems: " + Gem.board);
            Console.WriteLine();
            Console.WriteLine("Your cards: " + Field.String());
            Console.WriteLine("Your reserve: " + Reserve.String());
            Console.WriteLine("Your gems: " + Gems.ToString());
            Console.WriteLine();
        }
    }
}
