using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor
{
    public class RandomPlayer : Player
    {
        public override void takeTurn()
        {
            takeRandomTurn();
        }

        public override string ToString()
        {
            return "Random";
        }
    }
}
