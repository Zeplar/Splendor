using System.Collections.Generic;
using System.Text;

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
                sb.Append(t.ToString());
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
