using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    /// <summary>
    /// A collection of helpful functional extension methods missing from LINQ.
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
            => input.Select((p, i) => (i, p));

        /// <summary>
        /// Combine two collections by applying a function to elements at corresponding indices.
        /// </summary>
        /// <param name="input">First collection.</param>
        /// <param name="other">Second collection.</param>
        /// <param name="func">A function for combining the elements.</param>
        /// <typeparam name="T1">Type of the elements of the first collection.</typeparam>
        /// <typeparam name="T2">Type of the elements of the second collection.</typeparam>
        /// <typeparam name="TOut">Type of the elements of the result of the zip.</typeparam>
        /// <returns>A collection of <typeparamref name="TOut"/>.</returns>
        public static IEnumerable<TOut> ZipWith<T1, T2, TOut>(
            this IEnumerable<T1> input,
            IEnumerable<T2> other,
            Func<T1, T2, TOut> func)
            => input.Zip(other).Select(p => func(p.First, p.Second));
    }
}
