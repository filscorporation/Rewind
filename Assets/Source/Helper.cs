using System;
using System.Collections.Generic;
using Random = System.Random;

namespace Source
{
    public static class Helper
    {
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random(DateTime.Now.Millisecond);

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}