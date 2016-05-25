using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor.Index
{

    public class IndexGene : Player
    {

        private int popSize = 200;
        private int depth;
        private int generations = 20;
        private int evaluations = 0;
        private AForge.Genetic.ShortArrayChromosome lastBestChromosome = null;
        private IndexFit fit;


        public IndexGene(int popsize, int evaluations, Heuristic scoringFunction)
        {
            name = "Index " + scoringFunction.ToString();
            popSize = popsize;
            depth = 5;
            this.evaluations = evaluations;
            fit = new IndexFit(scoringFunction);
        }

        public IndexGene(int popsize, int depth, int generations, Heuristic scoringFunction)
        {
            name = "Index " + scoringFunction.ToString();
            popSize = popsize;
            this.depth = depth;
            this.generations = generations;
            fit = new IndexFit(scoringFunction);
        }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static IndexGene Create(string[] args)
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
            return new IndexGene(parameters[0], parameters[1], f);
        }

        public override string ToString()
        {
            return name;
        }

        public override void takeTurn()
        {
            RecordHistory.current.record();
            AForge.Genetic.Population ga = new AForge.Genetic.Population(popSize, new AForge.Genetic.ShortArrayChromosome(depth), fit, new AForge.Genetic.RankSelection(), random);
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
            lastBestChromosome = ga.BestChromosome as AForge.Genetic.ShortArrayChromosome;
            fit.timesEvaluated = 0;
            GameController.recording = tempRecord;
            fit.Evaluate(lastBestChromosome);
            Move m = Board.current.legalMoves[lastBestChromosome.Value[0] % Board.current.legalMoves.Count];
            takeAction(m);
            RecordHistory.current.record(this + " took move " + m);
        }


    }


}
