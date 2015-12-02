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
            while (!current.gameOver && current.turn < 20)
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
            return Math.Max(b.maximizingPlayer.points - b.minimizingPlayer.points, 0);
        }

        const string directory = @"..\..\..\..\Splendor\History\";

        private void recordPop(SplendorGene g, Board b, int fitness)
        {
            return;
            File.AppendAllText(directory + "fitness" + Splendor.turn + ".csv", g.GetHashCode() + "," + b.turn + "," + fitness + Environment.NewLine);
        }

    }
}
