using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Common;
using System.IO;
using static Common.Extensions;
using static Common.Permutation;

namespace Day07
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

        public (bool, List<int>) RunIntCode(Queue<int> input)
        {
            List<int> output = new List<int>();
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
                            if (input.Any())
                            {
                                Memory[Memory[PC + 1]] = input.Dequeue();
                                PC += 2;
                                break;
                            } else
                            {
                                return (true, output);
                            }
                        }
                    case 4:
                        {
                            int val = GetParam(opcode, 0);
                            output.Add(val);
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
                        return (false, output);
                    default:
                        throw new Exception($"ERROR: unknown Opcode {opcode} at address {PC}");
                }
            }
        }
    }

    class Program
    {
        static string AmpProgram;

        static Queue<int> MakeInput(int phase, int input)
        {
            Queue<int> q = new Queue<int>();
            q.Enqueue(phase);
            q.Enqueue(input);
            return q;
        }

        static int RunAmplifiers(List<int> phases)
        {
            int prevOutput = 0;
            for (int amp = 0; amp < 5; ++amp)
            {
                Queue<int> input = MakeInput(phases[amp], prevOutput);
                IntCodeComputer amplifier = new IntCodeComputer(AmpProgram);
                var result = amplifier.RunIntCode(input);
                if (result.Item1)
                {
                    throw new Exception("Program did not halt as expected");
                }
                prevOutput = result.Item2.First();
            }
            return prevOutput;
        }

        static int RunAmplifiersInLoop(List<int> phases)
        {
            List<IntCodeComputer> amplifiers = new List<IntCodeComputer>();
            List<Queue<int>> inputs = new List<Queue<int>>();
            HashSet<int> runningAmplifiers = Enumerable.Range(0, phases.Count).ToHashSet();

            foreach (int phase in phases)
            {
                Queue<int> q = new Queue<int>();
                q.Enqueue(phase);
                inputs.Add(q);
                amplifiers.Add(new IntCodeComputer(AmpProgram));
                runningAmplifiers.Add(amplifiers.Count - 1);
            }
            inputs[0].Enqueue(0);

            int lastOutput = 0;

            while (runningAmplifiers.Any())
            {
                int active = runningAmplifiers.First(i => inputs[i].Any());
                var result = amplifiers[active].RunIntCode(inputs[active]);
                if (!result.Item1)
                {
                    runningAmplifiers.Remove(active);
                }
                int nextAmp = (active + 1) % amplifiers.Count;
                foreach (int val in result.Item2)
                {
                    inputs[nextAmp].Enqueue(val);
                    if (nextAmp == 0)
                    {
                        lastOutput = val;
                    }
                }
            }
            return lastOutput;
        }

        static void Main(string[] args)
        {
            AmpProgram = File.ReadLines("../../../input.txt").First();
            List<List<int>> phasePermutations = Permutations(Enumerable.Range(0, 5).ToList());
            Console.WriteLine($"Part 1: {phasePermutations.Select(RunAmplifiers).Max()}");
            List<List<int>> phasePermutations2 = Permutations(Enumerable.Range(5, 5).ToList());
            Console.WriteLine($"Part 2: {phasePermutations2.Select(RunAmplifiersInLoop).Max()}");
        }
    }
}
