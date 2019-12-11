using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Common
{
    public static class Extensions
    {
        private static IEnumerable<TElem> BestElements<TElem, TVal>(
            IEnumerable<TElem> src,
            Func<TElem, TVal> projection,
            int betterDirection) where TVal : IComparable<TVal>, new()
        {
            bool found = false;
            TVal max = new TVal();
            List<TElem> bestElems = new List<TElem>();
            foreach (TElem e in src)
            {
                TVal val = projection(e);
                int comparisonResult = val.CompareTo(max) * betterDirection;
                if (!found || comparisonResult > 0)
                {
                    found = true;
                    max = val;
                    bestElems = new List<TElem>();
                    bestElems.Add(e);
                }
                else if (comparisonResult == 0)
                {
                    bestElems.Add(e);
                }
            }
            return bestElems.AsEnumerable();
        }

        public static IEnumerable<TElem> MaximalElements<TElem, TVal>(this IEnumerable<TElem> src, Func<TElem, TVal> projection) where TVal : IComparable<TVal>, new()
        {
            return BestElements(src, projection, 1);
        }

        public static IEnumerable<TElem> MinimalElements<TElem, TVal>(this IEnumerable<TElem> src, Func<TElem, TVal> projection) where TVal : IComparable<TVal>, new()
        {
            return BestElements(src, projection, -1);
        }

        public static IEnumerable<KeyValuePair<int, T>> ZipWithIndex<T>(this IEnumerable<T> src)
        {
            int i = 0;
            foreach (T e in src)
            {
                yield return KeyValuePair.Create(i++, e);
            }
            yield break;
        }

        public static IEnumerable<KeyValuePair<BigInteger, T>> ZipWithBigIndex<T>(this IEnumerable<T> src)
        {
            BigInteger i = 0;
            foreach (T e in src)
            {
                yield return KeyValuePair.Create(i++, e);
            }
            yield break;
        }

        public static IEnumerable<List<TElem>> SplitWhen<TElem>(this IEnumerable<TElem> seq,
            Func<TElem, TElem, bool> splitPredicate)
        {
            List<TElem> prefix = new List<TElem>();
            foreach(TElem next in seq)
            {
                if (prefix.Count >= 1)
                {
                    if (splitPredicate(prefix.Last(), next))
                    {
                        yield return prefix;
                        prefix = new List<TElem>();
                    }
                }
                prefix.Add(next);
            }
            if (prefix.Any())
            {
                yield return prefix;
            }
            yield break;
        }

        public static IEnumerable<IEnumerable<T>> Slices<T>(this IEnumerable<T> seq, int sliceLength)
        {
            IEnumerable<T> rest = seq;
            while (rest.Any())
            {
                yield return rest.Take(sliceLength);
                rest = rest.Skip(sliceLength);
            }
            yield break;
        }

        // If the "array" is jagged, the result will be too, but
        // it will have a triangular shape
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> src)
        {
            List<IEnumerator<T>> srcEnumerators = src.Select(seq => seq.GetEnumerator()).ToList();
            bool done = false;
            while (!done)
            {
                Queue<T> heads = new Queue<T>();
                done = true;
                foreach (IEnumerator<T> enumerator in srcEnumerators)
                {
                    if (enumerator.MoveNext())
                    {
                        heads.Enqueue(enumerator.Current);
                        done = false;
                    }
                }
                if (heads.Any())
                {
                    yield return heads;
                }
            }
            yield break;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> src)
        {
            return src.SelectMany(part => part);
        }

        public static TVal GetOrElse<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal ifNotFound)
        {
            if (dict.TryGetValue(key, out TVal result))
            {
                return result;
            }
            else
            {
                return ifNotFound;
            }
        }

        public static void AddOrSet<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = val;
            }
            else
            {
                dict.Add(key, val);
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Dictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings)
        {
            return groupings.ToDictionary(g => g.Key, g => g.ToList());
        }

    }
}
