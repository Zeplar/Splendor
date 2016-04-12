using System;
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
     //   private int generations;
        private BuyOrderChromosome lastBestChromosome = null;
        public static Move predicted;
        private int evaluations = 0;

        int[] xoverimpnts = new int[15];
        int[] mutimpnts = new int[15];
        int diverseChromosomes = 0;
        int totalChromosomes = 0;
        //Watchpoints

        public SelfishGene(Heuristic scoringFunction) : this(scoringFunction, 200, 20) { }

        public SelfishGene(Heuristic scoringFunction, int popsize, int evaluations)
        {
            fitness = new BuyFit(scoringFunction);
            name = "SelfishGene " + scoringFunction.ToString() + " " + popsize + " {" + evaluations + "}";
            popSize = popsize;
            this.evaluations = evaluations;
        }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static SelfishGene Create(string[] args)
        {
            Heuristic f;
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: selfish <popSize> <evaluations> <...scoring function...>");
                return null;
            }
            List<string> scoring = new List<string>(args);
            List<int> parameters;
            scoring.RemoveRange(0, 2);
            try
            {
                f = Heuristic.parse(scoring);
                parameters = new List<string>(args).GetRange(0, 2).ConvertAll<int>(x => int.Parse(x));
            } catch (FormatException)
            {
                throw new FormatException("Usage: selfish <popSize> <evaluations> <...scoring function...>");
            }
            return new SelfishGene(f, parameters[0], parameters[1]);
        }


        public override void takeTurn()
        {
            
            int totalximpnts = 0;
            int totalmimpnts = 0;
            bool tempRecording = GameController.recording;
            GameController.recording = false;
            lastBestChromosome = null;
            double lastBestFitness = 0;
            fitness.cards = Board.current.viewableCards.FindAll(x => x.Deck != Card.Decks.nobles);
            var ga = new Population(popSize, new BuyOrderChromosome(fitness.cards.Count), fitness, new RankSelection(), random);
            ga.CrossoverRate = 0.2;

       //     if (!predicted.Equals(Board.current.PrevMove)) RecordHistory.current.record("!!! Prediction failed.");
       //     predicted = null;
            int i = 0;
            while (fitness.timesEvaluated < evaluations)
            {
                if (lastBestChromosome != null) ga.AddChromosome(lastBestChromosome);
                ga.RunEpoch();
          //      if (ga.FitnessMax < lastBestFitness) throw new Exception("Fitness decreased between generations");
                lastBestChromosome = ga.BestChromosome as BuyOrderChromosome;
                lastBestFitness = ga.FitnessMax;

                CONSOLE.Overwrite(6, "Generations " + i + " Evaluation " + fitness.timesEvaluated);
                RecordHistory.current.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                if ((GameController.turn % 3 == 0) && (i < 4))
                {
                    List<double> fitnesses = ga.getFitnesses();
                    List<double> parents = ga.getParentFitnesses();
                    List<string> snap = new List<string>();
                    for (int j = 0; j < fitnesses.Count; j++) snap.Add(fitnesses[j].ToString() + "," + parents[j].ToString());
                    RecordHistory.current.snapshot(snap);

                }
                xoverimpnts[i] += BuyOrderChromosome.crossOverImprovements - totalximpnts;
                totalximpnts = BuyOrderChromosome.crossOverImprovements;
                mutimpnts[i] += BuyOrderChromosome.mutationImprovements - totalmimpnts;
                totalmimpnts = BuyOrderChromosome.mutationImprovements;
                i++;
            }
            fitness.timesEvaluated = 0;
            GameController.recording = tempRecording;
            if (lastBestChromosome == null) throw new Exception("Null chromosome after evaluation " + i);
            lastBestChromosome.Evaluate(fitness);
            Move m = fitness.simulateMyTurn(lastBestChromosome, Board.current).PrevMove;
            RecordHistory.current.record(this + " took move " + m);

            //foreach (BuyOrderChromosome p in chromosomes)
            //{
            //    RecordHistory.writeToFile("Chromosomes.txt", string.Format("{0:0.00}",p.Fitness) + "|" + p.depth + "|" + p.Value.String() + "|" + string.Format("{0:0.00}",p.parentFitness));
            //}
            //RecordHistory.writeToFile("Chromosomes.txt", "");
            List<BuyOrderChromosome> chromosomes = BuyOrderChromosome.diversify(ga.population);
            CONSOLE.Overwrite(12, "XOver Impnts over gen.: " + xoverimpnts.String());
            CONSOLE.Overwrite(13, "Mut Impnts over gen.: " + mutimpnts.String());
            CONSOLE.Overwrite(14, "Crossover Improvements: " + BuyOrderChromosome.crossOverImprovements + " / " + BuyOrderChromosome.totalCrossOvers);
            CONSOLE.Overwrite(15, "Mutation Improvements: " + BuyOrderChromosome.mutationImprovements + " / " + BuyOrderChromosome.totalMutations);
            CONSOLE.Overwrite(16, "Number of Diverse Chromosomes: " + chromosomes.Count);

            totalChromosomes += ga.population.Count;
            diverseChromosomes += chromosomes.Count;
            CONSOLE.Overwrite(17, "Total diverse/chromosomes: " + ((double)diverseChromosomes / totalChromosomes));
            for (int j = 0; j < chromosomes.Count; j++)
            {
                CONSOLE.Overwrite(18 + j, chromosomes[j].Value.String() + "   depth: " +chromosomes[j].depth + "    fitness: " + chromosomes[j].Fitness);
            }
            takeAction(m);
        }

        public override string ToString()
        {
            return name;
        }



    }
}
