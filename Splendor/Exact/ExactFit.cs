using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;
using System;
using System.IO;

namespace Splendor.Exact
{
    public class ExactFit : IFitnessFunction
    {
        private ScoringMethods.Function scoringFunction;

        public ExactFit(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        public double Evaluate(IChromosome chromosome)
        {
            return score((ExactChromosome)chromosome);

        }

        public Move getExactMove(Board b)
        {
            List<Move> moves = Move.getAllLegalMoves(b);
            if (moves.Count == 0)
            {
                return null;
            }
            return moves[GameController.random.Next(moves.Count)];
        }
       

        /// <summary>
        /// Generates the next boardstate or returns false if unable
        /// </summary>
        private Board generate(ExactChromosome max, Board b)
        {
            Debug.Assert(b.turn % 2 == 0);
            int i = b.turn / 2;
            if (max.moves.Count <= i)
            {
                max.moves.Add(null);
            }
            if (max.moves[i] == null || !max.moves[i].isLegal(b))
            {
                max.moves[i] = getExactMove(b);
                if (max.moves[i] == null)
                {
                    return null;
                }
            }
            max.boardState[i] = hash(b);
            return b.generate(max.moves[i]);
        }

        /// <summary>
        /// Evaluates a chromosome against Greedy
        /// </summary>
        private double score(ExactChromosome max)
        {
            Board current = Board.current;
            Board next;
            Move nextMove;
            double score = 0;
            while (!current.gameOver && current.turn < max.length)
            {
                next = generate(max, current);
                if (next == null)
                {
                    break;
                }
                score += scoringFunction.evaluate(next);
                current = next;
                //Do the "Generate-next-move" loop for greedy
                nextMove = Greedy.getGreedyMove(current, ScoringMethods.DeltaPoints);
                if (nextMove != null)
                {
                    current = current.generate(nextMove);
                }
                else
                {
                    return score;
                }
            }
            return score;
        }

        private uint hash(Board b)
        {
            Gem bp = b.currentPlayer.gems + b.currentPlayer.discount;
            Byte[] buyingPower = new Byte[6];
            for (int i=0; i < 6; i++)
            {
                buyingPower[i] = Convert.ToByte(bp[i] % 8);
            }

            Byte[] fieldState = new Byte[2];
            List<Card> startingCards = GameController.boardCards;
            for (int i=0; i < 8 && i < startingCards.Count; i++)
            {
                if (b.boardCards.Contains(startingCards[i]))
                {
                    fieldState[0] <<= 1;
                    fieldState[0] += 1;
                }
            }
            for (int i = 8; i < 16 && i < startingCards.Count; i++)
            {
                if (b.boardCards.Contains(startingCards[i]))
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
