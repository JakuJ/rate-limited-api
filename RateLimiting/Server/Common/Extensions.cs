using System.Collections.Generic;

namespace Server.Common
{
    /// <summary>
    /// A collection of helpful extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Zip the elements of an <see cref="IEnumerable{T}"/> with their indices.
        /// </summary>
        /// <param name="input">The <see cref="IEnumerable{T}"/> to enumerate.</param>
        /// <typeparam name="T">Type of the elements of the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <returns>An enumeration of pairs (i, x) where i is the index of element x in the original <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<(int Index, T Value)> Enumerate<T>(this IEnumerable<T> input)
        {
            var i = 0;
            foreach (T t in input)
            {
                yield return (i++, t);
            }
        }
    }
}
