using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Splendor
{
    public class RandomSearch : Player
    {
        public override string ToString()
        {
            return "RandomSearch " + depth + " " + evaluations;
        }
        private int depth;
        private int evaluations;
        private Heuristic f;
        private object Lock = new object();
        public static RandomSearch Create(string[] args)
        {
            Heuristic f;
            if (args.Length < 3)
            {
               throw new FormatException("Usage: random <depth> <evaluations> <...scoring function...>");
            }
            List<string> scoring = new List<string>(args);
            List<int> parameters;
            scoring.RemoveRange(0, 2);
            try
            {
                f = Heuristic.parse(scoring);
                parameters = new List<string>(args).GetRange(0, 2).ConvertAll<int>(x => int.Parse(x));
            }
            catch (FormatException excpt)
            {
                throw excpt;
            }
            return new RandomSearch(f, parameters[0], parameters[1]);
        }

        public RandomSearch(Heuristic f, int depth, int evaluations)
        {
            this.f = f;
            this.depth = depth;
            this.evaluations = evaluations;
        }

        public override void takeTurn()
        {
            Board.useDictionary = true;
            ConcurrentBag<double> allScores = new ConcurrentBag<double>();
            Move bestMove = null;
            double bestScore = double.MinValue;
            int evals = 0;
            while (evals < evaluations) {
                if (evals % 5000 > 4900) Board.current.ResetDictionary();
                evals++;
                Parallel.Invoke(() =>
               {
                   double tempScore = 0;
                   Board b = Board.current;
                   Move tempMove = Move.getRandomMove(b);
                   Move m = tempMove;
                   for (int i = 0; i < depth; i++)
                   {
                       //Simulate Random move
                       if (m == null) break;
                       b = b.generate(m);
                       tempScore += f.evaluate(b);

                       //Check that simulation isn't over
                       if (b.gameOver) break;

                       //Simulate opponent move
                       b = b.generate(Greedy.getGreedyMove(b, f));
                       tempScore -= f.evaluate(b);

                       m = Move.getRandomMove(b);
                   }
                   allScores.Add(tempScore);
                   lock (Lock)
                   {
                       if (tempScore > bestScore)
                       {
                           bestMove = tempMove;
                           bestScore = tempScore;
                       }
                   }

               });
            }
            if (GameController.turn % 3 == 0) RecordHistory.current.snapshot(allScores);
            Board.useDictionary = false;
            takeAction(bestMove);
        }
    }
}
