using System.Diagnostics;
using System;

namespace Splendor
{

    public struct Gem
    {
        private int a, b, c, d, e, gold;

        public Gem(int a, int b, int c, int d, int e, int g)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.gold = g;
        }


        public static Gem zero = new Gem(0, 0, 0, 0, 0, 0);
        public static Gem board = new Gem(4, 4, 4, 4, 4, 8);
        public static Gem selected = Gem.zero;
        public static Gem[] AllThree = { new Gem(0, 0, 1, 1, 1, 0), new Gem(0,1,0,1,1,0), new Gem(1,0,0,1,1,0),
                                         new Gem(0,1,1,1,0,0), new Gem(1,0,1,1,0,0), new Gem(1,1,0,1,0,0),
                                         new Gem(1,1,1,0,0,0), new Gem(1,0,1,0,1,0), new Gem(1,1,0,0,1,0), new Gem(0,1,0,0,1,0)};
        public static Gem[] AllTwo = { new Gem(2, 0, 0, 0, 0, 0), new Gem(0, 2, 0, 0, 0, 0), new Gem(0, 0, 2, 0, 0, 0), new Gem(0, 0, 0, 2, 0, 0), new Gem(0, 0, 0, 0, 2, 0) };

        public static void Reset()
        {
            selected = Gem.zero;
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
                        Trace.TraceError("Color index out of range");
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
                ret[i] = Math.Max(0, a[i] + b[i]);
            }
            return ret;
        }

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
        private Gem positive
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
                Trace.TraceError("Got negative gold gems somehow.");
            }
            return ret;
        }

        public static void tryTake()
        {
            Move x;
            if (selected.magnitude == 2)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (selected[i] == 2)
                    {
                        x = new Move.TAKE2(i);
                        if (x.isLegal())
                        {
                            x.takeAction();
                        }
                    }
                }
            }
            else if (selected.magnitude == 3)
            {
                x = new Move.TAKE3(selected);
                if (x.isLegal())
                {
                    x.takeAction();
                }
            }
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
                i += (int)Math.Pow(10, j) * this[j];
            }
            return i;
        }

    }



}
