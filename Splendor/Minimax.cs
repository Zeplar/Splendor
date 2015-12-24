using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Splendor
{

    public class Minimax : Player
    {
        public int treeDepth;
        public Minimax(int i, Func<Board,int> f)
        {
            gems = Gem.zero;
            reserve = new List<Card>();
            field = new List<Card>();
            treeDepth = i;
            scoringFunction = f;
            name = "Minimax " + treeDepth;
        }

        private Func<Board, int> scoringFunction;

        public static Move minimax(Board startingPoint, int depth, Func<Board,int> scoringFunction, out int score)
        {
            Move bestMove = null;
            List<Move> legalMoves = startingPoint.legalMoves;
            int[] bestScore = new int[legalMoves.Count];
            //int val;
            Func<int, int, bool> comp = (x,y) => (x > y);

            if (depth == 0 || legalMoves.Count == 0 || startingPoint.gameOver)
            {
                score = scoringFunction(startingPoint);
                return null;
            }

            if (startingPoint.turn % 2 == 1)
            {
                for(int i=0; i < bestScore.Length; i++) bestScore[i] = int.MaxValue;
                comp = (x, y) => (x < y);
            }
            else for (int i = 0; i < bestScore.Length; i++) bestScore[i] = int.MinValue;


            Parallel.For(0, legalMoves.Count, (x) =>
           {
               Move m = legalMoves[x];
               minimax(startingPoint.generate(m), depth - 1, scoringFunction, out bestScore[x]);
                //if (comp(val, bestScore))
                {
                    //bestScore = val;
                    //bestMove = m;
                }
           });
            bestMove = legalMoves[0];
            for (int i=0; i < bestScore.Length; i++)
            {
                if (comp(bestScore[i], bestScore[0]))
                {
                    bestScore[0] = bestScore[i];
                    bestMove = legalMoves[i];
                }
            }
            score = bestScore[0];
            return bestMove;
        }


        public override void takeTurn()
        {
            Board b = Board.current;
            int x;
            Move k = minimax(b, treeDepth, scoringFunction, out x);
            RecordHistory.record(this + " made move " + k?.ToString());
            k.takeAction();

        }

    }

}
