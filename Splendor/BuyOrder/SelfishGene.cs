using System;
using System.Text;
using AForge.Genetic;
using System.Diagnostics;
using System.Collections.Generic;

namespace Splendor.BuyOrder
{
    public class SelfishGene : Player
    {
        private BuyFit fitness;
        private int popSize;
        private int generations;
        private PermutationChromosome lastBestChromosome = null;
        public static Move predicted;

        //Watchpoints

        public SelfishGene(ScoringMethods.Function scoringFunction) : this(scoringFunction, 200, 20) { }

        public SelfishGene(ScoringMethods.Function scoringFunction, int popsize, int gens)
        {
            fitness = new BuyFit(scoringFunction);
            name = "SelfishGene";
            popSize = popsize;
            generations = gens;
            fn = scoringFunction;
        }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static SelfishGene Create(string[] args)
        {
            ScoringMethods.Function f;
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: selfish <popSize> <generations> <...scoring function...>");
                return null;
            }
            List<string> scoring = new List<string>(args);
            List<int> parameters;
            scoring.RemoveRange(0, 2);
            try
            {
                f = ScoringMethods.parse(scoring);
                parameters = new List<string>(args).GetRange(0, 2).ConvertAll<int>(x => int.Parse(x));
            } catch (FormatException)
            {
                throw new FormatException("Usage: selfish <popSize> <generations> <...scoring function...>");
            }
            return new SelfishGene(f, parameters[0], parameters[1]);
        }


        public override void takeTurn()
        {
            bool tempRecording = GameController.recording;
            GameController.recording = false;
            lastBestChromosome = null;
            fitness.cards = Board.current.viewableCards.FindAll(x => x.deck != GameController.nobles);
            var ga = new Population(popSize, new PermutationChromosome(fitness.cards.Count), fitness, new RouletteWheelSelection(), random);

        //    if (!predicted.Equals(Board.current.prevMove)) RecordHistory.record("!!! Prediction failed.");
            predicted = null;

            for (int i = 0; i < generations; i++)
            {
                if (lastBestChromosome != null) ga.AddChromosome(lastBestChromosome);
                ga.RunEpoch();
                lastBestChromosome = ga.BestChromosome as PermutationChromosome;
                CONSOLE.Overwrite(6, "Generations " + i + " out of " + generations);
                RecordHistory.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                if ((GameController.turn % 10 == 0) && (i == 0 || i == generations / 2 || i == generations-1)) RecordHistory.snapshot(ga.getFitnesses());
            }
            GameController.recording = tempRecording;
            lastBestChromosome.Evaluate(fitness);
            Move m = fitness.simulateMyTurn(lastBestChromosome, Board.current).prevMove;
            RecordHistory.record(this + " took move " + m);
            m.takeAction();
        }

        public override string ToString()
        {
            return "BuyOrder " + fn;
        }

    }
}
