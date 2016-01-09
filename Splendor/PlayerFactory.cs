﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendor
{
    public static class PlayerFactory
    {

        private static Dictionary<string, Func<string[], Player>> dict = new Dictionary<string, Func<string[], Player>>();
        public static Player CreateNew(string name, List<string> args)
        {
            string[] x = args.ToArray();
            return dict[name](x);
        }
        public static bool Exists(string name)
        {
            return dict.ContainsKey(name);
        }
        private static void Register(string name, Func<string[], Player> f)
        {
            dict.Add(name, f);
        }

        public static string listAll
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (string k in dict.Keys) s.Append(k + ", ");
                s.Remove(s.Length - 2, 2);
                return s.ToString();
            }
        }

        

        public static void Load()
        {
            Register("human", (x) => new Human());
            Register("greedy", x =>
            {
                return x.Length == 0 ? new Greedy() : new Greedy(ScoringMethods.parseScoringFunction(x, 0));
            });
            Register("player", (x) => new Player());
            Register("minimax", x =>
            {
                int i = int.Parse(x[0]);

                return new Minimax(i, ScoringMethods.parseScoringFunction(x, 1));
            });
            Register("exact", x => new Exact.ExactGene(ScoringMethods.parseScoringFunction(x,0)));
            Register("selfish", x =>
            {
                return x.Length > 2 ? new BuyOrder.SelfishGene(ScoringMethods.parseScoringFunction(x, 2), int.Parse(x[0]), int.Parse(x[1])) : new BuyOrder.SelfishGene(ScoringMethods.parseScoringFunction(x, 0));
            });
            Register("greedybuyer", x => new GreedyBuyer());

        }
    }
}
