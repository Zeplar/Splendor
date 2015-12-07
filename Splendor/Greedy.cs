using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Greedy : Player
    {
        private int noLegalMoves = 0;
        private int scoringFunction = 0;


        private static int score(Board b, int scoringFunction)
        {
            switch (scoringFunction)
            {
                case 0:
                    return b.maximizingPlayer.points;
                case 1:
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                case 2:
                    return -b.minimizingPlayer.points;
                case 3:
                    return 2 * b.maximizingPlayer.points + b.maximizingPlayer.field.Count;
                default:
                    throw new NotImplementedException();
            }
        }


        public Greedy() { }
        public Greedy(string c)
        {
            switch (c)
            {
                case "max": scoringFunction = 0; break;
                case "delta": scoringFunction = 1;break;
                case "min": scoringFunction = 2; break;
                case "adjusted": scoringFunction = 3; break;
            }
            this.name = c;
        }

        public override string ToString()
        {
            return "" + this.name;
        }


        public static Move getGreedyMove(Board b, int scoringFunction=0)
        {
            Move bestMove = null;
            int bestScore = -100;

            List<Move.BUY> buys = Move.BUY.getLegalMoves(b);
            foreach (Move.BUY m in buys)
            {
                int temp = score(b.generate(m), scoringFunction);
                if (temp > bestScore)
                {
                    bestScore = temp;
                    bestMove = m;
                }
            }
            if (bestMove == null)
            {
                bestMove = Move.getAllLegalMoves(b).Find(x => true);
            }
            return bestMove;
        }

        public override void takeTurn()
        {

            Move m = getGreedyMove(Board.current, this.scoringFunction);
            if (m != null)
            {
                m.takeAction();
                RecordHistory.record(this + " took move " + m);
                return;
            }
            if (takeRandomTurn()) return;
            noLegalMoves++;
            Console.WriteLine(this + " found no legal move and restarted for the " + noLegalMoves + " time.");
            Splendor.replayGame();
        }
    }
}
