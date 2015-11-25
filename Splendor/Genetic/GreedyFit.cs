﻿using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;

namespace Splendor.Genetic
{
    public class GreedyFit : IFitnessFunction
    {
        

        public double Evaluate(IChromosome chromosome)
        {
            SplendorGene g = (SplendorGene)chromosome;
            return score(g);
        }

        public Move getMoveByIndex(int super, int sub, Board b)
        {
            List<Move> moves;
            switch (super)
            {
                case 0:
                    moves = Move.TAKE2.getLegalMoves(b).ConvertAll(x => (Move)x);
                    break;
                case 1:
                    moves = Move.TAKE3.getLegalMoves(b).ConvertAll(x => (Move)x);
                    break;
                case 2:
                    moves = Move.BUY.getLegalMoves(b).ConvertAll(x => (Move)x);
                    break;
                default:
                    moves = Move.RESERVE.getLegalMoves(b).ConvertAll(x => (Move)x);
                    break;
            }
            if (moves.Count > 0)
            {
                return moves[sub % moves.Count];
            }
            return null;
        }

        private Board simulateChromosome(SplendorGene max)
        {
            Board b = Board.current;
            int i = 0;
            Move nextMove = null;
            while (i < max.length)
            {
                while (i < max.length && nextMove == null)
                {
                    nextMove = getMoveByIndex(max.major[i], max.minor[i], b);
                    i++;
                }
                if (nextMove == null || b.gameOver)
                {
                    return b;
                }
                b = b.generate(nextMove);

                nextMove = Greedy.getGreedyMove(b);
                if (nextMove == null || b.gameOver)
                {
                    return b;
                }
                b = b.generate(nextMove);
                i++;
            }
            Debug.Assert(b.turn <= 2*max.length, "Board turn error");
            return b;
        }

        //Check this carefully. Bugs found: i not incrementing; returning before the end of the chromosome
        private int score(IChromosome chromosome)
        {
            Board b = simulateChromosome((SplendorGene)chromosome);
            if (b.gameOver)
            {
                if (b.maximizingPlayer.points > b.minimizingPlayer.points)
                {
                    return 100 - b.turn;
                } else
                {
                    return 0;
                }
            }
            if (b.turn < 2) //Penalize chromosomes with no legal moves
            {
                return 0;
            }
            return b.maximizingPlayer.points;
        }
    }
}
