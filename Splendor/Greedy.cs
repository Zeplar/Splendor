using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Greedy : Player
    {
        private static int noLegalMoves = 0;

        public override string ToString()
        {
            return "" + this.name + "(" + this.turnOrder + ")";
        }

        public Greedy(string name="Greedy")
        {
            gems = new Gem();
            reserve = new List<Card>();
            field = new List<Card>();
            this.name = name;

        }

        private void getBiggestBuy(List<Move.BUY> buys)
        {
            int bestScore = -1;
            int temp = 0;
            Move.BUY bestMove = null;
            foreach (Move.BUY b in buys)
            {
                temp = Board.current.generate(b).maximizingPlayer.points;
                if (temp > bestScore)
                {
                    bestScore = temp;
                    bestMove = b;
                }
            }
            if (Splendor.recording)
            {
                RecordHistory.record(this.ToString() + " made greedy move " + bestMove);
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
            noLegalMoves += 1;
            Console.WriteLine("Greedy found no legal moves and was forced to restart " + noLegalMoves + " time(s).");
            Splendor.replayGame();
        }
    }
}
