using System;
using Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day16
{
    class Program
    {
        public static IEnumerable<TResult> Scan<TResult, T>(IEnumerable<T> source, TResult initial, Func<TResult, T, TResult> aggregator)
        {
            TResult currentResult = initial;
            foreach (T element in source)
            {
                currentResult = aggregator(currentResult, element);
                yield return currentResult;
            }
            yield break;
        }

        static int multiplier(int outpos, int inpos)
        {
            int index = (inpos + 1) / outpos;
            switch (index % 4)
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return 0;
                case 3: return -1;
                default: throw new Exception("This should be unreachable");
            }
        }

        static int FFTconvolution(List<int> partialSums, int pos)
        {
            int repeatLength = (pos + 1);
            int start = repeatLength - 1;
            int sign = 1;
            long sum = 0;
            while (start < partialSums.Count)
            {
                int end = Math.Min(start + repeatLength, partialSums.Count - 1);
                sum += sign * (partialSums[end] - partialSums[start]);
                start += 2 * repeatLength;
                sign *= -1;
            }
            return (int)sum;
        }

        static void CalculateFFTElement(List<int> partialSums, List<int> current, int pos)
        {
            current[pos] = Math.Abs(FFTconvolution(partialSums, pos)) % 10;
        }

        static void Main(string[] args)
        {
            string input = File.ReadLines("../../../input.txt").First();
            int skip = 5971981;
//            int skip = 293510;
            //string input = "02935109699940807407585447034323";
            int repeats = 10000;
            List<int> values = Enumerable.Repeat(input.AsEnumerable().Select(c => int.Parse($"{c}")), repeats).Flatten().ToList();

            List<int> prev = values;
            List<int> current = values.Select(i => 0).ToList();
            for (int i = 0; i < 100; ++i)
            {
                List<int> partialSums = Scan(prev, 0, (a, b) => a + b).ToList();
                partialSums.Insert(0, 0);
                for (int pos = 0; pos < current.Count; ++pos)
                {
                    CalculateFFTElement(partialSums, current, pos);
                }
                prev = current;
                current = current.Select(i => 0).ToList();

                Console.WriteLine($"Phase {i + 1} calculated");
            }

            for (int pos = skip; pos < skip + 8; ++pos)
            {
                Console.Write(prev[pos]);
            }
            Console.WriteLine("");
            // not 020619550
        }
    }
}
