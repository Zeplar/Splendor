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
                int j = GameController.random.Next(i, list.Count);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
        public static int addUp(this List<Card> list)
        {
            int ret = 0;
            for (int i=0; i < list.Count; i++)
            {
                ret += list[i].id;
            }
            return ret;
        }

        public static string String<T>(this IList<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in list)
            {
                sb.Append(t.ToString());
            }
            return sb.ToString();
        } 

        public static string String(this List<Card> list)
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
