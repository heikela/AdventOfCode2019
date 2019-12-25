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
        static string ProgrammerProgram;

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

        static void HandleOutput(IEnumerable<BigInteger> output)
        {
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
                    else
                    {
                        Console.Write((char)n);
                    }
                }
            }
        }

        static void TestJumpProgram(IEnumerable<string> program)
        {
            IntCodeComputer computer = new IntCodeComputer(ProgrammerProgram);
            (bool running, List<BigInteger> output) = computer.RunIntCodeV11(MakeInput(program));
            HandleOutput(output);
        }

        static void Main(string[] args)
        {
            ProgrammerProgram = File.ReadLines("../../../input.txt").First();

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
            TestJumpProgram(jumpProgram);

            //"D and not (A & B & C) and (E or H)"
            List<string> part2Program = new List<string>()
            {
                "NOT A T", 
                "NOT T T",
                "AND B T",
                "AND C T",
                "NOT T J",
                "AND D J",
                "NOT E T",
                "NOT T T",
                "OR H T",
                "AND T J",
                "RUN"
            };
            TestJumpProgram(part2Program);

        }
    }
}
