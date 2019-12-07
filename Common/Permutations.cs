using System.Collections.Generic;
using System.Linq;
using System;

namespace Common
{
    public static class Permutation
    {
        public static List<List<T>> Permutations<T>(List<T> items) where T : IEquatable<T>
        {
            if (items.Count == 0)
            {
                return new List<List<T>>() { new List<T>() };
            }
            else if (items.Count == 1)
            {
                return new List<List<T>>() { new List<T>() { items[0] } };
            }
            return items.SelectMany(item => {
                var permutationsOfTheRest = Permutations(items.Where(remainingItem => !remainingItem.Equals(item)).ToList());
                return permutationsOfTheRest
                    .Select<List<T>, List<T>>(
                        permutationOfTheRest => permutationOfTheRest.Concat(new List<T>() { item }).ToList())
                    .ToList();
            }).ToList();
        }
    }
}
