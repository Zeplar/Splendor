﻿using System;
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

        public SelfishGene(Func<Board, int> scoringFunction) : this(scoringFunction, 200, 20) { }

        public SelfishGene(Func<Board, int> scoringFunction, int popsize, int gens)
        {
            fitness = new BuyFit(scoringFunction);
            name = "SelfishGene";
            popSize = popsize;
            generations = gens;
            RecordHistory.clearPlot();
            RecordHistory.plot("Population: " + popsize + " ; Generations: " + gens + Environment.NewLine);
        }


        public override void takeTurn()
        {
            fitness.cards = Board.current.viewableCards.FindAll(x => true);
  //          Console.WriteLine("Loaded cards.");
            var ga = new Population(popSize, new PermutationChromosome(fitness.cards.Count), fitness, new RouletteWheelSelection());
            ga.MutationRate = 0.2;

  //          Console.WriteLine("Loaded GA.");
            for (int i = 0; i < generations; i++)
            {
                ga.RunEpoch();
                CONSOLE.Overwrite("Generations " + i + " out of " + generations);

                if (Splendor.turn < 200)
                {
                    RecordHistory.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                }

            }
        }

    }
}
