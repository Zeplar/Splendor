using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor.Strategize2
{
    public abstract class Strategy
    {
        public enum type { GetCard, DenyCard };
        private type strategyType;


        /// <summary>
        /// What card the strategy refers to
        /// </summary>
        private Card reference;
        public Func<Board, bool> endCondition;

        public Strategy(type strategyType, Card reference)
        {
            this.strategyType = strategyType;
            this.reference = reference;
        }

        public abstract Move getMove(Board b);


        public class GetCard : Strategy
        {
            public GetCard(Card reference) : base(type.GetCard, reference)
            {
                endCondition = (bd => bd.currentPlayer.Field.Contains(reference) || bd.notCurrentPlayer.Field.Contains(reference));
            }

            public override Move getMove(Board b)
            {
                ScoringMethods.Function fn = new ScoringMethods.Function(10) * ScoringMethods.HasCard(reference);
                fn += ScoringMethods.Gems;
                fn += ScoringMethods.Lead;
                return BuySeeker.getMove(b, reference);
            }
        }

        public class DenyCard : Strategy
        {
            public DenyCard(Card reference) : base(type.DenyCard, reference)
            {
                endCondition = (bd => !bd.BoardCards.Contains(reference));
            }

            public override Move getMove(Board b)
            {
                if (b.currentPlayer.canBuy(b, reference)) return new Move.BUY(reference);

                Move m = new Move.RESERVE(reference);
                if (m.isLegal(b)) return m;

                return BuySeeker.getMove(b, reference);
            }
        }

    }


}
