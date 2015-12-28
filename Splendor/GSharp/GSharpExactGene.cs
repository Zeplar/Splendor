using System;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Populations;

namespace Splendor.GSharp
{
    class GSharpExactGene : Player
    {
        public int generations;
        public int size;
        public int length;
        public GSharpChromosome lastBest;

        public GSharpExactGene(int generations, int length, int size) : base()
        {
            this.size = size;
            this.generations = generations;
            this.length = length;
            this.name = "GSharp";
        }


        public override void takeTurn()
        {
            lastBest = (lastBest == null) ? new GSharpChromosome(length) : lastBest;
            var population = new Population(size, size, lastBest);
            var selection = new EliteSelection();
            var mutation = new GSharpMutate();
            var fitness = new GSharpFitness();
            var crossover = new GSharpCrossover();
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);

            ga.Termination = new GeneticSharp.Domain.Terminations.GenerationNumberTermination(generations);

            ga.Start();
            lastBest = (GSharpChromosome)ga.BestChromosome;
            Console.WriteLine("Best Fitness: " + ga.BestChromosome.Fitness + " Real Score: " + this.points);
            ((GSharpChromosome.gene)ga.BestChromosome.GetGene(0).Value).move.takeAction();
        }

    }
}
