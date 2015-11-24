using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Minimax : Player
    {
        static int counter = 0;
        public int treeDepth;
        public Minimax(int i)
        {
            gems = Gem.zero;
            reserve = new List<Card>();
            field = new List<Card>();
            treeDepth = i;

        }

        class simMove
        {

            public Move move;
            public int score;
            public simMove(Move m, int s)
            {
                move = m;
                score = s;
            }
        }

        simMove generateMove(Board b, int depth, bool maxPlayer)
        {
            Move bestMove = null;
            List<Move> legalMoves = b.legalMoves;
            legalMoves.RemoveAll(x => x.moveType == 3);
            int bestScore, val;

            if (depth == treeDepth || legalMoves.Count == 0 || b.gameOver)
            {
                return new simMove(null, score(b));
            }
            if (maxPlayer)
            {
                bestScore = -100;
                foreach (Move m in legalMoves)
                {
                    val = generateMove(b.generate(m), depth + 1, false).score;
                    if (val > bestScore)
                    {
                        bestScore = val;
                        bestMove = m;
                    }
                }
                return new simMove(bestMove, bestScore);
            }
            else
            {
                bestScore = 100;
                foreach (Move m in legalMoves)
                {
                    val = generateMove(b.generate(m), depth + 1, true).score;
                    if (val < bestScore)
                    {
                        bestScore = val;
                        bestMove = m;
                    }
                }
                return new simMove(bestMove, bestScore);
            }
        }

        /// <summary>
        /// Scores the board from the perspective of the generating player	
        /// </summary>
        private int score(Board b)
        {
            Player self = b.maximizingPlayer;
            Player opp = b.minimizingPlayer;
            Debug.Assert(self != opp);
            int points = self.points - opp.points;
            if (b.gameOver)
            {
                if (points < 0)
                {
                    return -1000;
                }
                if (points > 0)
                {
                    return 1000;
                }
                if (points == 0)
                {
                    return 1000 * (opp.field.Count - self.field.Count);
                }
            }
            return points;
        }


        public override void takeTurn()
        {
            Board b = Board.current;
            simMove m = generateMove(b, 0, true);
            if (m.move != null)
            {
                RecordHistory.record(this + " made move " + m.move.ToString());
                m.move.takeAction();

            }
            else if (Move.getAllLegalMoves().Count == 0)
            {
                counter++;
                Console.WriteLine("No legal moves for the " + counter + " time");
                Splendor.replayGame();
            }
            else
            {
                RecordHistory.record(this + " made random move " + Move.getAllLegalMoves()[0].ToString());
                Move.getAllLegalMoves()[0].takeAction();
                
            }
        }

    }

}
