using System;
using System.Linq;
using System.Collections.Generic;
using Common;
using System.IO;
using static Common.Permutation;
using System.Numerics;

namespace Day07
{
    class Program
    {
        static string AmpProgram;

        static Queue<BigInteger> MakeInput(BigInteger phase, BigInteger input)
        {
            Queue<BigInteger> q = new Queue<BigInteger>();
            q.Enqueue(phase);
            q.Enqueue(input);
            return q;
        }

        static BigInteger RunAmplifiers(List<int> phases)
        {
            BigInteger prevOutput = 0;
            for (int amp = 0; amp < 5; ++amp)
            {
                Queue<BigInteger> input = MakeInput(phases[amp], prevOutput);
                IntCodeComputer amplifier = new IntCodeComputer(AmpProgram);
                var result = amplifier.RunIntCode(input);
                if (result.running)
                {
                    throw new Exception("Program did not halt as expected");
                }
                prevOutput = result.output.First();
            }
            return prevOutput;
        }

        static int RunAmplifiersInLoop(List<int> phases)
        {
            List<IntCodeComputer> amplifiers = new List<IntCodeComputer>();
            List<Queue<BigInteger>> inputs = new List<Queue<BigInteger>>();
            HashSet<int> runningAmplifiers = Enumerable.Range(0, phases.Count).ToHashSet();

            foreach (int phase in phases)
            {
                Queue<BigInteger> q = new Queue<BigInteger>();
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
                if (!result.running)
                {
                    runningAmplifiers.Remove(active);
                }
                int nextAmp = (active + 1) % amplifiers.Count;
                foreach (int val in result.output)
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
