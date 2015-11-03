using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor
{

    public class Greedy : Player
    {

        public Greedy()
        {
            gems = new Gem();
            reserve = new List<Card>();
            field = new List<Card>();
        }

        private void getBiggestBuy(List<Move.BUY> buys)
        {
            int bestScore = -1;
            Move.BUY bestMove = null;
            foreach (Move.BUY b in buys)
            {
                if (b.card.points > bestScore)
                {
                    bestScore = b.card.points;
                    bestMove = b;
                }
            }
            bestMove.takeAction();
        }

        public override void takeTurn()
        {
            //Get a list of cards that are affordable
            List<Move.BUY> buys = Move.BUY.getLegalMoves();

            //Attempt to buy the highest-tier affordable card
            if (buys.Count > 0)
            {
                getBiggestBuy(buys);
                return;
            }
            else
            {
                List<Move> allMoves = Move.getAllLegalMoves();
                if (allMoves.Count > 0)
                {
                    allMoves[0].takeAction();
                    return;
                }
            }

            //If AI did not successfully buy a card, take gems, or reserve a card, halt the program.
            Splendor.replayGame();
        }
    }
}
