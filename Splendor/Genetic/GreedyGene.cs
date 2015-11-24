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


    public class GreedyGene : Player
    {

        private int popSize = 100;
        private int depth = 20;
        private int generations = 10;

        private GreedyFit fit = new GreedyFit();

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
                //if (Splendor.turn > 30)
                //{
                //    Console.Write("Debug");
                //}
                pop.Selection();
            }
            Move m = null;
            SplendorGene g = (SplendorGene)pop.BestChromosome;
            int j = 0;
            while (m == null && j < g.length)
            {
                m = fit.getMoveByIndex(g.moveTypes[j], g.moveValues[j], Board.current);
                j++;
            }
            if (m == null)
            {
                if (Move.getAllLegalMoves().Count > 0)
                {
                    m = Move.getAllLegalMoves()[0];
                    RecordHistory.record(this + " made random move " + m + " from chromosome " + g);
                }
                else
                {
                    Console.WriteLine("GreedyGene failed to find a move on chromosome " + g);
                    RecordHistory.record("GreedyGene restarted the game with chromosome " + g);
                    Splendor.replayGame();
                    return;
                }
            } else
            {
                RecordHistory.record(this + " made move " + m + " from chromosome " + g);
            }
            m.takeAction();
            Board b = Board.current;
            Console.WriteLine();
            Console.Write("   " + b.notCurrentPlayer + " " + b.notCurrentPlayer.points + " - " + b.currentPlayer + " " + b.currentPlayer.points + "   Fitness: " + pop.BestChromosome.Fitness + "  " + m);
        }


    }


}
