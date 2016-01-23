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

        public SelfishGene(ScoringMethods.Function scoringFunction) : this(scoringFunction, 200, 20) { }

        public SelfishGene(ScoringMethods.Function scoringFunction, int popsize, int gens)
        {
            fitness = new BuyFit(scoringFunction);
            name = "SelfishGene";
            popSize = popsize;
            generations = gens;
            RecordHistory.clearPlot();
            RecordHistory.plot(popsize + "," + gens + Environment.NewLine);
        }


        public override void takeTurn()
        {
            fitness.cards = Board.current.viewableCards.FindAll(x => true);
  //          Console.WriteLine("Loaded cards.");
            var ga = new Population(popSize, new PermutationChromosome(fitness.cards.Count), fitness, new RankSelection());
            ga.MutationRate = 0.1;

  //          Console.WriteLine("Loaded GA.");
            for (int i = 0; i < generations; i++)
            {
                ga.RunEpoch();
                CONSOLE.Overwrite(6, "Generations " + i + " out of " + generations);
                CONSOLE.WriteLine("Best chromosome this generation: " + write((PermutationChromosome)ga.BestChromosome) + " | " + ga.BestChromosome.Fitness);
                RecordHistory.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                //if ((GameController.turn % 10 == 0) && (i == 0 || i == generations / 2 || i == generations-1)) RecordHistory.plot(ga.getFitnesses());
            }
        }

        private string write(PermutationChromosome p)
        {
            return p.Value.String();
        }

    }
}
