﻿using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Splendor
{

    public class Minimax : Player
    {

        public int treeDepth;
        private int[][] history;
        private int[] historyLocation;
        private bool recordEverything = false;

        public Minimax(int i)
        {
            gems = Gem.zero;
            reserve = new List<Card>();
            field = new List<Card>();
            treeDepth = i;

        }

        private void makeHistory()
        {
            history = new int[treeDepth + 2][];
            for (int k = 0; k < treeDepth + 2; k++)
            {
                history[k] = new int[(int)Math.Pow(30, k)];
                for (int z = 0; z < history[k].Length; z++)
                {
                    history[k][z] = -200;
                }
            }
            historyLocation = new int[treeDepth + 2];

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
            int bestScore, val, t;

            if (depth == treeDepth || legalMoves.Count == 0)
            {
                if (recordEverything)
                {
                    t = historyLocation[depth + 1];
                    history[depth + 1][t] = score(b);
                    historyLocation[depth + 1] += 1;
                }
                return new simMove(null, score(b));
            }
            if (maxPlayer)
            {
                bestScore = -100;
                foreach (Move m in legalMoves)
                {

                    val = generateMove(b.generate(m), depth + 1, false).score;

                    if (recordEverything)
                    {
                        t = historyLocation[depth + 1];
                        history[depth + 1][t] = val;
                        historyLocation[depth + 1] += 1;
                    }
                    if (val > bestScore)
                    {
                        bestScore = val;
                        bestMove = m;
                    }
                }
                if (recordEverything)
                {
                    historyLocation[depth + 1] += 1;
                    historyLocation[treeDepth + 1] += 1;
                }
                return new simMove(bestMove, bestScore);
            }
            else
            {
                bestScore = 100;
                foreach (Move m in legalMoves)
                {
                    val = generateMove(b.generate(m), depth + 1, true).score;
                    if (recordEverything)
                    {
                        t = historyLocation[depth];
                        history[depth + 1][t] = val;
                        historyLocation[depth + 1] += 1;
                    }
                    if (val < bestScore)
                    {
                        bestScore = val;
                        bestMove = m;
                    }
                }
                if (recordEverything)
                {
                    historyLocation[depth + 1] += 1;
                    historyLocation[treeDepth + 1] += 1;
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
            //int points = self.points;
            if (self.points < 15 && opp.points >= 15)
            {
                return -1000;
            }
            int points = self.points - opp.points;
            return points;
        }

        public override void takeTurn()
        {
            if (recordEverything)
            {
                makeHistory();
            }
            Board b = Board.current;
            simMove m = generateMove(b, 0, true);
            if (recordEverything)
            {
                history[0][0] = m.score;
            }
            if (m.move != null)
            {
                RecordHistory.record("Minimax took move " + m.move.ToString());
                m.move.takeAction();

            }
            else if (Move.getAllLegalMoves().Count == 0)
            {
                Debug.Fail("No legal moves!");
                Splendor.replayGame();
            }
            else
            {
                RecordHistory.record("Minimax took random move " + Move.getAllLegalMoves()[0].ToString());
                Move.getAllLegalMoves()[0].takeAction();
                
            }
            if (recordEverything) {
                RecordHistory.recordMinimaxTree(history);
             }
        }

    }

}
