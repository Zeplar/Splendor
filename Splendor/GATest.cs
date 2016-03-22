//using System;
//using AForge.Genetic;
//using AForge;
//namespace Splendor
//{
//    public static class test
//    {
//        // define optimization function
//        public class UserFunction : OptimizationFunction1D
//        {
//            public UserFunction() :
//               base(new Range(0, 255))
//            { }

//            public override double OptimizationFunction(double x)
//            {
//                return Math.Cos(x / 23) * Math.Sin(x / 50) + 2;
//            }
//        }

//        public static void run()
//        {
//            UserFunction f = new UserFunction();
//            // create genetic population
//            Population population = new Population(40,
//                   new BinaryChromosome(32),
//                   f,
//                   new EliteSelection(),
//                   new ThreadSafeRandom());
//            for (int i = 0; i < 1; i++)
//            {
//                // run one epoch of the population
//                population.RunEpoch();
//            }
//            CONSOLE.Overwrite(15, "" + f.Translate((BinaryChromosome)population.BestChromosome));
//        }
//    }
//}
