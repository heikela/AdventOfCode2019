using System;
using Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day19
{
    class Program
    {
        static string ProgramCode;

        static bool IsPulled(int x, int y)
        {
            IntCodeComputer computer = new IntCodeComputer(ProgramCode);
            Queue<BigInteger> input = new Queue<BigInteger>();
            input.Enqueue(x);
            input.Enqueue(y);
            (bool running, List<BigInteger> output) = computer.RunIntCode(input);
            return output[0] == 1;
        }

        static void Main(string[] args)
        {
            ProgramCode = File.ReadLines("../../../input.txt").First();

            Dictionary<IntPoint2D, int> pulled = new Dictionary<IntPoint2D, int>();

            int y;
            for (y = 0; y < 50; ++y)
            {
                for (int x = 0; x < 50; ++x)
                {
                    pulled.Add(new IntPoint2D(x, y), IsPulled(x, y) ? 1 : 0);
                }

            }
            SparseGrid.Print(pulled, n => n == 1 ? '#' : ' ');

            Console.WriteLine($"{pulled.Count(kv => kv.Value == 1)} points affected");

            Dictionary<int, int> lowestXbyY = new Dictionary<int, int>(); // inclusive
            Dictionary<int, int> highestXbyY = new Dictionary<int, int>(); // exclusive
            int SHIP_SIZE = 100;

            y = SHIP_SIZE;
            int prevXStart = 0;
            int prevXEnd = 0;
            while (true)
            {
                int x = prevXStart;
                while (!IsPulled(x, y))
                {
                    ++x;                    
                }
                prevXStart = x;
                lowestXbyY.Add(y, x);

                if (x < prevXEnd)
                {
                    x = prevXEnd;
                }

                while (IsPulled(x, y))
                {
                    ++x;
                }
                prevXEnd = x;
                highestXbyY.Add(y, x - SHIP_SIZE);

                int upperY = y - SHIP_SIZE + 1;
                if (upperY >= 0)
                {
                    int smallestPossibleX = lowestXbyY[y];
                    int largestPossibleX = highestXbyY.GetOrElse(upperY, int.MinValue);
                    if (smallestPossibleX <= largestPossibleX)
                    {
                        Console.WriteLine($"Ship fits at {10000 * smallestPossibleX + upperY}");
                        break;
                    }
                }

                ++y;
            }
        }
    }
}
