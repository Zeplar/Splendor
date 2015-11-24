using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor
{
    public class RandomPlayer : Player
    {
        static int counter = 0;
        public override void takeTurn()
        {
            List<Move> moves = Move.getAllLegalMoves();
            while (moves.Count < 1)
            {
                //counter++;
                //Console.WriteLine("No legal moves for the " + counter + " time.");

                //Splendor.replayGame();
                //return;
                returnRandomGems();
                moves = Move.getAllLegalMoves();
            }
            moves[random.Next(0, moves.Count)].takeAction();
        }

        /// <summary>
        /// Returns a random gem to the pile.
        /// </summary>
        private void returnRandomGems()
        {
            int k = 0;

            while (true)
            {
                k = random.Next(5);
                if (gems[k] > 0)
                {
                    Gem.board[k] += 1;
                    gems[k] -= 1;
                    return;
                }
            }
        }

        public override string ToString()
        {
            return "Random";
        }
    }
}
