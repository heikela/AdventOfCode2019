using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Day09
{
    class IntCodeComputer
    {
        List<Int64> Memory;
        int PC;
        Int64 RelativeBase;

        public IntCodeComputer(string input)
        {
            Memory = input.Split(',').Select(s => Int64.Parse(s)).ToList();
            PC = 0;
            RelativeBase = 0;
        }

        void GrowMemory(int requiredAddress)
        {
            if (Memory.Count <= requiredAddress)
            {
                Memory.AddRange(new Int64[requiredAddress - Memory.Count + 1]);
            }
        }

        Int64 GetParam(Int64 opcode, int param)
        {
            int pow = 100;
            for (int i = 0; i < param; ++i)
            {
                pow *= 10;
            }
            bool immediate = (opcode / pow) % 10 == 1;
            bool relative = (opcode / pow) % 10 == 2;
            Int64 paramValue = Memory[PC + param + 1];
            if (immediate)
            {
                return paramValue;
            }
            else if (relative)
            {
                int addr = (int)(paramValue + RelativeBase);
                GrowMemory(addr);
                return Memory[addr];
            }
            else
            {
                int addr = (int)(paramValue);
                GrowMemory(addr);
                return Memory[addr];
            }
        }

        int GetParamAddr(Int64 opcode, int param)
        {
            int pow = 100;
            for (int i = 0; i < param; ++i)
            {
                pow *= 10;
            }
            bool relative = (opcode / pow) % 10 == 2;
            Int64 paramValue = Memory[PC + param + 1];
            if (relative)
            {
                int addr = (int)(paramValue + RelativeBase);
                GrowMemory(addr);
                return addr;
            }
            else
            {
                int addr = (int)(paramValue);
                GrowMemory(addr);
                return addr;
            }
        }

        void ExecuteBinOp(Int64 opcode, Func<Int64, Int64, Int64> op)
        {
//            Int64 destAddr = (int)Memory[PC + 3];
            int destAddr = GetParamAddr(opcode, 2);
/*            if ((opcode / 10000) % 10 == 2)
            {
                destAddr = destAddr + RelativeBase;
            }*/
            GrowMemory((int)destAddr);
            Memory[destAddr] = op(GetParam(opcode, 0), GetParam(opcode, 1));
        }

        public (bool, List<Int64>) RunIntCode(Queue<Int64> input)
        {
            List<Int64> output = new List<Int64>();
            while (true)
            {
                Int64 opcode = Memory[PC];
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
                            if (input.Any())
                            {
                                int destAddr = GetParamAddr(opcode, 0);
                                Memory[destAddr] = input.Dequeue();
                                PC += 2;
                                break;
                            }
                            else
                            {
                                return (true, output);
                            }
                        }
                    case 4:
                        {
                            Int64 val = GetParam(opcode, 0);
                            output.Add(val);
                            PC += 2;
                            break;
                        }
                    case 5:
                        {
                            Int64 val = GetParam(opcode, 0);
                            Int64 dest = GetParam(opcode, 1);
                            PC += 3;
                            if (val != 0)
                            {
                                PC = (int)dest;
                            }
                            break;
                        }
                    case 6:
                        {
                            Int64 val = GetParam(opcode, 0);
                            Int64 dest = GetParam(opcode, 1);
                            PC += 3;
                            if (val == 0)
                            {
                                PC = (int)dest;
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
                    case 9:
                        {
                            Int64 change = GetParam(opcode, 0);
                            RelativeBase = change + RelativeBase;
                            PC += 2;
                            break;
                        }
                    case 99:
                        return (false, output);
                    default:
                        throw new Exception($"ERROR: unknown Opcode {opcode} at address {PC}");
                }
                GrowMemory(PC + 3);
            }
        }
    }

    class Program
    {
        static string BoostProgram;

        static Queue<Int64> MakeInput(IEnumerable<Int64> values)
        {
            Queue<Int64> q = new Queue<Int64>();
            foreach (Int64 d in values)
            {
                q.Enqueue(d);
            }
            return q;
        }

        static void RunWithInput(Int64 value)
        {
            IntCodeComputer computer = new IntCodeComputer(BoostProgram);
            var result = computer.RunIntCode(MakeInput(new Int64[] { value }));
            foreach (Int64 outputValue in result.Item2)
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
