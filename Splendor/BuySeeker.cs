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
        private Gem neededGems(Board b)
        {
            return (b.maximizingPlayer.discount + b.maximizingPlayer.gems).requiredToBuy(toBuy.cost);
        }
        private Board state;

        public BuySeeker(Card target, Board state)
        {
            toBuy = target;
            this.state = state;
        }

        public BuySeeker(Card target) : this(target, Board.current) { }

        public Move getMove()
        {
            Move targetMove = new Move.BUY(toBuy);
            int value = 100;
            if (targetMove.isLegal(state)) return targetMove; //Buy if possible

            if (targetMove.isLegal(state.generate(Move.getRandomMove(state))) && new Move.RESERVE(toBuy).isLegal()) return new Move.RESERVE(toBuy); //Reserve if opponent can buy next turn
            targetMove = null;
            foreach (Move m in Move.TAKE2.getLegalMoves())
            {
                int x = neededGems(state.generate(m)).magnitude;
                if (x < value)
                {
                    value = x;
                    targetMove = m;
                }
            }
            foreach (Move m in Move.TAKE3.getLegalMoves())
            {
                int x = neededGems(state.generate(m)).magnitude;
                if (x <= value)
                {
                    value = x;
                    targetMove = m;
                }
            }
            if (targetMove != null) return targetMove;
            if (new Move.RESERVE(toBuy).isLegal()) return new Move.RESERVE(toBuy);
            return Move.getRandomMove();
        }

    }

}
