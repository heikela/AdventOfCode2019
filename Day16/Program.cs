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
        static List<int> BasePattern = new List<int>
        {
            0, 1, 0, -1
        };

        static int convolution(IEnumerable<int> first, IEnumerable<int> second)
        {
            var a = first.GetEnumerator();
            var b = second.GetEnumerator();
            int sum = 0;
            while (a.MoveNext() && b.MoveNext())
            {
                sum += a.Current * b.Current;
            }
            return sum;
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

        static int FFTconvolution(IEnumerable<int> first, int offset)
        {        
            var a = first.GetEnumerator();
            int sum = 0;
            int i = offset;
            while (a.MoveNext())
            {
                sum += a.Current * multiplier(offset, i);
                ++i;
            }
            return sum;
        }

        static IEnumerable<int> FFT(IEnumerable<int> input, int offset = 0)
        {
            var inputEnumerator = input.GetEnumerator();
            int i = 0;
            while (inputEnumerator.MoveNext())
            {
                var repeatingPattern = BasePattern.SelectMany(n => Enumerable.Repeat(n, i + offset + 1));
                var repeatedPattern = Enumerable.Repeat(repeatingPattern, 100000).Flatten();
                int sum = convolution(input.Skip(i), repeatedPattern.Skip(i + offset + 1));
                yield return Math.Abs(sum) % 10;
                ++i;
            }
            yield break;
        }

        static void Main(string[] args)
        {
            string input = File.ReadLines("../../../input.txt").First();
            int skip = 5971981;
            //string input = "12345678";
            List<int> values = Enumerable.Repeat(input.AsEnumerable().Select(c => int.Parse($"{c}")), 10000).Flatten().ToList();

            for (int i = 0; i < 100; ++i)
            {
                values = FFT(values, skip).ToList();
                Console.WriteLine($"Phase {i + 1}:");
                foreach (var n in values.Take(8))
                {
                    Console.Write(n);
                }
                Console.WriteLine("");
            }

            // not 020619550
        }
    }
}
