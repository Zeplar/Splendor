using System.Diagnostics;
using System;
using CH.Combinatorics;
using System.Collections.Generic;

namespace Splendor
{

    public struct Gem
    {
        private int a, b, c, d, e, gold;

        /// <summary>
        /// The number of gems the board starts with.
        /// 2P: 4  
        /// 3P: 5
        /// 4P: 7
        /// </summary>
        public static Gem StartingGems;

        public static Dictionary<char, int> colors = new Dictionary<char, int>();

        public Gem(int a, int b, int c, int d, int e, int g)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.gold = g;
        }

        public static Gem FromJson(Newtonsoft.Json.JsonTextReader reader)
        {
            int[] g = new int[6];
            for (int i=0; i < 6; i++)
            {
                Console.WriteLine(reader.Value);
                g[i] = (int)reader.ReadAsInt32();
            }
            return new Gem(g);
        }

        public Gem(int[] gemArray)
        {
            Debug.Assert(gemArray.Length < 7);
            a = gemArray[0];
            b = gemArray[1];
            c = gemArray[2];
            d = gemArray[3];
            e = gemArray[4];
            gold = gemArray.Length == 6 ? gemArray[5] : 0;
        }

        public Gem(IEnumerable<byte> genumerable)
        {

            List<byte> gemArray = new List<byte>(genumerable);
            Debug.Assert(gemArray.Count < 7);
            a = gemArray[0];
            b = gemArray[1];
            c = gemArray[2];
            d = gemArray[3];
            e = gemArray[4];
            gold = gemArray.Count == 6 ? gemArray[5] : 0;
        }

        private static List<Gem> generateFrom(params int[] generator)
        {
            Debug.Assert(generator.Length == 5);
            List<Gem> ret = new List<Gem>();
            foreach (IEnumerable<int> i in generator.Permute())
            {
                ret.Add(new Gem(new List<int>(i).ToArray()));
            }
            for (int i = 0; i < ret.Count; i++)
            {
                for (int j = ret.Count-1; j > i; j--)
                {
                    if (ret[j] == ret[i]) ret.RemoveAt(j);
                }
            }
            return ret;
        }

        private static List<Gem> AllThrees()
        {
            List<Gem> a = generateFrom(1, 1, 1, -1, 0);
            List<Gem> b = generateFrom(1, 1, 1, -1, -1);
            List<Gem> c = generateFrom(1, 1, 1, -2, 0);
            List<Gem> d = generateFrom(1, 1, 1, -2, -1);
            List<Gem> e = generateFrom(1, 1, 1, -3, 0);
            List<Gem> f = generateFrom(1, 1, -1, -1, 0);
            List<Gem> g = generateFrom(1, -1, 0, 0, 0);

            a.AddRange(b); a.AddRange(c); a.AddRange(d); a.AddRange(e); a.AddRange(f); a.AddRange(g);
            return a;
        }

        private static List<Gem> AllTwos()
        {
            List<Gem> a = generateFrom(2, -1, 0, 0, 0);
            List<Gem> b = generateFrom(2, -1, -1, 0, 0);
            List<Gem> c = generateFrom(2, -2, 0, 0, 0);
            a.AddRange(b); a.AddRange(c);
            return a;
    }


        public static Gem zero = new Gem(0, 0, 0, 0, 0, 0);
        public static Gem board = new Gem(4, 4, 4, 4, 4, 8);
        public static Gem selected = Gem.zero;
        public static List<Gem> ThreeNetThree = generateFrom(1, 1, 1, 0, 0);
        public static List<Gem> ExchangeThree = AllThrees();
        public static List<Gem> TwoNetTwo = generateFrom(2, 0, 0, 0, 0);
        public static List<Gem> ExchangeTwo = AllTwos();

