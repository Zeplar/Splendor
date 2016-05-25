using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;
using System;

namespace Splendor.Index
{
    public class IndexFit : IFitnessFunction
    {
        private Heuristic scoringFunction;
        private Heuristic greedy = Heuristic.Points;
        public int timesEvaluated;

        public IndexFit(Heuristic scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        public double Evaluate(IChromosome chromosome)
        {
            timesEvaluated++;
            return Math.Max(1, score((AForge.Genetic.ShortArrayChromosome)chromosome));

        }

        public Move getExactMove(Board b)
        {
            List<Move> moves = Move.getAllLegalMoves(b);
            if (moves.Count == 0)
            {
                return null;
            }
            return moves[GameController.Random.Next(moves.Count)];
        }
       

        /// <summary>
        /// Generates the next boardstate or returns false if unable
        /// </summary>
        private Board generate(AForge.Genetic.ShortArrayChromosome max, Board b)
        {
            Debug.Assert(b.Turn % 2 == 0);
            int i = b.Turn / 2;
            Move m = b.legalMoves[max.Value[i] % b.legalMoves.Count];
            return b.generate(m);
        }

        private bool predictWin(Board current, Move pred)
        {
            if (current.gameOver)
            {
                if (current.winner == 0)
                {
                    RecordHistory.current.record("!!! " + GameController.currentPlayer + " now thinks it's going to win! Pred. Greedy move " + pred);
                }
                else if (current.winner == 1)
                {
                    RecordHistory.current.record("!!! " + GameController.currentPlayer + " now thinks it's going to lose!");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates a chromosome against Greedy
        /// </summary>
        private double score(AForge.Genetic.ShortArrayChromosome max)
        {
            Board current = Board.current;
            Board next;
            Move nextMove;
            Move pred = null; //Predicted move next turn for Greedy
            double score = 0;
            int i = 0;
            while (current.Turn < max.Value.Length)
            {
                if (predictWin(current, pred)) break;
                next = generate(max, current);
                if (next == null) break;
                score += scoringFunction.evaluate(next);
                i++;
                current = next;
                if (predictWin(current, pred)) break;
                //Do the "Generate-next-move" loop for greedy
                nextMove = Greedy.getGreedyMove(current, greedy);
                if (i == 1) pred = nextMove;
                if (nextMove != null)
                {
                    current = current.generate(nextMove);
                }
                else
                {
                    break;
                }
            }

            return score / i;
        }

        private uint hash(Board b)
        {
            Gem bp = b.currentPlayer.Gems + b.currentPlayer.discount;
            Byte[] buyingPower = new Byte[6];
            for (int i=0; i < 6; i++)
            {
                buyingPower[i] = Convert.ToByte(bp[i] % 8);
            }

            Byte[] fieldState = new Byte[2];
            List<Card> startingCards = GameController.boardCards;
            for (int i=0; i < 8 && i < startingCards.Count; i++)
            {
                if (b.BoardCards.Contains(startingCards[i]))
                {
                    fieldState[0] <<= 1;
                    fieldState[0] += 1;
                }
            }
            for (int i = 8; i < 16 && i < startingCards.Count; i++)
            {
                if (b.BoardCards.Contains(startingCards[i]))
                {
                    fieldState[1] <<= 1;
                    fieldState[1] += 1;
                }
            }

            uint res1, res2;
            res1 = buyingPower[0]; res1 <<= 8; res1 += buyingPower[1]; res1 <<= 8; res1 += buyingPower[2]; res1 <<= 8; res1 += buyingPower[3];
            res2 = buyingPower[4]; res2 <<= 8; res2 += buyingPower[5]; res2 <<= 8; res2 += fieldState[0]; res2 <<= 8; res2 += fieldState[1];
            return res1 ^ res2;
        }

    }
}
