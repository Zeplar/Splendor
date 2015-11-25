using AForge.Genetic;
using System.Diagnostics;
using System;
namespace Splendor.Genetic
{

    //11-89 vs Greedy with 9 restarts
    //12-88 with random search(5000,20,1)

        //Check fitness not increasing monotonically -- print moves
        //Mutation/Crossover may be unstable
        //Check random search (>> popsize, 1 generation)


    public class Gene : Player
    {

        private int popSize = 200;
        private int depth = 30;
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
            Move m = null;
            for (int i=0; i < g.length; i++)
            {
                m = fit.getExactMove(g[i][0], g[i][1]);
                if (m != null) break;
            }
            if (m == null)
            {
                Console.WriteLine("Gene restarted the game.");
                Splendor.replayGame();
                return;
            }
            m.takeAction();
            Board b = Board.current;
            Console.WriteLine();
            Console.Write("   " + b.notCurrentPlayer + " " + b.notCurrentPlayer.points + " - " + b.currentPlayer + " " + b.currentPlayer.points + "   Fitness: " + pop.BestChromosome.Fitness + "  " + m);
        }


    }


}
