using System.Collections.Generic;
using System.Linq;
using System;

namespace Common
{
    public static class Permutation
    {
        public static List<List<int>> PermutationsOfRange(int n)
        {
            List<int> items = Enumerable.Range(0, n).ToList();
            return PermutationsOfIntsWithoutDuplicates(items);
        }

        public static List<List<T>> Permutations<T>(List<T> items)
        {
            return PermutationsOfRange(items.Count)
                .Select(perm => perm.Select(i => items[i]).ToList())
                .ToList();
        }

        private static List<List<int>> PermutationsOfIntsWithoutDuplicates(List<int> items)
        {
            if (items.Count == 0)
            {
                return new List<List<int>>() { new List<int>() };
            }
            else if (items.Count == 1)
            {
                return new List<List<int>>() { new List<int>() { items[0] } };
            }
            return items.SelectMany(item => {
                var permutationsOfTheRest = PermutationsOfIntsWithoutDuplicates(items.Where(remainingItem => remainingItem != item).ToList());
                return permutationsOfTheRest
                    .Select(
                        permutationOfTheRest => permutationOfTheRest.Concat(new List<int>() { item }).ToList())
                    .ToList();
            }).ToList();
        }
    }
}
