using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor.Strategize
{
    //51-49 vs Greedy with 500,10,20
    //56-44 {54-46} vs Greedy with 500,10,20 and new mutate

    //With Crossover:
    //50-50 vs Greedy with (200,10,20)
    //74-26 vs Greedy with (500,10,40)
    //55-45 vs Minimax(3) with (500,10,40)
    //45-39 vs Greedy with (100, 20, 40)
    //15-27 vs Minimax(3) with (100,20,40)
    //5-5 vs Minimax(3) with (100,20,100)


    //ThreadPool.QueueUserWorkItem()

    public class Strategize : Player
    {

        private int popSize = 200;
        private int depth = 20;
        private int generations = 20;
        private int evaluations = 0;
        private StrategizeChromosome lastBestChromosome = null;
        private StrategizeFit fit;

        public Strategize(int popsize, int evaluations, Heuristic scoringFunction)
        {
            name = "Strategize " + scoringFunction.ToString();
            this.popSize = popsize;
            this.depth = 10;
            this.evaluations = evaluations;
            fit = new StrategizeFit(scoringFunction);
        }

        public Strategize(int popsize, int depth, int generations, Heuristic scoringFunction)
        {
            name = "Strategize " + scoringFunction.ToString();
            this.popSize = popsize;
            this.depth = depth;
            this.generations = generations;
            fit = new StrategizeFit(scoringFunction);

        }

        public Strategize(Heuristic fn) : this(500, 10, 20, fn) { }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static Strategize Create(string[] args)
        {
            Heuristic f;
            if (args.Length < 3)
            {
                throw new FormatException("Usage: Strategize <popSize> <evaluations> <...scoring function...>");
            }
            List<string> scoring = new List<string>(args);
            List<int> parameters;
            scoring.RemoveRange(0, 2);
            try
            {
                f = Heuristic.parse(scoring);
                parameters = new List<string>(args).GetRange(0, 2).ConvertAll<int>(x => int.Parse(x));
            }
            catch (FormatException z)
            {
                throw z;
            }
            return new Strategize(parameters[0], parameters[1], f);
        }

        public override string ToString()
        {
            return name;
        }

        public override void takeTurn()
        {
            RecordHistory.current.record();
            AForge.Genetic.Population ga = new AForge.Genetic.Population(popSize, new StrategizeChromosome(depth), fit, new AForge.Genetic.RankSelection(), random);
            bool tempRecord = GameController.recording;
            GameController.recording = false;
            int i = 0;
            while (fit.timesEvaluated < evaluations)
            {
                //Getting an index out of range exception here when using RouletteWheelSelection (16 rounds in, seed 100)
                ga.RunEpoch();
                RecordHistory.current.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                if ((GameController.turn % 5 == 0) && (i == 0 || i == generations / 2 || i == generations - 1))
                {
                    List<double> fitnesses = ga.getFitnesses();
                    List<double> parents = ga.getParentFitnesses();
                    List<string> snap = new List<string>();
                    for (int j = 0; j < fitnesses.Count; j++) snap.Add(fitnesses[j].ToString() + "," + parents[j].ToString());
                    RecordHistory.current.snapshot(snap);
                }
                i++;
            }
            lastBestChromosome = ga.BestChromosome as StrategizeChromosome;
            fit.timesEvaluated = 0;
            GameController.recording = tempRecord;
            fit.Evaluate(lastBestChromosome);
            Move m = Greedy.getGreedyMove(Board.current, lastBestChromosome.getFn);
            if (m == null)
            {
                throw new NullReferenceException("Strategize couldn't find a random move.");
            }
            takeAction(m);
            RecordHistory.current.record(this + " took move " + m);
        }


    }


}
