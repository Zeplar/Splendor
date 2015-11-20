using AForge.Genetic;
using System.Diagnostics;
using System;
namespace Splendor.Genetic
{

    //11-89 vs Greedy with 9 restarts

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
            Population pop = new Population(popSize, new SplendorGene(depth), fit, new EliteSelection());

            for (int i=0; i < generations; i++)
            {
                //Getting an index out of range exception here when using RouletteWheelSelection
                pop.RunEpoch();
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
                if (Move.getAllLegalMoves().Count > 0 )
                {
                    m = Move.getAllLegalMoves()[0];
                    m.takeAction();
                    return;
                }
                Console.WriteLine("GreedyGene failed to find a move and restarted the game.");
                Splendor.replayGame();
                return;
            }
            m.takeAction();
            Board b = Board.current;
         //   Console.WriteLine();
         //   Console.Write("   " + b.notCurrentPlayer + " " + b.notCurrentPlayer.points + " - " + b.currentPlayer + " " + b.currentPlayer.points + "   Fitness: " + pop.BestChromosome.Fitness);
        }
    }
}
