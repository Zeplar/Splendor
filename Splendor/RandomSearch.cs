using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
    public class RandomSearch : Player
    {
        private int depth;
        private int evaluations;
        private ScoringMethods.Function f;
        public static RandomSearch Create(string[] args)
        {
            ScoringMethods.Function f;
            if (args.Length < 3)
            {
               throw new FormatException("Usage: random <depth> <evaluations> <...scoring function...>");
            }
            List<string> scoring = new List<string>(args);
            List<int> parameters;
            scoring.RemoveRange(0, 2);
            try
            {
                f = ScoringMethods.parse(scoring);
                parameters = new List<string>(args).GetRange(0, 2).ConvertAll<int>(x => int.Parse(x));
            }
            catch (FormatException excpt)
            {
                throw excpt;
            }
            return new RandomSearch(f, parameters[0], parameters[1]);
        }

        public RandomSearch(ScoringMethods.Function f, int depth, int evaluations)
        {
            this.f = f;
            this.depth = depth - 1;
            this.evaluations = evaluations;
        }

        public override void takeTurn()
        {
            Move bestMove = null;
            double bestScore = double.MinValue;
            double tempScore;
            Move tempMove;
            int evals = 0;
            Board b;
            while (evals < evaluations)
            {
                tempMove = Move.getRandomMove(Board.current);
                b = Board.current.generate(tempMove);
                tempScore = f.evaluate(b);
                evals++;
                for (int i=0; i < depth; i++)
                {
                    //Check that simulation isn't over
                    if (b.gameOver || evals == evaluations) break;

                    //Simulate opponent move
                    b = b.generate(Greedy.getGreedyMove(b, f));
                    tempScore -= f.evaluate(b);
                    evals++;

                    //Check that simulation isn't over
                    if (evals == evaluations) break;

                    //Simulate Random move
                    b = b.generate(Move.getRandomMove(b));
                    tempScore += f.evaluate(b);
                    evals++;
                }

                if (tempScore > bestScore)
                {
                    bestMove = tempMove;
                    bestScore = tempScore;
                }
            }
            takeAction(bestMove);
        }
    }
}
