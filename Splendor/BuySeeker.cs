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

            if (targetMove.isLegal(state.generate(state.legalMoves[0])) && new Move.RESERVE(toBuy).isLegal(state)) return new Move.RESERVE(toBuy); //Reserve if opponent can buy next turn
            targetMove = null;
            List<Move> takes = state.legalMoves.FindAll(x => x.moveType == 0 || x.moveType == 1);
            foreach (Move m in takes)
            {
                int x = neededGems(state.generate(m)).magnitude;
                if (x < value)
                {
                    value = x;
                    targetMove = m;
                }
            }
            if (targetMove != null) return targetMove;
            if (new Move.RESERVE(toBuy).isLegal(state)) return new Move.RESERVE(toBuy);
            return state.legalMoves[0];
        }

    }

}
