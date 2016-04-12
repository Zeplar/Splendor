using System.Collections.Generic;
using System.Text;
using System;

namespace Splendor
{
    public static class Extensions
    {
        public static void shuffle<T>(this IList<T> list)
        {
            int i = 0;
            for (; i < list.Count; i++)
            {
                int j = GameController.Random.Next(i, list.Count);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static void times(this double[] array, double x)
        {
            for (int i = 0; i < array.Length; i++) array[i] *= x;
        }

        /// <summary>
        /// Returns an array containing only one object for each equivalance class
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="equality"></param>
        /// <returns></returns>
        public static Array Set(this Array arr, Func<object, object, bool> equality = null)
        {
            if (equality == null) { equality = Equals; }
            List<object> ret = new List<object>();
            foreach (object c in arr)
            {
                if (!ret.Contains(c)) ret.Add(c);
            }
            return ret.ToArray();
        }

        public static int addUp(this IEnumerable<Card> list)
        {
            int ret = 0;
            foreach (Card i in list)
            {
                ret += i.id;
            }
            return ret;
        }

        public static string String<T>(this IEnumerable<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in list)
            {
                sb.Append(t.ToString() + ",");
            }
            return sb.ToString();
        } 

        public static string String(this IEnumerable<Card> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Card c in list)
            {
                sb.Append(c.ToString() + ", ");
            }
            return sb.ToString();
        }

    }
}
