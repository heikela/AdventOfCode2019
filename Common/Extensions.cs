﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Common
{
    public static class Extensions
    {
        public static IEnumerable<TElem> MaximalElements<TElem, TVal>(this IEnumerable<TElem> src, Func<TElem, TVal> projection) where TVal : IComparable<TVal>, new()
        {
            bool found = false;
            TVal max = new TVal();
            List<TElem> bestElems = new List<TElem>();
            foreach (TElem e in src)
            {
                TVal val = projection(e);
                if (!found || val.CompareTo(max) > 0)
                {
                    found = true;
                    max = val;
                    bestElems = new List<TElem>();
                    bestElems.Add(e);
                }
                else if (val.CompareTo(max) == 0)
                {
                    bestElems.Add(e);
                }
            }
            return bestElems.AsEnumerable();
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
    }
}