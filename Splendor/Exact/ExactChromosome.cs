using System;
using System.Text;
using AForge.Genetic;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor.Exact
{
    public class ExactChromosome : ChromosomeBase
    {

        /// <summary>
        /// Probability of a large mutation instead of a small mutation
        /// </summary>
        public static float largeMutationRate = 0.3f;

        private static int xoimpnts = 0;
        private static int xos = 0;
        private static int muts = 0;
        private static int mutimpnts = 0;
        private static object Lock = new object();

        public static int mutationImprovements
        {
            get { return mutimpnts; }
            set
            {
                lock (Lock)
                {
                    mutimpnts = value;
                }
            }
        }
        public static int totalMutations { get { return muts; } set { lock (Lock) { muts = value; } } }

        /// <summary>
        /// Increments xoimpnts
        /// </summary>
        public static int crossOverImprovements
        {
            get { return xoimpnts; } set {  lock (Lock)
                { xoimpnts = value; }
            }
        }

        /// <summary>
        /// Increments xos
        /// </summary>
        public static int totalCrossOvers
        {
            get  {return xos; } set {lock (Lock)
                {
                    xos = value;
                }  }
        }


        /// <summary>
        /// Chromosome's length (#moves simulated)
        /// </summary>
        public int length;

        /// <summary>
        /// Represents boardstate at each turn
        /// </summary> 
        public uint[] boardState;
        public List<Move> moves;


        public int score = 0;

        static int ID = 0;
        private int id;
        /// <summary>
        /// Initializes a new instance of the <see cref="ExactChromosome"/> class.
        /// </summary>
        /// <param name="len"></param>Chromosome's length in moves.
        public ExactChromosome(int len)
        {
            this.length = len;
            this.boardState = new uint[len];
            Generate();
            id = ID;
            ID++;
        }

        protected ExactChromosome(ExactChromosome source)
        {
            //copy all properties
            fitness = source.fitness;
            length = source.length;
            boardState = (uint[])source.boardState.Clone();
            score = source.score;
            moves = new List<Move>();
            moves.AddRange(source.moves);
            id = source.id;
            parentFitness = source.parentFitness;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(boardState[0]);
            for (int i=1; i < length; i++)
            {
                sb.Append(' ');
                sb.Append(boardState[i]);
            }
            sb.Append("| Fitness: " + fitness);
            return sb.ToString();
        }



        public override IChromosome Clone()
        {
            return new ExactChromosome(this);
        }

        public override IChromosome CreateNew()
        {
            return new ExactChromosome(length);
        }

        public override void Crossover(IChromosome pair)
        {
            ExactChromosome other = (ExactChromosome)pair;
            for (int i=0; i < length; i++)
            {
                uint hash = boardState[i];
                for (int j=i+1; j < length; j++)
                {
                    if (hash != 0 && other.boardState[j] == hash)
                    {
                        CrossoverFrom(other, i, j);
                        totalCrossOvers++;
                        if (i == 0 && j == 0) CONSOLE.WriteLine("Trivial XOver.");
                        else CONSOLE.WriteLine("XOver at " + i + "," + j);
                        return;
                    }
                }
            }
        }
        
        private void CrossoverFrom(ExactChromosome other, int thisStart, int otherStart)
        {
            List<Move> temp = new List<Move>();
            for (int i=thisStart; i < moves.Count; i++)
            {
                temp.Add(moves[i]);
 //               if (moves[i]?.moveType == 2) break;
            }
            moves.RemoveRange(thisStart, temp.Count);
            for (int i= otherStart; i < other.moves.Count; i++)
            {
                moves.Add(other.moves[i]);
 //               if (other.moves[i]?.moveType == 2) break;
            }
            other.moves = other.moves.GetRange(0, otherStart);
            other.moves.InsertRange(otherStart, temp);

            moves = moves.GetRange(0, Math.Min(moves.Count, length));
            other.moves = other.moves.GetRange(0, Math.Min(other.moves.Count, length));
        }

        /// <summary>
        ///
        /// </summary>
        public override void Generate()
        {
            moves = new List<Move>();
        }

        /// <summary>
        /// Changes one move to null, so that it will be re-evaluated next generation.
        /// </summary>
        public override void Mutate()
        {
            //bool large = Splendor.random.NextDouble() < largeMutationRate;
            moves[GameController.Random.Next(moves.Count)] = null;
            totalMutations++;
            parentFitness = -parentFitness;
        }

        public override int GetHashCode()
        {
            return id;
        }

    }
}
