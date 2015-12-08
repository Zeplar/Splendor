using AForge.Genetic;
using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor.Genetic
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

    public class Gene : Player
    {

        private int popSize = 200;
        private int depth = 10;
        private int generations = 20;

        private ExactFit fit = new ExactFit();

        public Gene(int popsize, int depth, int generations)
        {
            this.popSize = popsize;
            this.depth = depth;
            this.generations = generations;
            //Console.WriteLine("Gene: " + popsize + " " + depth + " " + generations);
        }

        public Gene() : this(200, 10, 20) { }

        public override string ToString()
        {
            return "GreedyGene";
        }

        public override void takeTurn()
        {
            RecordHistory.record();

            Population pop = new Population(popSize, new SplendorGene(depth), fit, new testRoulette());

            for (int i=0; i < generations; i++)
            {
                //Getting an index out of range exception here when using RouletteWheelSelection (16 rounds in, seed 100)
                pop.Crossover();
                pop.Mutate();
                pop.Selection();
                //Console.WriteLine("Best fitness in gen " + i + ": " + pop.BestChromosome.Fitness);
            }
            SplendorGene g = (SplendorGene)pop.BestChromosome;
            Move m = g.moves[0];
            if (m == null)
            {
                Console.WriteLine();
                Console.Write("Gene took a random turn.");
                takeRandomTurn();
                return;
            }
            m.takeAction();
            Board b = Board.current;
            //Console.WriteLine();
            //Console.Write("    Dead moves:" + g.score);
            //Console.Write("   " + b.notCurrentPlayer + " " + b.notCurrentPlayer.points + " - " + b.currentPlayer + " " + b.currentPlayer.points + "   Fitness: " + pop.BestChromosome.Fitness + "  " + m);
        }


    }


}
