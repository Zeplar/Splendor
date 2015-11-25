using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;

namespace Splendor.Genetic
{
    public class ExactFit : IFitnessFunction
    {

        /// <summary>
        /// Scores chromosomes by pitting them against all enemy chromosomes.
        /// </summary>
        public void runTournament(List<IChromosome> cur, List<IChromosome> next)
        {
            Debug.Assert(cur.Count == next.Count, cur.Count + " " + next.Count);
            ClearScore(cur);
            ClearScore(next);
            int count = cur.Count;

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    SplendorGene winner;
                    bool curWon = score(cur[i], next[i]) > 0;
                    if (curWon)
                    {
                        winner = (SplendorGene)cur[i];
                    }
                    else
                    //!!! This gives ties to the opponent, but I doubt that throws it off too much (if scoring accounts for tiebreakers).
                    {
                        winner = (SplendorGene)next[i];
                    }
                    winner.score += 1;
                }
            }

            //This is necessary to actually update the chromosomes' (private) value field.
            for (int i=0; i < count; i++)
            {
                cur[count].Evaluate(this);
                next[count].Evaluate(this);
            }
            Debug.Assert(cur.Count == next.Count, "Something changed in the tournament");

        }

        //public double Evaluate(IChromosome chromosome)
        //{
        //    return ((SplendorGene)chromosome).score;
        //}

        public double Evaluate(IChromosome chromosome)
        {
            return score((SplendorGene)chromosome);

        }

        private void ClearScore(List<IChromosome> population)
        {
            foreach (SplendorGene g in population)
            {
                g.score = 0;
            }
        }

        public Move getExactMove(int major, int minor)
        {
            return getExactMove((byte)major, (byte)minor);
        }


        public Move getExactMove(byte major, byte minor)
        {
            switch (major)
            {
                case 0:
                    return new Move.TAKE2(minor % 5);
                case 1:
                    return new Move.TAKE3(Gem.AllThree[minor % 10]);
                case 2:
                    return new Move.BUY(Card.allCardsByID[minor % 90]);
                case 3:
                    return new Move.RESERVE(Card.allCardsByID[minor % 90]);
                default:
                    Debug.Fail("Invalid major move");
                    return null;
            }
        }

        private int score(IChromosome max, IChromosome min)
        {
            SplendorGene m = (SplendorGene)max;
            SplendorGene n = (SplendorGene)min;
            return score(m, n);
        }



        /// <summary>
        /// Evaluates a chromosome against Greedy
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private int score(SplendorGene max)
        {
            Board b = Board.current;
            int i = -1;
            int j = -1;
            Move nextMove;
            while (i < max.length)
            {
                //Return at gameOver, so we go at most one turn too far.
                if (b.gameOver)
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                }

                //Do the "Generate-next-move" loop for max
                do
                {
                    nextMove = getExactMove(max.major[i], max.minor[i]);
                    i++;
                }
                while (i < max.length && !nextMove.isLegal(b));

                if (!nextMove.isLegal(b))
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                }
                else
                {
                    b = b.generate(nextMove);
                }

                //Do the "Generate-next-move" loop for greedy
                nextMove = Greedy.getGreedyMove(b);
                if (nextMove != null)
                {
                    b = b.generate(nextMove);
                } else
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                }

            }
            return b.maximizingPlayer.points - b.minimizingPlayer.points; //If either chromosome runs out of genes
        }

        private int score(SplendorGene max, SplendorGene min)
        {
            Board b = Board.current;
            int i = -1;
            int j = -1;
            Move nextMove;
            while (i < max.length && j < max.length)
            {
                //Return at gameOver, so we go at most one turn too far.
                if (b.gameOver)
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                }

                //Do the "Generate-next-move" loop for max
                do
                {
                    nextMove = getExactMove(max.major[i], max.minor[i]);
                    i++;
                }
                while (i < max.length && !nextMove.isLegal(b));

                if (!nextMove.isLegal(b))
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                } else
                {
                    b = b.generate(nextMove);
                }
                
                //Do the "Generate-next-move" loop for min
                do
                {
                    nextMove = getExactMove(max.major[j], max.minor[j]);
                    j++;
                }
                while (j < max.length && !nextMove.isLegal(b));

                if (!nextMove.isLegal(b))
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;
                } else
                {
                    b = b.generate(nextMove);
                }

            }
            return b.maximizingPlayer.points - b.minimizingPlayer.points; //If either chromosome runs out of genes
        }
    }
}
