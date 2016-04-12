using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splendor
{
    public static class PlayerFactory
    {

        private static Dictionary<string, Func<string[], Player>> dict = new Dictionary<string, Func<string[], Player>>();
        public static Player CreateNew(string[] args)
        {
            return dict[args[0]](args.Skip(1).ToArray());
        }
        public static bool Exists(string name)
        {
            return dict.ContainsKey(name);
        }
        private static void Register(string name, Func<string[], Player> f, params string[] requiredArgs)
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
            //Register("human", (x) => new Human());
            Register("greedy", Greedy.Create);
            Register("random", RandomSearch.Create);
            Register("player", (x) => new Player());
            Register("minimax", x =>
            {
                int i = int.Parse(x[0]);

                return new Minimax(i, Heuristic.parse(x, 1));
            }, "Depth");
            Register("exact", Exact.ExactGene.Create);
            Register("selfish", BuyOrder.SelfishGene.Create);
            Register("greedybuyer", x => new GreedyBuyer());

        }
    }
}
