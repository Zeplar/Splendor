using AForge.Genetic;
using System.Diagnostics;
using System;
namespace Splendor.Genetic
{
    //51-49 vs Greedy with 500,10,20
    //59-41 vs Greedy with 500,10,40

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
            Console.WriteLine("Gene: " + popsize + " " + depth + " " + generations);
        }

        public Gene() : this(200, 10, 20) { }

        public override string ToString()
        {
            return "GreedyGene";
        }

        public override void takeTurn()
        {
            RecordHistory.record();

            Population pop = new Population(popSize, new SplendorGene(depth), fit, new RankSelection());

            for (int i=0; i < generations; i++)
            {
                //Getting an index out of range exception here when using RouletteWheelSelection (16 rounds in, seed 100)
                pop.Mutate();
                pop.Selection();
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
