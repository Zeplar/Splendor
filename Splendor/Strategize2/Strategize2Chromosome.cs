using System;
using System.Text;
using AForge.Genetic;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor.Strategize2
{
    public class Strategize2Chromosome : ChromosomeBase
    {
        /// <summary>
        /// Represents boardstate at each turn
        /// </summary> 
        internal List<Strategy> strategies;

        public int len;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactChromosome"/> class.
        /// </summary>
        /// <param name="len"></param>Chromosome's length in moves.
        public Strategize2Chromosome(int len)
        {
            this.len = len;
            Generate();
        }

        protected Strategize2Chromosome(Strategize2Chromosome source)
        {
            //copy all properties
            fitness = source.fitness;
            parentFitness = source.parentFitness;
            this.len = source.len;
            strategies = source.strategies.GetRange(0, source.strategies.Count);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("| Fitness: " + fitness);
            return sb.ToString();
        }



        public override IChromosome Clone()
        {
            return new Strategize2Chromosome(this);
        }

        public override IChromosome CreateNew()
        {
            return new Strategize2Chromosome(len);
        }


        /// <summary>
        /// Single-Point crossover of strategies
        /// </summary>
        /// <param name="pair"></param>
        public override void Crossover(IChromosome pair)
        {
            Strategize2Chromosome other = (Strategize2Chromosome)pair;

            int index = GameController.Random.Next(len);
            Strategy temp = strategies[index];
            strategies[index] = other.strategies[index];
            other.strategies[index] = temp;

        }


        /// <summary>
        /// Generates len strategies
        /// </summary>
        public override void Generate()
        {
            strategies = new List<Strategy>();
            for (int i=0; i < len; i++)
            {
                Strategy.type t = GameController.Random.Next(2) == 0 ? Strategy.type.DenyCard : Strategy.type.GetCard;

                List<Card> avail = Board.current.viewableCards.FindAll(x => x.Deck != Card.Decks.nobles);
                Card c = avail[GameController.Random.Next(avail.Count)];
                if (t == Strategy.type.DenyCard) strategies.Add(new Strategy.DenyCard(c));
                else strategies.Add(new Strategy.GetCard(c));
            }
        }

        /// <summary>
        /// Changes one strategy to a randomly generated strategy
        /// </summary>
        public override void Mutate()
        {
            int i = GameController.Random.Next(len);
            Strategy.type t = GameController.Random.Next(2) == 0 ? Strategy.type.DenyCard : Strategy.type.GetCard;
            Card c = Board.current.viewableCards[GameController.Random.Next(Board.current.viewableCards.Count)];
            if (t == Strategy.type.DenyCard) strategies.Add(new Strategy.DenyCard(c));
            else strategies[i] = (new Strategy.GetCard(c));
        }

    }
}
