﻿using System;
using System.Text;
using AForge.Genetic;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor.Genetic
{
    public class SplendorGene : ChromosomeBase
    {

        /// <summary>
        /// Probability of a large mutation instead of a small mutation
        /// </summary>
        public static float largeMutationRate = 0.5f;

        /// <summary>
        /// Chromosome's length (#moves simulated)
        /// </summary>
        public int length;

        /// <summary>
        /// Array of each move by type: 0,1,2,3 ==> TAKE2, TAKE3, BUY, RESERVE
        /// </summary>
        public byte[] major;

        /// <summary>
        /// Array of each move by index into #legal moves of that type
        /// </summary> 
        public byte[] minor;

        private List<Move> MOVES;
        public List<Move> moves
        {
            get
            {
                if (MOVES == null)
                {
                    updateMoves();
                }
                return MOVES;
            }
        }

        public int score = 0;

        public int[] this[int i]
        {
            get
            {
                return new int[] { major[i], minor[i] };
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplendorGene"/> class.
        /// </summary>
        /// <param name="len"></param>Chromosome's length in moves.
        public SplendorGene(int len)
        {
            this.length = len;
            this.major = new byte[len];
            this.minor = new byte[len];
            Generate();
        }

        protected SplendorGene(SplendorGene source)
        {
            //copy all properties
            fitness = source.fitness;
            length = source.length;
            major = (Byte[])source.major.Clone();
            minor = (Byte[])source.minor.Clone();
        }

        /// <summary>
        /// List all the moves described by this chromosome.
        /// </summary>
        public void updateMoves()
        {
            
            Board b = Board.current;
            byte moveValue;
            MOVES = new List<Move>();
            

            for (int i = 0; i < length; i++)
            {
                moveValue = minor[i];
                switch (major[i])
                {
                    case 0:
                        MOVES = Move.TAKE2.getLegalMoves().ConvertAll(x => (Move)x);
                        break;
                    case 1:
                        MOVES = Move.TAKE3.getLegalMoves().ConvertAll(x => (Move)x);
                        break;
                    case 2:
                        MOVES = Move.BUY.getLegalMoves().ConvertAll(x => (Move)x);
                        break;
                    case 3:
                        MOVES = Move.RESERVE.getLegalMoves().ConvertAll(x => (Move)x);
                        break;
                    default:
                        Debug.Fail("Invalid move type.");
                        return;
                }
                if (MOVES.Count > 0)
                {
                    moves.Add(MOVES[moveValue % MOVES.Count]);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(major[0]);
            sb.Append(':');
            sb.Append(minor[0]);
            for (int i=1; i < length; i++)
            {
                sb.Append(' ');
                sb.Append(major[i]);
                sb.Append(':');
                sb.Append(minor[i]);
            }
            sb.Append("| Fitness: " + fitness);
            return sb.ToString();
        }



        public override IChromosome Clone()
        {
            return new SplendorGene(this);
        }

        public override IChromosome CreateNew()
        {
            return new SplendorGene(length);
        }

        public override void Crossover(IChromosome pair)
        {
            SplendorGene p = (SplendorGene)pair;
            int crossOverPoint = Splendor.random.Next(length - 1) + 1;
            int crossOverLength = length - crossOverPoint;
            if (p != null && p.length == length)
            {
                byte[] temp = new byte[crossOverLength];

                Array.Copy(major, crossOverPoint, temp, 0, crossOverLength);
                Array.Copy(p.major, crossOverPoint, major, crossOverPoint, crossOverLength);
                Array.Copy(temp, 0, p.major, crossOverPoint, crossOverLength);

                Array.Copy(minor, crossOverPoint, temp, 0, crossOverLength);
                Array.Copy(p.minor, crossOverPoint, minor, crossOverPoint, crossOverLength);
                Array.Copy(temp, 0, p.minor, crossOverPoint, crossOverLength);

            }
        }

        /// <summary>
        /// Generates random values for the chromosome. MoveTypes are mod 4, but moveValues can be any byte.
        /// </summary>
        public override void Generate()
        {
            Splendor.random.NextBytes(major);
            Splendor.random.NextBytes(minor);
            for (int i=0; i < length; i++)
            {
                major[i] = (byte)(major[i] % 4);
            }

        }

        /// <summary>
        /// Changes one byte in moveTypes or moveValues depending on largeMutationRate.
        /// </summary>
        public override void Mutate()
        {
            bool large = Splendor.random.NextDouble() < largeMutationRate;
            int i = Splendor.random.Next(length);
            if (large)
            {
                major[i] = (byte)Splendor.random.Next(4);
                minor[i] = (byte)Splendor.random.Next();
            }
            else
            {
                minor[i] = (byte)Splendor.random.Next();
            }
        }
        
    }
}