        public static void Reset()
        {
            selected = Gem.zero;
            colors['w'] = 0;
            colors['u'] = 1;
            colors['b'] = 2;
            colors['r'] = 3;
            colors['g'] = 4;
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    case 2:
                        return c;
                    case 3:
                        return d;
                    case 4:
                        return e;
                    case 5:
                        return gold;
                    default:
                        Debug.Fail("Color index out of range");
                        return 0;
                }
            }
                set
                {
                    switch (index)
                    {
                        case 0:
                            a = value;
                            break;
                        case 1:
                            b = value;
                            break;
                        case 2:
                            c = value;
                            break;
                        case 3:
                            d = value;
                            break;
                        case 4:
                            e = value;
                            break;
                        case 5:
                            gold = value;
                            break;
                        default:
                            Trace.TraceError("Color index out of range");
                            break;
                    }
                }
        }

        public override string ToString()
        {
            if (this[5] == 0)
            {
                return string.Format("<{0}{1}{2}{3}{4}>", this[0], this[1], this[2], this[3], this[4]);
            }
            return string.Format("<{0}{1}{2}{3}{4}{5}>", this[0], this[1], this[2], this[3], this[4], this[5]);
        }



   
        public static Gem operator +(Gem a, Gem b)
        {
            Gem ret = new Gem();
            int i;
            for (i = 0; i < 6; i++)
            {
                ret[i] = a[i] + b[i];
            }
            return ret;
        }


        /// <summary>
        /// Standard vector subtraction.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Gem operator -(Gem a, Gem b)
        {
            Gem ret = new Gem();
            int i;
            for (i = 0; i < 6; i++)
            {
                ret[i] = a[i] - b[i];
            }
            return ret;
        }


        /// <summary>
        /// Returns True if (a) could buy (b)-- does not count gold gems in (b)   	
        /// </summary>
        public static bool operator >=(Gem a, Gem b)
        {
            Gem dif = a - b;
            int x = 0;
            int i = 0;
            for (; i < 5; i++)
            {
                if (dif[i] < 0)
                {
                    x -= dif[i];
                }
            }
            return (a.gold >= x);
        }

        public static bool operator <=(Gem a, Gem b)
        {
            Trace.TraceError("<= is not a valid operation on Gems");
            return false;
        }

        /// <summary>
        /// Returns True if (a) is strictly greater than or equal to (b) -- not counting gold gems  	
        /// </summary>
        public static bool operator >(Gem a, Gem b)
        {
            int i = 0;
            for (; i < 5; i++)
            {
                if (a[i] < b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator <(Gem a, Gem b)
        {
           Trace.TraceError("< is not a valid operation on Gems");
            return false;
        }

        public static bool operator ==(Gem a, Gem b)
        {
            bool x = true;
            for (int i = 0; i < 6; i++)
            {
                x &= a[i] == b[i];
            }
            return x;
        }
        public static bool operator !=(Gem a, Gem b)
        {
            return !(a == b);
        }

        public int magnitude
        {
            get
            {
                return this[0] + this[1] + this[2] + this[3] + this[4] + this[5];
            }
        }

        /// <summary>
        /// Adds up all the negative gems. Returns a nonpositive number.
        /// </summary>
        public int deficit
        {
            get
            {
                int i = 0;
                int j = 0;
                for (; i < 5; i++)
                {
                    if (this[i] < 0)
                    {
                        j -= this[i];
                    }
                }
                return j;
            }
        }

        /// <summary>
        /// Returns the gem with all colors nonnegative.
        /// </summary>
        public Gem positive
        {
            get
            {
                Gem ret = this;
                for (int i = 0; i < 5; i++)
                {
                    ret[i] = Math.Max(0, ret[i]);
                }
                return ret;
            }
        }

        public Gem requiredToBuy(Gem target)
        {
            return (target - this).positive;
        }

        /// <summary>
        /// Returns the result of spending g gems, gold included.
        /// </summary>
        public Gem takeaway(Gem gem)
        {
            Gem g = gem.positive;
            Gem ret = this;
            for (int i = 0; i < 5; i++)
            {
                if (ret[i] < g[i])
                {
                    ret.gold -= g[i] - ret[i];
                    ret[i] = 0;
                }
                else
                {
                    ret[i] -= Math.Max(0, g[i]);
                }
            }
            if (ret.gold < 0)
            {
                throw new InvalidOperationException("Got negative gold gems somehow.");
            }
            return ret;
        }


        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof (Gem))
            {
                return false;
            }
            return this == (Gem)obj;

        }

        public override int GetHashCode()
        {
            int i = 0;
            for (int j=0; j < 6; j++)
            {
                i += (1<< (2*j)) * this[j];
            }
            return i;
        }

    }



}
