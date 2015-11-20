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

        public double Evaluate(IChromosome chromosome)
        {
            SplendorGene g = (SplendorGene)chromosome;
            return g.score;
        }

        private void ClearScore(List<IChromosome> population)
        {
            foreach (SplendorGene g in population)
            {
                g.score = 0;
            }
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
            while (true)
            {
                nextMove = null;
                while (i < max.length && nextMove == null)
                {
                    nextMove = getMoveByIndex(max.moveTypes[i], max.moveValues[i], b);
                    i++;
                }
                if (nextMove == null)
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;

                }
                b = b.generate(nextMove);
                nextMove = null;
                while (j < max.length && nextMove == null)
                {
                    nextMove = getMoveByIndex(max.moveTypes[j], max.moveValues[j], b);
                    j++;
                }
                if (nextMove == null)
                {
                    return b.maximizingPlayer.points - b.minimizingPlayer.points;

                }
                b.generate(nextMove);
            }
        }
    }
}
