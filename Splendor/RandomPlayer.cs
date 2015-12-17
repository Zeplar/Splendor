using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor
{
    public class RandomPlayer : Player
    {
        public override void takeTurn()
        {
            Move m = Move.getRandomMove();
            Debug.Assert(m != null, "Random couldn't find a legal move.");
            m.takeAction();
        }

        public override string ToString()
        {
            return "Random";
        }
    }
}
