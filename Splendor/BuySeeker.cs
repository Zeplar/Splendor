using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
    public class BuySeeker
    {
        private Card toBuy;
        private Gem neededGems;
        private Board state;

        public BuySeeker(Card target, Board state)
        {
            toBuy = target;
            neededGems = (state.maximizingPlayer.discount + state.maximizingPlayer.gems).requiredToBuy(target.cost);
            this.state = state;
        }

        public BuySeeker(Card target) : this(target, Board.current) { }

        public Move getMove()
        {
            Move targetMove = new Move.BUY(toBuy);
            int value = 0;
            if (targetMove.isLegal(state)) return targetMove; //Buy if possible

            if (targetMove.isLegal(state.generate(Move.getRandomMove(state)))) return new Move.RESERVE(toBuy); //Reserve if opponent can buy next turn

            foreach (Move.TAKE2 m in Move.TAKE2.getLegalMoves(state))
            {
                int temp = Math.Min(2, neededGems[m.color]);
                if (temp > value)
                {
                    value = temp;
                    targetMove = m;
                }
            }
            foreach (Move.TAKE3 m in Move.TAKE3.getLegalMoves(state))
            {
                int temp = neededGems.magnitude - (neededGems - m.colors).positive.magnitude;
                if (temp >= value)
                {
                    value = temp;
                    targetMove = m;
                }
            }
            if (value > 0) return targetMove; //Buy if any of the TAKES helped at all (defaulting to TAKE3 by the >=).
            return new Move.RESERVE(toBuy); //Otherwise just reserve it.
        }

    }

}
