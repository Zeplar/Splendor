using System;
using System.Text;
using AForge.Genetic;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor.Strategize
{
    public class StrategizeChromosome : ChromosomeBase
    {
        /// <summary>
        /// Represents boardstate at each turn
        /// </summary> 
        protected Heuristic[] fn;

        public int len;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExactChromosome"/> class.
        /// </summary>
        /// <param name="len"></param>Chromosome's length in moves.
        public StrategizeChromosome(int len)
        {
            this.len = len;
            Generate();
        }

        protected StrategizeChromosome(StrategizeChromosome source)
        {
            //copy all properties
            fitness = source.fitness;
            parentFitness = source.parentFitness;
            this.len = source.len;
            fn = new Heuristic[source.fn.Length];
            source.fn.CopyTo(fn, 0);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(getFn);
            sb.Append("| Fitness: " + fitness);
            return sb.ToString();
        }

        /// <summary>
        /// Produces a scoring function that is the sum of all scoring functions in the chromosome.
        /// The sum of the scalars is always 1.
        /// </summary>
        public Heuristic getFn
        {
            get
            {
                Heuristic ret = fn[0];
                for (int i = 1; i < fn.Length; i++)
                {
                    ret += fn[i];
                }
                return ret;
            }
        }



        public override IChromosome Clone()
        {
            return new StrategizeChromosome(this);
        }

        public override IChromosome CreateNew()
        {
            return new StrategizeChromosome(10);
        }

        /// <summary>
        /// Crosses one type of Scoring Function
        /// </summary>
        /// <param name="other"></param>
        private void smallCrossover(StrategizeChromosome other)
        {
            int index = GameController.Random.Next(fn.Length);
            Heuristic temp = fn[index];
            fn[index] = other.fn[index];
            other.fn[index] = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private void largeCrossover(StrategizeChromosome other)
        {
            for (int i = 0; i < fn.Length; i++)
            {
                fn[i] += other.fn[i];
            }

        }

        public override void Crossover(IChromosome pair)
        {
            StrategizeChromosome other = (StrategizeChromosome)pair;
            if (GameController.Random.NextDouble() < .5)
            {
                smallCrossover(other);
            }
            else largeCrossover(other);
        }


        /// <summary>
        ///
        /// </summary>
        public override void Generate()
        {
            fn = new Heuristic[(Heuristic.dictionary.Count)];
            Heuristic.dictionary.Values.CopyTo(fn, 0);
            for (int i = 0; i < fn.Length; i++) fn[i].scalar = GameController.Random.NextDouble();
        }

        /// <summary>
        /// Scales one component by rand(0,2)
        /// </summary>
        public override void Mutate()
        {
            int i = GameController.Random.Next(fn.Length);
            fn[i].scalar *= 2 * GameController.Random.NextDouble();
        }

    }
}
