using System;
using System.Linq;
using System.Collections.Generic;
using static Common.Extensions;

namespace Day04
{
    class Program
    {
        static bool Good1(List<int> digits)
        {
            IEnumerable<List<int>> adjacents = digits.SplitWhen((prev, next) => prev != next);
            IEnumerable<List<int>> nonDecreasing = digits.SplitWhen((prev, next) => prev > next);
            return (adjacents.Any(group => group.Count >= 2) && nonDecreasing.Count() == 1);
        }

        static bool Good2(List<int> digits)
        {
            IEnumerable<List<int>> adjacents = digits.SplitWhen((prev, next) => prev != next);
            IEnumerable<List<int>> nonDecreasing = digits.SplitWhen((prev, next) => prev > next);
            return (adjacents.Any(group => group.Count == 2) && nonDecreasing.Count() == 1);
        }

        static List<int> NumberToDigits(int n)
        {
            return n.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToList();
        }

        static void Main(string[] args)
        {
            IEnumerable<int> candidates = Enumerable.Range(158126, 624574 - 158126 + 1);
            IEnumerable<List<int>> unpackedCandidates = candidates.Select(NumberToDigits);
            Console.WriteLine($"Part1: {unpackedCandidates.Count(Good1)} good passwords.");
            Console.WriteLine($"Part1: {unpackedCandidates.Count(Good2)} good passwords.");
        }
    }
}
