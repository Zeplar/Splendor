using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor.Exact
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

    public class ExactGene : Player
    {

        private int popSize = 200;
        private int depth = 20;
        private int generations = 20;
        private ExactChromosome lastBestChromosome = null;
        private ExactFit fit;

        public ExactGene(int popsize, int depth, int generations, ScoringMethods.Function scoringFunction)
        {
            this.popSize = popsize;
            this.depth = depth;
            this.generations = generations;
            fn = scoringFunction;
            fit = new ExactFit(scoringFunction);
            RecordHistory.clearPlot();
            RecordHistory.plot("EXACT GENE|||Population: " + popsize + " ; Generations: " + generations + Environment.NewLine);
        }

        public ExactGene(ScoringMethods.Function fn) : this(500, 10, 20, fn) { }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static ExactGene Create(string[] args)
        {
            ScoringMethods.Function f;
            if (args.Length < 4)
            {
                throw new FormatException("Usage: exact <popSize> <depth> <generations> <...scoring function...>");
            }
            List<string> scoring = new List<string>(args);
            List<int> parameters;
            scoring.RemoveRange(0, 3);
            try
            {
                f = ScoringMethods.parse(scoring);
                parameters = new List<string>(args).GetRange(0, 3).ConvertAll<int>(x => int.Parse(x));
            }
            catch (FormatException z)
            {
                throw z;
            }
            return new ExactGene(parameters[0], parameters[1], parameters[2], f);
        }

        public override string ToString()
        {
            return "Exact " + fn;
        }

        public override void takeTurn()
        {
            RecordHistory.record();
            AForge.Genetic.Population pop = new AForge.Genetic.Population(popSize, new ExactChromosome(depth), fit, new AForge.Genetic.RankSelection(), random);
            bool tempRecord = GameController.recording;
            GameController.recording = false;
            for (int i=0; i < generations; i++)
            {
                if (lastBestChromosome != null) pop.AddChromosome(lastBestChromosome);
                //Getting an index out of range exception here when using RouletteWheelSelection (16 rounds in, seed 100)
                pop.RunEpoch();
                RecordHistory.plot(i + "," + pop.FitnessMax + Environment.NewLine);
                if ((GameController.turn % 5 == 0) && (i == 0 || i == generations / 2 || i == generations - 1)) RecordHistory.snapshot(pop.getFitnesses());
                lastBestChromosome = pop.BestChromosome as ExactChromosome;

            }
            GameController.recording = tempRecord;
            fit.Evaluate(lastBestChromosome);
            Move m = lastBestChromosome.moves[0];
            if (m == null)
            {
                m = Move.getRandomMove();
                throw new NullReferenceException("ExactGene couldn't find a random move.");
            }
            m.takeAction();
            RecordHistory.record(this + " took move " + m);
        }


    }


}
