using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Splendor
{

    /// <summary>
    /// Scores the board from the perspective of the generating player	
    /// </summary>
    public static class ScoringMethods
    {

        private static string save = System.IO.Directory.GetCurrentDirectory() + @"\save.txt";

        public static Dictionary<string,Function> dictionary = new Dictionary<string, Function>();

        public static void register()
        { 
            dictionary.Add("lead", Lead);
            dictionary.Add("points", Points);
            dictionary.Add("winloss", WinLoss);
            dictionary.Add("prestige", Prestige);
            //dictionary.Add("legalbuys", LegalBuys);
            dictionary.Add("turn", turn);
            dictionary.Add("gems", Gems);
            dictionary.Add("nobles", DistanceFromNobles);
            Load();

            //Check how many losers thought they were winners
            //Check where losses occur-- is it always a card draw?
            //
        }

        public static void Save(string name, Function fn)
        {
            System.IO.File.AppendAllText(save, name + ":" + fn.ToString() + "\n");
        }
        private static void Load()
        {
            string[] lines = System.IO.File.ReadAllLines(save);
            string[] split, descr;
            foreach (string line in lines)
            {
                split = line.Split(':');
                descr = split[1].Replace("(","( ").Replace(")"," )").ToLower().Split();
                Function fn = parse(descr);
                fn.description = split[0];
                dictionary.Add(split[0], fn);
            }
        }

        public static string listAll
        {
            get
            {
                StringBuilder s = new StringBuilder();
                foreach (string k in dictionary.Keys) s.Append(k + ", ");
                return s.ToString();
            }
        }

        public struct Function
        {
            public double scalar;
            private Func<Board, double> fn;
            public string description;
            public bool perMove;

            public double evaluate(Board b)
            {
                try
                {
                    return scalar * fn(b);
                }
                catch
                {
                    throw new Exception("Something happened during fitness evaluation.");
                }
            }
            public Function(Func<Board, double> fn, string description)
            {
                this.fn = fn;
                this.description = description;
                scalar = 1;
                perMove = true;
            }

            public Function(double i)
            {
                scalar = i;
                fn = null;
                description = "";
                perMove = true;
            }

            public Function adjustScalar(double scalar)
            {
                var temp = this;
                temp.scalar = scalar;
                return temp;
            }

            public static Function allEval2 = (WinLoss * new Function(100)) + (Points * new Function(1.5)) + Prestige + Gems + DistanceFromNobles * (new Function(2));

            public static Function operator +(Function a, Function b)
            {
                if (a.description.Equals(b.description) || b.description == "")
                {
                    return a.adjustScalar(a.scalar + b.scalar);
                }
                if (a.description == "")
                {
                    return b.adjustScalar(a.scalar + b.scalar);
                }
                return new Function(bd => a.evaluate(bd) + b.evaluate(bd), "(" + a + " + " + b +")");
            }
            public static Function operator -(Function a, Function b)
            {
                if (a.description.Equals(b.description) || b.description == "")
                {
                    return a.adjustScalar(a.scalar - b.scalar);
                }
                if (a.description == "")
                {
                    return b.adjustScalar(a.scalar - b.scalar);
                }
                return new Function(bd => a.evaluate(bd) - b.evaluate(bd), "(" + a + " - " + b + ")");
            }
            public static Function operator *(Function a, Function b)
            {
                if (a.description.Equals(b.description) || b.description == "")
                {
                    return a.adjustScalar(a.scalar * b.scalar);
                }
                if (a.description == "")
                {
                    return b.adjustScalar(a.scalar * b.scalar);
                }
                return new Function(bd => a.evaluate(bd) * b.evaluate(bd), a + " * " + b);
            }
            public static Function operator /(Function a, Function b)
            {
                if (a.description.Equals(b.description) || b.description == "")
                {
                    return a.adjustScalar(a.scalar / b.scalar);
                }
                if (a.description == "")
                {
                    return b.adjustScalar(a.scalar / b.scalar);
                }
                return new Function(bd => a.evaluate(bd) / b.evaluate(bd), a + " / " + b);
            }

            public Function delta()
            {
                var fun = this.fn;
                return new Function(bd => fun(bd) - fun(bd.PrevBoard), "delta- " + description);
            }

            public override string ToString()
            {
                if (scalar == 1) return description;
                return "(" + scalar + description + ")";
            }
            public Function operate(string op, Function rhs)
            {
                switch (op)
                {
                    case "+":
                        return this + rhs;
                    case "-":
                        return this - rhs;
                    case "*":
                        return this * rhs;
                    case "/":
                        return this / rhs;
                    case "delta":
                        return this.delta();
                    default:
                        throw new FormatException("Invalid operator");
                }
            }
        }

        public static Function turn = new Function(bd => bd.Turn, "Turn");

        private static Function score = new Function(b => b.notCurrentPlayer.points, "Score");

        public static Function colors(Gem c)
        {
            Func<Board, double> fn = b =>
             {
                 Gem deltaGems = (b.notCurrentPlayer.Gems - b.PrevBoard.currentPlayer.Gems);
                 return (c - (c - deltaGems).positive).magnitude;
             };
            return new Function(fn, "colors: " + c);
        }

        /// <summary>
        /// Scores difference in points.
        /// </summary>
        public static Function Lead = new Function(bd => bd.notCurrentPlayer.points - bd.currentPlayer.points, "Lead");

        public static Function HasCard(Card id)
        {
            return new Function(bd => bd.notCurrentPlayer.Field.Contains(id) ? 1 : 0, "HasCard " + id);
        }

        public static Function DenyCard(Card id)
        {
            return new Function(bd => bd.currentPlayer.Field.Contains(id) ? -1 : 0, "DenyCard " + id);
        }

        /// <summary>
        /// Scores 1 for a win, -1 for a loss, else zero. Tiebreaks on prestige.
        /// </summary>
        public static Function WinLoss =
            new Function(bd =>
                {
                    if (bd.gameOver)
                    {
                        if (bd.currentPlayer.points < 15 && bd.notCurrentPlayer.points < 15) return 0;
                        if (bd.notCurrentPlayer.points < bd.currentPlayer.points)
                        {
                            return -1;
                        }
                        if (bd.notCurrentPlayer.points > bd.currentPlayer.points)
                        {
                            return 1;
                        }
                        return (bd.notCurrentPlayer.Field.Count < bd.currentPlayer.Field.Count) ? 1 : -1;

                    }
                    return 0;
                }, "WinLoss");

        public static Function Gems = new Function(bd => bd.PrevMove?.moveType == Move.Type.RESERVE ? 1 : bd.PrevMove?.moveType == Move.Type.BUY ? 0
                                                         : bd.PrevMove == null ? 0 : bd.notCurrentPlayer.Gems.magnitude - bd.PrevBoard.currentPlayer.Gems.magnitude, "Gems");

        /// <summary>
        /// Scores only points.
        /// </summary>
        public static Function Points = new Function(bd => bd.PrevMove?.moveType == Move.Type.BUY ? bd.notCurrentPlayer.points - bd.PrevBoard.currentPlayer.points : 0, "Points");

        /// <summary>
        /// Scores only prestige.
        /// </summary>
        public static Function Prestige = new Function(bd => bd.PrevMove?.moveType == Move.Type.BUY ? 1 : 0, "Prestige");

        public static Function DistanceFromNobles = new Function(bd => bd.PrevMove?.moveType == Move.Type.BUY ? _DistanceFromNobles(bd.PrevBoard,true) - _DistanceFromNobles(bd, false) : 0, "Nobles");

        private static double _DistanceFromNobles(Board b, bool current)
        {
            Player p = current ? b.currentPlayer : b.notCurrentPlayer;
            double i = 0;
            foreach (Card c in b.viewableCards.FindAll(x => x.Deck == Card.Decks.nobles))
            {
                i += (c.Cost - p.discount).positive.magnitude;
            }
            return i;
        }

        /// <summary>
        /// Scores number of legal Buys and Reserves available.
        /// Takes may be bad since trades vastly outnumber pure takes.
        /// </summary>
        //public static Function LegalBuys { get { return new Function(bd => bd.legalMoves.Count(x => x.moveType == Move.Type.BUY),"LegalBuys"); } }

        private static int precedence(string op)
        {
            switch (op)
            {
                case "*":
                    return 2;
                case "/":
                    return 2;
                case "+":
                    return 1;
                case "-":
                    return 1;
                default:
                    return -1;
            }
        }

        public static Function parse(IEnumerable<string> exp)
        {
            return eval_postfix(convertToPostfix(exp));
        }
        public static Function parse(IEnumerable<string> exp, int skip)
        {
            return eval_postfix(convertToPostfix(exp.Skip(skip)));
        }

        private static Function eval_postfix(Queue<string> exp)
        {
            Stack<Function> stack = new Stack<Function>();
            foreach (string s in exp)
            {
                if (precedence(s) > 0)
                {
                    Function temp = stack.Pop();
                    Function ret = stack.Pop().operate(s, temp);
                    stack.Push(ret);
                }
                else
                {
                    double i;
                    if (double.TryParse(s, out i)) stack.Push(new Function(i));
                    else
                    {
                        if (!dictionary.ContainsKey(s)) throw new FormatException(s + " is not in the dictionary.");
                        stack.Push(dictionary[s]);
                    }
                }
            }
            if (stack.Count != 1) throw new FormatException("Too many functions on the stack: " + stack.ToList().String());
            return stack.Pop();
        }

        private static Queue<string> convertToPostfix(IEnumerable<string> exp)
        {
            Queue<string> output = new Queue<string>();
            Stack<string> stack = new Stack<string>();
            foreach (string s in exp)
            {
                if (s == "(") stack.Push(s);
                else if (s == ")")
                {
                    while (stack.Peek() != "(") output.Enqueue(stack.Pop());
                    stack.Pop();
                }
                else if (precedence(s) > 0)
                {
                    while (stack.Count > 0 && precedence(s) <= precedence(stack.Peek()))
                    {
                        output.Enqueue(stack.Pop());
                    }
                    stack.Push(s);
                }
                else output.Enqueue(s);
            }
            foreach (string s in stack)
            {
                if (s == ")" || s == "(") throw new FormatException("Mismatched parenthesis");
                output.Enqueue(s);
            }
            return output;
        }

        public static void testScoringMethods(Board bd)
        {
            Function a = parse(new List<string>("1 + 2 / 3".Split()));
            Function b = parse(new List<string>("( 1 + 2 ) / 3".Split()));
            Function c = parse(new List<string>("1 + ( 2 / 3 )".Split()));

            Debug.Assert(a.evaluate(bd) == c.evaluate(bd));
            Debug.Assert(a.evaluate(bd) != b.evaluate(bd));
        }
    }

}