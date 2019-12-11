using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Common;
using System.Numerics;

namespace Day09
{
    class Program
    {
        static string BoostProgram;

        static Queue<BigInteger> MakeInput(IEnumerable<BigInteger> values)
        {
            Queue<BigInteger> q = new Queue<BigInteger>();
            foreach (BigInteger d in values)
            {
                q.Enqueue(d);
            }
            return q;
        }

        static void RunWithInput(BigInteger value)
        {
            IntCodeComputer computer = new IntCodeComputer(BoostProgram);
            var result = computer.RunIntCode(MakeInput(new BigInteger[] { value }));
            foreach (BigInteger outputValue in result.Item2)
            {
                Console.WriteLine($"{outputValue}");
            }
        }

        static void Main(string[] args)
        {
            BoostProgram = File.ReadLines("../../../input.txt").First();
            Console.WriteLine($"Part 1:");
            RunWithInput(1);
            Console.WriteLine($"Part 2:");
            RunWithInput(2);
        }
    }
}
