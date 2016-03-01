using System;
using System.Collections.Generic;
using System.Linq;

namespace Splendor
{
    public static class BuySeeker
    {
        private static Gem neededGems(Board b, Card toBuy)
        {
            Gem ret = toBuy.Cost - (b.currentPlayer.Gems + b.currentPlayer.discount);
            return ret.positive;
        }

        public static Move getMove(Board state, Card toBuy)
        {
            Move targetMove = new Move.BUY(toBuy);
            if (targetMove.isLegal(state)) return targetMove; //Buy if possible

            if (new Move.RESERVE(toBuy).isLegal(state) && state.notCurrentPlayer.canBuy(state, toBuy))
                return new Move.RESERVE(toBuy); //Reserve if opponent can buy next turn

            Gem needed = neededGems(state, toBuy);
            ScoringMethods.Function fn = ScoringMethods.colors(needed);
            return Greedy.getGreedyMove(state, fn);

            //int value = 0;
            //int x;

            //foreach (Move.TAKE3 m in Move.TAKE3.getLegalMoves(state))
            //{
            //    x = (needed - (needed - m.colors).positive).magnitude;
            //    if (x > value)
            //    {
            //        value = x;
            //        targetMove = m;
            //    }
            //}
            //if (value >= 2) return targetMove;
            //foreach (Move.TAKE2 m in Move.TAKE2.getLegalMoves(state))
            //{
            //    x = (needed - (needed - m.color).positive).magnitude;
            //    if (x > value)
            //    {
            //        value = x;
            //        targetMove = m;
            //    }
            //}
            //if (value > 0) return targetMove;

            //return state.legalMoves[0];
        }

        public static Move getMove(Board state, IEnumerable<Card> toBuy, IEnumerable<int> weights)
        {
            if (toBuy.Count() < 1) throw new Exception("BuySeeker.getMove called with no cards");
            if (toBuy.Count() != weights.Count()) throw new Exception("Buyseeker called with the wrong number of weights");
            Move target = new Move.BUY(toBuy.First());
            if (target.isLegal(state)) return target;   //Buy if possible
            target = new Move.RESERVE(toBuy.First());
            if (state.notCurrentPlayer.canBuy(state, toBuy.First()) && target.isLegal(state)) return target; //Reserve if opponent can buy

            var zipped = toBuy.Zip(weights, (c, w) => new { cd = c, wt = w });
            int[] needed = new int[5] { 0, 0, 0, 0, 0 };
            foreach (var a in zipped)
            {
                Gem toAdd = neededGems(state, a.cd);
                for (int i = 0; i < 5; i++) needed[i] += toAdd[i] * a.wt;
            }
            Gem targetGems = new Gem(needed);
            ScoringMethods.Function fn = ScoringMethods.colors(targetGems);
            return Greedy.getGreedyMove(state, fn);
        }

    }

}
