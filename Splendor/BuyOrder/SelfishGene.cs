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
        private int generations;
        private BuyOrderChromosome lastBestChromosome = null;
        public static Move predicted;
        private int evaluations = 0;


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
            bool tempRecording = GameController.recording;
            GameController.recording = false;
            lastBestChromosome = null;
            fitness.cards = Board.current.viewableCards.FindAll(x => x.Deck != Card.Decks.nobles);
            var ga = new Population(popSize, new BuyOrderChromosome(fitness.cards.Count), fitness, new RankSelection(), random);

        //    if (!predicted.Equals(Board.current.prevMove)) RecordHistory.record("!!! Prediction failed.");
            predicted = null;
            int i = 0;
            while (fitness.timesEvaluated < evaluations)
            {
                if (lastBestChromosome != null) ga.AddChromosome(lastBestChromosome);
                ga.RunEpoch();
                lastBestChromosome = ga.BestChromosome as BuyOrderChromosome;
                CONSOLE.Overwrite(6, "Generations " + i + " Evaluation " + fitness.timesEvaluated);
                RecordHistory.current.plot(i + "," + ga.FitnessMax + Environment.NewLine);
                if ((GameController.turn % 5 == 0) && (i == 0 || i == generations / 2 || i == generations-1))
                {
                    List<double> fitnesses = ga.getFitnesses();
                    List<double> parents = ga.getParentFitnesses();
                    List<string> snap = new List<string>();
                    for (int j = 0; j < fitnesses.Count; j++) snap.Add(fitnesses[j].ToString() + "," + parents[j].ToString());
                    RecordHistory.current.snapshot(snap);
                }
                i++;
            }
            fitness.timesEvaluated = 0;
            GameController.recording = tempRecording;
            if (lastBestChromosome == null) throw new Exception("Null chromosome after evaluation " + i);
            lastBestChromosome.Evaluate(fitness);
            Move m = fitness.simulateMyTurn(lastBestChromosome, Board.current).PrevMove;
            RecordHistory.current.record(this + " took move " + m);
            
            foreach (var l in BuyOrderChromosome.track_somes)
            {
                string s = l.ConvertAll(x => x.Value).String();
                string f = l.ConvertAll(x => x.Fitness).String();
                RecordHistory.current.writeToFile("track.txt", s);
                RecordHistory.current.writeToFile("track.txt", f);
            }
            
            //List<BuyOrderChromosome> chromosomes = new List<BuyOrderChromosome>(); 
            //for (int j = 0; j < ga.Count; j++)
            //{
            //    chromosomes.Add((BuyOrderChromosome)ga[j]);
            //}

            //chromosomes.Sort();
            //foreach (BuyOrderChromosome p in chromosomes)
            //{
            //    RecordHistory.writeToFile("Chromosomes.txt", string.Format("{0:0.00}",p.Fitness) + "|" + p.depth + "|" + p.Value.String() + "|" + string.Format("{0:0.00}",p.parentFitness));
            //}
            //RecordHistory.writeToFile("Chromosomes.txt", "");
            takeAction(m);
        }

        public override string ToString()
        {
            return name;
        }

    }
}
