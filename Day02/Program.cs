using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day02
{
    class Program
    {
        static void ExecuteBinOp(List<int> values, int pc, Func<int, int, int> op)
        {
            int srcAddr1 = values[pc + 1];
            int srcAddr2 = values[pc + 2];
            int destAddr = values[pc + 3];
            values[destAddr] = op(values[srcAddr1], values[srcAddr2]);
        }

        static int RunIntCode(string input, int noun, int verb)
        {
            List<int> values = input.Split(',').Select(line => int.Parse(line)).ToList();

            values[1] = noun;
            values[2] = verb;

            int pc = 0;
            while (true)
            {
                int opcode = values[pc];
                switch (values[pc])
                {
                    case 1:
                        {
                            ExecuteBinOp(values, pc, (a, b) => a + b);
                            pc += 4;
                            break;
                        }
                    case 2:
                        {
                            ExecuteBinOp(values, pc, (a, b) => a * b);
                            pc += 4;
                            break;
                        }
                    case 99:
                        return values[0];
                    default:
                        throw new Exception($"ERROR: unknown Opcode {opcode} at address {pc}");
                }
            }
        }

        static void Main(string[] args)
        {
            string input = File.ReadLines("../../../input.txt").First();

            Console.WriteLine($"Part1 = {RunIntCode(input, 12, 2)}");
            for (int noun = 0; noun < 100; ++noun)
            {
                for (int verb = 0; verb < 100; ++verb)
                {
                    int result = RunIntCode(input, noun, verb);
                    if (result == 19690720)
                    {
                        Console.WriteLine($"Part2 = {100 * noun + verb}");
                    }
                }
            }
        }
    }
}
