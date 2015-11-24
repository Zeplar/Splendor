using System;
using System.Text;
using AForge.Genetic;
using System.Collections.Generic;
using System.Diagnostics;

namespace Splendor.Genetic
{
    public class SplendorGene : ChromosomeBase
    {

        public Gem take3ref(Byte move)
        {
            int a = move & 1;
            int b = move & 2;
            int c = move & 4;
            int d = move & 8;
            int e = move & 16;
            return new Gem(a, b, c, d, e, 0);
        }

        public int take2ref(Byte move)
        {
            return move % 5;
        }

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
        public byte[] moveTypes;

        /// <summary>
        /// Array of each move by index into #legal moves of that type
        /// </summary> 
        public byte[] moveValues;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="SplendorGene"/> class.
        /// </summary>
        /// <param name="len"></param>Chromosome's length in moves.
        public SplendorGene(int len)
        {
            this.length = len;
            this.moveTypes = new byte[len];
            this.moveValues = new byte[len];
            Generate();
        }

        protected SplendorGene(SplendorGene source)
        {
            //copy all properties
            fitness = source.fitness;
            length = source.length;
            moveTypes = (Byte[])source.moveTypes.Clone();
            moveValues = (Byte[])source.moveValues.Clone();
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
                moveValue = moveValues[i];
                switch (moveTypes[i])
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
            sb.Append(moveTypes[0]);
            sb.Append(':');
            sb.Append(moveValues[0]);
            for (int i=1; i < length; i++)
            {
                sb.Append(' ');
                sb.Append(moveTypes[i]);
                sb.Append(':');
                sb.Append(moveValues[i]);
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

                Array.Copy(moveTypes, crossOverPoint, temp, 0, crossOverLength);
                Array.Copy(p.moveTypes, crossOverPoint, moveTypes, crossOverPoint, crossOverLength);
                Array.Copy(temp, 0, p.moveTypes, crossOverPoint, crossOverLength);

                Array.Copy(moveValues, crossOverPoint, temp, 0, crossOverLength);
                Array.Copy(p.moveValues, crossOverPoint, moveValues, crossOverPoint, crossOverLength);
                Array.Copy(temp, 0, p.moveValues, crossOverPoint, crossOverLength);

            }
        }

        /// <summary>
        /// Generates random values for the chromosome. MoveTypes are mod 4, but moveValues can be any byte.
        /// </summary>
        public override void Generate()
        {
            Splendor.random.NextBytes(moveTypes);
            Splendor.random.NextBytes(moveValues);
            for (int i=0; i < length; i++)
            {
                moveTypes[i] = (byte)(moveTypes[i] % 4);
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
                moveTypes[i] = (byte)Splendor.random.Next(4);
                moveValues[i] = (byte)Splendor.random.Next();
            }
            else
            {
                moveValues[i] = (byte)Splendor.random.Next();
            }
        }
        
    }
}
