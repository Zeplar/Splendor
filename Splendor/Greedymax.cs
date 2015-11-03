using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor
{

    public class Greedymax : Player
    {

        public int treeDepth = 1;

        public Greedymax()
        {
            gems = Gem.zero;
            reserve = new List<Card>();
            field = new List<Card>();
        }

        public Greedymax(int i)
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

        simMove generateMove(Board b, int depth, bool opp)
        {
            Board newBoard;
            Move bestMove = null;
            simMove sim;
            List<Move> legalMoves;
            int bestScore = opp ? 100 : -100;
            if (depth == treeDepth)
            {
                return new simMove(null, score(b));
            }
            legalMoves = b.legalMoves;

            //remove all Reserves
            legalMoves.RemoveAll(x => x.moveType == 3);
            //Avoid boardstates with no legal moves
            if (legalMoves.Count == 0)
            {
                return new simMove(null, 0);
            }
            foreach (Move m in legalMoves)
            {
                if (!m.isLegal(b))
                {
                   Trace.TraceError("Move " + m + " is not actually legal!");
                }
                newBoard = b.generate(m);
                sim = generateMove(newBoard, depth + 1, false);
                if (!opp)
                {
                    if (sim.score > bestScore && sim.move != null)
                    {
                        bestScore = sim.score;
                        bestMove = m;
                    }
                }
                else
                {
                    if (sim.score < bestScore && sim.move != null)
                    {
                        bestScore = sim.score;
                        bestMove = m;
                    }
                }
            }
            return new simMove(bestMove, bestScore);
        }

        /// <summary>
        /// Scores the board from the perspective of the generating player	
        /// </summary>
        int score(Board b)
        {
            Player self = b.startingPlayer;
            int points = self.points;
            return points;
        }

        public override void takeTurn()
        {
            Board b = Board.current;
            simMove m = generateMove(b, 0, false);
            if (m.move != null)
            {
                m.move.takeAction();
                //			Debug.Log ("Took action " + m.move);
            }
            else if (Move.getAllLegalMoves().Count == 0)
            {
                Trace.TraceError("No legal moves!");
                Splendor.replayGame();
            }
            else
            {
                Move.getAllLegalMoves()[0].takeAction();
                //			Debug.Log ("Took action Random");
            }
        }

    }

}
