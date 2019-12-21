using System;
using Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day21
{
    class Program
    {
        static Queue<BigInteger> MakeInput(IEnumerable<string> asciiLines)
        {
            Queue<BigInteger> inputs = new Queue<BigInteger>();
            foreach (string line in asciiLines)
            {
                foreach (char c in line)
                {
                    inputs.Enqueue(c);
                }
                inputs.Enqueue(10);
            }
            return inputs;
        }

        static void Main(string[] args)
        {
            string program = File.ReadLines("../../../input.txt").First();
            IntCodeComputer computer = new IntCodeComputer(program);

            //"D and not (A & B & C)"
            List<string> jumpProgram = new List<string>()
            {
                "NOT A T",
                "NOT T T",
                "AND B T",
                "AND C T",
                "NOT T T",
                "NOT D J",
                "NOT J J",
                "AND T J",
                "WALK"
            };

            (bool running, List<BigInteger> output) = computer.RunIntCode(MakeInput(jumpProgram));
            foreach (BigInteger n in output)
            {
                if (n > 255)
                {
                    Console.WriteLine($"Got hull damage reading {n}");
                }
                else
                {
                    if (n == 10)
                    {
                        Console.WriteLine();
                    }
                    else {
                        Console.Write((char)n);
                    }
                }
            }
        }
    }
}
