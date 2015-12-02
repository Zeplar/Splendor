using AForge.Genetic;
using System.Diagnostics;
using System;
namespace Splendor.Genetic
{
    //(Population, Depth, Generations)

    //11-89 vs Greedy with 9 restarts
    //12-88 with random search(5000,20,1)

    //34-66 with ExactFit(200,30,20)
    //41-59 with ExactFit(500,40,20)
    //20-20 with ExactFit(1000,40,20)
    //50-50 with ExactFit(300,100,20)
    //57-43 with ExactFit(1000,100,20)
    //21-79 vs Minimax(3) with ExactFit(1000,100,20)

    //55-45 vs Greedy with ExactFit(500,40,20) and upgraded chromosome
    //49-51 vs Minimax(3) with same
    //28-72 vs Minimax(4) with same

    //40-60 vs Greedy with same and adjusted scoring (deltaP * turn)
    //25-75 vs Minimax(3) with same and adjusted scoring

    

        //Mutate with next-move legal
        //


    public class Gene : Player
    {

        private int popSize = 200;
        private int depth = 10;
        private int generations = 20;

        private ExactFit fit = new ExactFit();

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
