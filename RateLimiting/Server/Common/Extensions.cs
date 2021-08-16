using System;
using System.Collections.Generic;

namespace Server.Common
{
    public static class Extensions
    {
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> input, Func<U, T, U> next, U state)
        {
            yield return state;
            foreach (T item in input)
            {
                state = next(state, item);
                yield return state;
            }
        }

        public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> input)
        {
            var i = 0;
            foreach (T t in input)
            {
                yield return (i++, t);
            }
        }
    }
}