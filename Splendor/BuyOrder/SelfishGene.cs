using GeneticSharp.Domain;
using GeneticSharp.Domain.Populations;
using System;
using System.Diagnostics;

namespace Splendor.BuyOrder
{
    public class SelfishGene : Player
    {
        private const int popSize = 200;
        private const int generations = 20;


        public override void takeTurn()
        {
            Population p = new Population(popSize, 2 * popSize, new Chromosome(6));
            p.CreateInitialGeneration();
            var ga = new GeneticAlgorithm(p, new Fitness(), new GeneticSharp.Domain.Selections.RouletteWheelSelection(), new Crossover(2,2), new Mutate());
            ga.MutationProbability = 0;
            ga.CrossoverProbability = 0;
            ga.Termination = new GeneticSharp.Domain.Terminations.GenerationNumberTermination(generations);
            ga.Start();
            Debug.Assert(false, "" + ga.State);
            
        }
    }
}
