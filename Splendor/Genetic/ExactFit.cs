using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;
using System;
using System.IO;

namespace Splendor.Genetic
{
    public class ExactFit : IFitnessFunction
    {

        //public double Evaluate(IChromosome chromosome)
        //{
        //    return ((SplendorGene)chromosome).score;
        //}

        public double Evaluate(IChromosome chromosome)
        {
            return score((SplendorGene)chromosome);

        }

        public Move getExactMove(Board b)
        {
            List<Move> moves = Move.getAllLegalMoves(b);
            if (moves.Count == 0)
            {
                return null;
            }
            return moves[Splendor.random.Next(moves.Count)];
        }
       

        /// <summary>
        /// Generates the next boardstate or returns false if unable
        /// </summary>
        private Board generate(SplendorGene max, Board b)
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
        private int score(SplendorGene max)
        {
            Board current = Board.current;
            Board next;
            Move nextMove;
            while (!current.gameOver && current.turn < max.length)
            {
                next = generate(max, current);
                if (next == null)
                {
                    break;
                }
                current = next;
                //Do the "Generate-next-move" loop for greedy
                nextMove = Greedy.getGreedyMove(current);
                if (nextMove != null)
                {
                    current = current.generate(nextMove);
                }
                else
                {
                    recordPop(max, current, score(current));
                    return score(current);
                }
            }
            recordPop(max, current, score(current));
            return score(current);
        }

        private int score(Board b)
        {
            return b.maximizingPlayer.points;
            //return Math.Max(b.maximizingPlayer.points - b.minimizingPlayer.points, 0);
        }

        const string directory = @"..\..\..\..\Splendor\History\";

        private void recordPop(SplendorGene g, Board b, int fitness)
        {
            //File.AppendAllText(directory + "fitness" + Splendor.turn + ".csv", g.GetHashCode() + "," + b.turn + "," + fitness + Environment.NewLine);
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
            List<Card> startingCards = Splendor.viewableCards;
            for (int i=0; i < 8 && i < startingCards.Count; i++)
            {
                if (b.viewableCards.Contains(startingCards[i]))
                {
                    fieldState[0] <<= 1;
                    fieldState[0] += 1;
                }
            }
            for (int i = 8; i < 16 && i < startingCards.Count; i++)
            {
                if (b.viewableCards.Contains(startingCards[i]))
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
