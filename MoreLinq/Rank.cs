namespace MoreLinq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class MoreEnumerable
    {
        /// <summary>
        /// Ranks each item in the sequence in descending ordering using a default comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of item in the sequence</typeparam>
        /// <param name="source">The sequence whose items will be ranked</param>
        /// <returns>A sequence of position integers representing the ranks of the corresponding items in the sequence</returns>
        public static IEnumerable<int> Rank<TSource>(this IEnumerable<TSource> source)
        {
            return source.RankBy(x => x);
        }

        /// <summary>
        /// Rank each item in the sequence using a caller-supplied comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <param name="source">The sequence of items to rank</param>
        /// <param name="comparer">A object that defines comparison semantics for the elements in the sequence</param>
        /// <returns>A sequence of position integers representing the ranks of the corresponding items in the sequence</returns>
        public static IEnumerable<int> Rank<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            return source.RankBy(x => x, comparer);
        }

        /// <summary>
        /// Ranks each item in the sequence in descending ordering by a specified key using a default comparer
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the key used to rank items in the sequence</typeparam>
        /// <param name="source">The sequence of items to rank</param>
        /// <param name="keySelector">A key selector function which returns the value by which to rank items in the sequence</param>
        /// <returns>A sequence of position integers representing the ranks of the corresponding items in the sequence</returns>
        public static IEnumerable<int> RankBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return RankBy(source, keySelector, null);
        }

        /// <summary>
        /// Ranks each item in a sequence using a specified key and a caller-supplied comparer
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the key used to rank items in the sequence</typeparam>
        /// <param name="source">The sequence of items to rank</param>
        /// <param name="keySelector">A key selector function which returns the value by which to rank items in the sequence</param>
        /// <param name="comparer">An object that defines the comparison semantics for keys used to rank items</param>
        /// <returns>A sequence of position integers representing the ranks of the corresponding items in the sequence</returns>
        public static IEnumerable<int> RankBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            return RankByImpl(source, keySelector, comparer ?? Comparer<TKey>.Default);
        }
        
        private static IEnumerable<int> RankByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            var rank = 0;
            var rankDictionary = new Dictionary<TSource, int>();
            source = source.ToArray(); // avoid enumerating source twice
            foreach (var item in source.Distinct().OrderByDescending(keySelector, comparer))
                rankDictionary.Add(item, ++rank);

            foreach (var item in source)
                yield return rankDictionary[item];
        }
    }
}