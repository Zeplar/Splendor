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

        public static Move getGreedyMove(Board b)
        {
            Move bestMove = null;
            int points = -1;

            List<Move.BUY> buys = Move.BUY.getLegalMoves(b);
            foreach (Move.BUY m in buys)
            {
                if (b.generate(m).maximizingPlayer.points > points)
                {
                    points = m.card.points;
                    bestMove = m;
                }
            }
            if (points < 0)
            {
                bestMove = Move.getAllLegalMoves(b).Find(x => true);
            }
            return bestMove;
        }

        public override void takeTurn()
        {

            Move m = getGreedyMove(Board.current);
            if (m != null)
            {
                m.takeAction();
                RecordHistory.record(this + " took move " + m);
                return;
            }
            //If AI did not successfully take a move, restart the game
            noLegalMoves += 1;
            Console.WriteLine("Greedy found no legal moves and was forced to restart " + noLegalMoves + " time(s).");
            RecordHistory.record("Greedy found no legal moves and was forced to restart " + noLegalMoves + " time(s).");
            Splendor.replayGame();
        }
    }
}
