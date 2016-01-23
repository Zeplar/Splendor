using GeneticSharp;
using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor.Exact
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

    public class ExactGene : Player
    {

        private int popSize = 200;
        private int depth = 10;
        private int generations = 20;

        private ExactFit fit;

        private double totalTime = 0;

        public ExactGene(int popsize, int depth, int generations, ScoringMethods.Function scoringFunction)
        {
            this.popSize = popsize;
            this.depth = depth;
            this.generations = generations;
            fit = new ExactFit(scoringFunction);
            RecordHistory.clearPlot();
            RecordHistory.plot("EXACT GENE|||Population: " + popsize + " ; Generations: " + generations + Environment.NewLine);
        }

        public ExactGene(ScoringMethods.Function fn) : this(500, 10, 20, fn) { }

        public override string ToString()
        {
            return "GreedyGene";
        }

        public override void takeTurn()
        {
            RecordHistory.record();
            DateTime start = DateTime.Now;
            AForge.Genetic.Population pop = new AForge.Genetic.Population(popSize, new ExactChromosome(depth), fit, new AForge.Genetic.RankSelection());
            for (int i=0; i < generations; i++)
            {
                //Getting an index out of range exception here when using RouletteWheelSelection (16 rounds in, seed 100)
                pop.Selection();
                pop.Crossover();
                pop.Mutate();
                RecordHistory.plot(i + "," + pop.FitnessMax + Environment.NewLine);
            }
            double thisTurn = (DateTime.Now - start).TotalSeconds;
            totalTime += thisTurn;
            CONSOLE.Overwrite("Average turn time: " + 2*totalTime / (1+GameController.turn) + Environment.NewLine + "Turn: " + GameController.turn);
            ExactChromosome g = (ExactChromosome)pop.BestChromosome;
            Move m = g.moves[0];
            if (m == null)
            {
                m = Move.getRandomMove();
                Debug.Assert(m != null, "ExactGene couldn't even find a random move.");
            }
            m.takeAction();
            Board b = Board.current;
        }


    }


}
