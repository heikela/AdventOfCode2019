using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day05
{
    class IntCodeComputer
    {
        List<int> Memory;
        int PC;

        public IntCodeComputer(string input)
        {
            Memory = input.Split(',').Select(s => int.Parse(s)).ToList();
            PC = 0;
        }

        int GetParam(int opcode, int param)
        {
            int pow = 100;
            for (int i = 0; i < param; ++i)
            {
                pow *= 10;
            }
            bool immediate = (opcode / pow) % 10 == 1;
            int paramValue = Memory[PC + param + 1];
            if (immediate)
            {
                return paramValue;
            }
            else
            {
                return Memory[paramValue];
            }
        }

        void ExecuteBinOp(int opcode, Func<int, int, int> op)
        {
            int destAddr = Memory[PC + 3];
            Memory[destAddr] = op(GetParam(opcode, 0), GetParam(opcode, 1));
        }

        public int RunIntCode(Func<int> input, Action<int> output)
        {
            while (true)
            {
                int opcode = Memory[PC];
                switch (opcode % 100)
                {
                    case 1:
                        {
                            ExecuteBinOp(opcode, (a, b) => a + b);
                            PC += 4;
                            break;
                        }
                    case 2:
                        {
                            ExecuteBinOp(opcode, (a, b) => a * b);
                            PC += 4;
                            break;
                        }
                    case 3:
                        {
                            Memory[Memory[PC + 1]] = input();
                            PC += 2;
                            break;
                        }
                    case 4:
                        {
                            int val = GetParam(opcode, 0);
                            output(val);
                            PC += 2;
                            break;
                        }
                    case 5:
                        {
                            int val = GetParam(opcode, 0);
                            int dest = GetParam(opcode, 1);
                            PC += 3;
                            if (val != 0)
                            {
                                PC = dest;
                            }
                            break;
                        }
                    case 6:
                        {
                            int val = GetParam(opcode, 0);
                            int dest = GetParam(opcode, 1);
                            PC += 3;
                            if (val == 0)
                            {
                                PC = dest;
                            }
                            break;
                        }
                    case 7:
                        {
                            ExecuteBinOp(opcode, (a, b) => a < b ? 1 : 0);
                            PC += 4;
                            break;
                        }
                    case 8:
                        {
                            ExecuteBinOp(opcode, (a, b) => a == b ? 1 : 0);
                            PC += 4;
                            break;
                        }
                    case 99:
                        return Memory[0];
                    default:
                        throw new Exception($"ERROR: unknown Opcode {opcode} at address {PC}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string program = File.ReadLines("../../../input.txt").First();
            IntCodeComputer p1 = new IntCodeComputer(program);
            p1.RunIntCode(() => 1, (val) => Console.WriteLine($"{val}"));
            IntCodeComputer p2 = new IntCodeComputer(program);
            p2.RunIntCode(() => 5, (val) => Console.WriteLine($"{val}"));
        }
    }
}
