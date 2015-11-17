using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;

namespace Splendor.Genetic
{
    public class SplendorFit : IFitnessFunction
    {
        /// <summary>
        /// Scores chromosomes by pitting them against all enemy chromosomes and averaging the delta point value.
        /// </summary>
        public void runTournament(List<IChromosome> cur, List<IChromosome> next)
        {
            Debug.Assert(cur.Count == next.Count, cur.Count + " " + next.Count);

            //foreach (SplendorGene g in cur)
            //{
            //    g.updateMoves();
            //}
            //foreach (SplendorGene g in next)
            //{
            //    g.updateMoves();
            //}

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
            Debug.Assert(cur.Count == next.Count, "Something changed in the tournament");

        }

        public double Evaluate(IChromosome chromosome)
        {
            SplendorGene g = (SplendorGene)chromosome;
            return g.score;
        }

        public void ClearScore(List<IChromosome> population)
        {
            foreach (SplendorGene g in population)
            {
                g.score = 0;
            }
        }

        private Move getMoveByIndex(int super, int sub, Board b)
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

        private int score(IChromosome max, IChromosome min)
        {
            SplendorGene m = (SplendorGene)max;
            SplendorGene n = (SplendorGene)min;
            return score(m, n);
        }

        private int score(SplendorGene max, SplendorGene min)
        {
            Board b = Board.current;
            int i = 0;
            int j = 0;
            Move nextMove;
            while (i < max.length && j < max.length)
            {
                nextMove = null;
                while (i < max.length && nextMove == null)
                {
                    nextMove = getMoveByIndex(max.moveTypes[i], max.moveValues[i], b);
                    i++;
                }
                b = b.generate(nextMove);
                nextMove = null;
                while (j < max.length && nextMove == null)
                {
                    nextMove = getMoveByIndex(max.moveTypes[j], max.moveValues[j], b);
                    j++;
                }
                b.generate(nextMove);
            }
            return b.maximizingPlayer.points - b.minimizingPlayer.points;
        }
    }
}
