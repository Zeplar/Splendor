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


        //int totalximpnts = 0;
        //int totalmimpnts = 0;
        //int[] xoverimpnts = new int[5];
        //int[] mutimpnts = new int[5];
        //int diverseChromosomes = 0;
        //int totalChromosomes = 0;

        //private void setData(int i)
        //{
        //    if (xoverimpnts.Length <= i)
        //    {
        //        Array.Resize(ref xoverimpnts, i + 1);
        //        Array.Resize(ref mutimpnts, i + 1);
        //    }
        //    xoverimpnts[i] += BuyOrderChromosome.crossOverImprovements - totalximpnts;
        //    totalximpnts = BuyOrderChromosome.crossOverImprovements;
        //    mutimpnts[i] += BuyOrderChromosome.mutationImprovements - totalmimpnts;
        //    totalmimpnts = BuyOrderChromosome.mutationImprovements;
        //}


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
            Board.useDictionary = true;
            bool tempRecording = GameController.recording;
            GameController.recording = false;
            fitness.cards = Board.current.viewableCards.FindAll(x => x.Deck != Card.Decks.nobles);
            var ga = new Population(popSize, new BuyOrderChromosome(2*fitness.cards.Count), fitness, new RankSelection(), random);
            lastBestChromosome = ga.population[0] as BuyOrderChromosome;
            ga.CrossoverRate = 0.5;

       //     if (!predicted.Equals(Board.current.PrevMove)) RecordHistory.current.record("!!! Prediction failed.");
       //     predicted = null;
            int i = 0;
            turnTimer.Restart();
            while (turnTimer.Elapsed < Board.current.notCurrentPlayer.turnTimer.Elapsed && i < 40)  //(fitness.timesEvaluated < evaluations)
            {
                ga.RunEpoch();
                ga.AddChromosome(lastBestChromosome);
                if (ga.BestChromosome.Fitness > lastBestChromosome.Fitness) lastBestChromosome = ga.BestChromosome as BuyOrderChromosome;
                if (i % 6 == 0) Board.current.ResetDictionary();
                CONSOLE.Overwrite(6, "Generations " + i + " Evaluation " + fitness.timesEvaluated);
                RecordHistory.current.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                if ((GameController.turn % 3 == 0) && (i < 4)) takeSnapshot(ga);
                //setData(i);
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
            //List<BuyOrderChromosome> chromosomes = BuyOrderChromosome.diversify(ga.population);
            //CONSOLE.Overwrite(12, "XOver Impnts over gen.: " + xoverimpnts.String());
            //CONSOLE.Overwrite(13, "Mut Impnts over gen.: " + mutimpnts.String());
            //CONSOLE.Overwrite(14, "Crossover Improvements: " + BuyOrderChromosome.crossOverImprovements + " / " + BuyOrderChromosome.totalCrossOvers);
            //CONSOLE.Overwrite(15, "Mutation Improvements: " + BuyOrderChromosome.mutationImprovements + " / " + BuyOrderChromosome.totalMutations);
            //CONSOLE.Overwrite(16, "Number of Diverse Chromosomes: " + chromosomes.Count);

            //totalChromosomes += ga.population.Count;
            //diverseChromosomes += chromosomes.Count;
            //CONSOLE.Overwrite(17, "Total diverse/chromosomes: " + ((double)diverseChromosomes / totalChromosomes));
            //for (int j = 0; j < chromosomes.Count; j++)
            //{
            //    CONSOLE.Overwrite(18 + j, chromosomes[j].Value.String() + "   depth: " +chromosomes[j].depth + "    fitness: " + chromosomes[j].Fitness);
            //}
            takeAction(m);
            Board.useDictionary = false;
        }

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Records the population fitness data in /snapshot_x.csv
        /// </summary>
        /// <param name="ga"></param>
        private void takeSnapshot(Population ga)
        {
            List<double> fitnesses = ga.getFitnesses();
            List<double> parents = ga.getParentFitnesses();
            List<string> snap = new List<string>();
            for (int j = 0; j < fitnesses.Count; j++) snap.Add(fitnesses[j].ToString() + "," + parents[j].ToString());
            RecordHistory.current.snapshot(snap);
        }



    }
}
