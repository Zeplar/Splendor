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
        //Watchpoints

        public SelfishGene(ScoringMethods.Function scoringFunction) : this(scoringFunction, 200, 20) { }

        public SelfishGene(ScoringMethods.Function scoringFunction, int popsize, int evaluations)
        {
            fitness = new BuyFit(scoringFunction);
            name = "SelfishGene " + scoringFunction.ToString();
            popSize = popsize;
            this.evaluations = evaluations;
        }

        /// <summary>
        /// Used for registration in the player factory.
        /// </summary>
        public static SelfishGene Create(string[] args)
        {
            ScoringMethods.Function f;
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
                f = ScoringMethods.parse(scoring);
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
            ga.CrossoverRate = 0;
            ga.MutationRate = 0;
       //     if (!predicted.Equals(Board.current.PrevMove)) RecordHistory.current.record("!!! Prediction failed.");
            predicted = null;
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
                if ((GameController.turn % 5 == 0) && (i % 4 == 0))
                {
                    List<double> fitnesses = ga.getFitnesses();
                    List<double> parents = ga.getParentFitnesses();
                    List<string> snap = new List<string>();
                    for (int j = 0; j < fitnesses.Count; j++) snap.Add(fitnesses[j].ToString() + "," + parents[j].ToString());
                    RecordHistory.current.snapshot(snap);
                }
                //xoverimpnts[i] += BuyOrderChromosome.crossOverImprovements - totalximpnts;
                //totalximpnts = BuyOrderChromosome.crossOverImprovements;
                //mutimpnts[i] += BuyOrderChromosome.mutationImprovements - totalmimpnts;
                //totalmimpnts = BuyOrderChromosome.mutationImprovements;
                i++;
            }
            fitness.timesEvaluated = 0;
            GameController.recording = tempRecording;
            if (lastBestChromosome == null) throw new Exception("Null chromosome after evaluation " + i);
            lastBestChromosome.Evaluate(fitness);
            Move m = fitness.simulateMyTurn(lastBestChromosome, Board.current).PrevMove;
            RecordHistory.current.record(this + " took move " + m);


            List<BuyOrderChromosome> chromosomes = new List<BuyOrderChromosome>();
            for (int j = 0; j < ga.Count; j++)
            {
                chromosomes.Add((BuyOrderChromosome)ga[j]);
            }
            //foreach (BuyOrderChromosome p in chromosomes)
            //{
            //    RecordHistory.writeToFile("Chromosomes.txt", string.Format("{0:0.00}",p.Fitness) + "|" + p.depth + "|" + p.Value.String() + "|" + string.Format("{0:0.00}",p.parentFitness));
            //}
            //RecordHistory.writeToFile("Chromosomes.txt", "");
            CONSOLE.Overwrite(12, "XOver Impnts over gen.: " + xoverimpnts.String());
            CONSOLE.Overwrite(13, "Mut Impnts over gen.: " + mutimpnts.String());
            CONSOLE.Overwrite(14, "Crossover Improvements: " + BuyOrderChromosome.crossOverImprovements + " / " + BuyOrderChromosome.totalCrossOvers);
            CONSOLE.Overwrite(15, "Mutation Improvements: " + BuyOrderChromosome.mutationImprovements + " / " + BuyOrderChromosome.totalMutations);
        //    CONSOLE.Overwrite(16, "Number of Diverse Chromosomes: " + diversity(chromosomes));
            takeAction(m);
        }

        public override string ToString()
        {
            return name;
        }

        public int diversity(List<BuyOrderChromosome> cs)
        {
            cs.Sort();
            for (int i=0; i < cs.Count; i++)
            {
                BuyOrderChromosome toCompare = cs[i];
                for (int j= i+1; j < cs.Count; j++)
                {
                    if (toCompare.almostEqual(cs[j]))
                    {
                        cs.RemoveAt(j);
                        j--;
                    }
                }
            }
            return cs.Count;
        }

    }
}
