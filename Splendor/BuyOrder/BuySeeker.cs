using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
    public class BuySeeker
    {
        private Gem neededGems(Board b, Card toBuy)
        {
            Gem ret = toBuy.Cost - (b.currentPlayer.Gems + b.currentPlayer.discount);
            return ret.positive;
        }

        public BuySeeker() { }

        public BuySeeker(Card target, Board state)
        {
            throw new Exception();
        }

        public BuySeeker(Card target) : this(target, Board.current) { }

        public Move getMove(Board state, Card toBuy)
        {
            Move targetMove = new Move.BUY(toBuy);
            if (targetMove.isLegal(state)) return targetMove; //Buy if possible

            if (new Move.RESERVE(toBuy).isLegal(state) && state.notCurrentPlayer.canBuy(state, toBuy))
                return new Move.RESERVE(toBuy); //Reserve if opponent can buy next turn

            Gem needed = neededGems(state, toBuy);
            int value = 0;
            int x;

            foreach (Move.TAKE3 m in Move.TAKE3.getLegalMoves(state))
            {
                x = (needed - (needed - m.colors).positive).magnitude;
                if (x > value)
                {
                    value = x;
                    targetMove = m;
                }
            }
            if (value >= 2) return targetMove;
            foreach (Move.TAKE2 m in Move.TAKE2.getLegalMoves(state))
            {
                x = (needed - (needed - m.color).positive).magnitude;
                if (x > value)
                {
                    value = x;
                    targetMove = m;
                }
            }
            if (value > 0) return targetMove;

            return state.legalMoves[0];
        }

    }

}
