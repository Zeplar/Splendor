using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor
{

    public class Greedy : Player
    {

        public override string ToString()
        {
            return "Greedy " + this.name + " " + this.turnOrder;
        }

        public Greedy(string name="")
        {
            gems = new Gem();
            reserve = new List<Card>();
            field = new List<Card>();
            this.name = name;

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
            if (Splendor.recording)
            {
                RecordHistory.record(this.ToString() + " made greedy move " + bestMove);
                if (bestMove.card.id == 70)
                {
                    RecordHistory.record();
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
                    if (Splendor.recording)
                    {
                        RecordHistory.record(this.ToString() + " made random move " + allMoves[0].ToString());
                    }
                    allMoves[0].takeAction();
                    return;
                }
            }

            //If AI did not successfully buy a card, take gems, or reserve a card, halt the program.
            Splendor.replayGame();
        }
    }
}
