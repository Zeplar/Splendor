using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor.Exact
{

    public class ExactGene : Player
    {

        private int popSize = 200;
        private int depth;
        private int generations = 20;
        private int evaluations = 0;
        private ExactChromosome lastBestChromosome = null;
        private ExactFit fit;

        int[] xoverimpnts = new int[15];
        int[] mutimpnts = new int[15];

        public ExactGene(int popsize, int evaluations, Heuristic scoringFunction)
        {
            name = "Exact " + scoringFunction.ToString();
            this.popSize = popsize;
            this.depth = 5;
            this.evaluations = evaluations;
            fit = new ExactFit(scoringFunction);
        }

        public ExactGene(int popsize, int depth, int generations, Heuristic scoringFunction)
        {
            name = "Exact " + scoringFunction.ToString();
            this.popSize = popsize;
            this.depth = depth;
            this.generations = generations;
            fit = new ExactFit(scoringFunction);
        }

        public ExactGene(Heuristic fn) : this(500, 10, 20, fn) { }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static ExactGene Create(string[] args)
        {
            Heuristic f;
            if (args.Length < 3)
            {
                throw new FormatException("Usage: exact <popSize> <evaluations> <...scoring function...>");
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
            return new ExactGene(parameters[0], parameters[1], f);
        }

        public override string ToString()
        {
            return name;
        }

        public override void takeTurn()
        {
            int totalximpnts = 0;
            int totalmimpnts = 0;
            RecordHistory.current.record();
            AForge.Genetic.Population ga = new AForge.Genetic.Population(popSize, new ExactChromosome(depth), fit, new AForge.Genetic.RankSelection(), random);
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

                xoverimpnts[i] += ExactChromosome.crossOverImprovements - totalximpnts;
                totalximpnts = ExactChromosome.crossOverImprovements;
                mutimpnts[i] += ExactChromosome.mutationImprovements - totalmimpnts;
                totalmimpnts = ExactChromosome.mutationImprovements;

                i++;
            }
            lastBestChromosome = ga.BestChromosome as ExactChromosome;
            fit.timesEvaluated = 0;
            GameController.recording = tempRecord;
            fit.Evaluate(lastBestChromosome);
            Move m = lastBestChromosome.moves[0];
            if (m == null)
            {
                m = Move.getRandomMove();
                throw new NullReferenceException("ExactGene couldn't find a random move.");
            }


            CONSOLE.Overwrite(12, "XOver Impnts over gen.: " + xoverimpnts.String());
            CONSOLE.Overwrite(13, "Mut Impnts over gen.: " + mutimpnts.String());
            CONSOLE.Overwrite(14, "Crossover Improvements: " + ExactChromosome.crossOverImprovements + " / " + ExactChromosome.totalCrossOvers);
            CONSOLE.Overwrite(15, "Mutation Improvements: " + ExactChromosome.mutationImprovements + " / " + ExactChromosome.totalMutations);





            takeAction(m);
            RecordHistory.current.record(this + " took move " + m);
        }


    }


}
