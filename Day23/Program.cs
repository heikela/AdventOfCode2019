using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Common;
using System.Collections.Generic;

namespace Day23
{
    class OutputBuffer
    {
        public int? Addr;
        public BigInteger? X;
    };

    class Network
    {
        List<IntCodeComputer> Nics;
        List<Queue<BigInteger>> Inputs;
        List<OutputBuffer> OutBuffers;
        List<IntCodeComputer.State> NicStates;
        List<int> CountPollWithoutOutput;

        public Network(string program)
        {
            Nics = Enumerable.Range(0, 50).Select(i => new IntCodeComputer(program)).ToList();
            Inputs = Enumerable.Range(0, 50).Select(i =>
            {
                Queue<BigInteger> q = new Queue<BigInteger>();
                q.Enqueue(i);
                return q;
            }).ToList();
            OutBuffers = Enumerable.Range(0, 50).Select(i => new OutputBuffer()).ToList();
            NicStates = Enumerable.Range(0, 50).Select(i => IntCodeComputer.State.Running).ToList();
            CountPollWithoutOutput = Enumerable.Range(0, 50).Select(i => 0).ToList();
        }

        public BigInteger Simulate()
        {
            BigInteger? natX = null;
            BigInteger? natY = null;
            BigInteger? prevNatX = null;
            BigInteger? prevNatY = null;
            int idleThreshold = 1000;
            while (true)
            {
                for (int i = 0; i < 50; ++i)
                {
                    if (NicStates[i] != IntCodeComputer.State.Stopped)
                    {
                        (IntCodeComputer.State state, IEnumerable<BigInteger> output) result =
                            Nics[i].RunIntCode(Inputs[i], false, 1);
                        NicStates[i] = result.state;
                        if (result.state == IntCodeComputer.State.PollingForInput)
                        {
                            CountPollWithoutOutput[i]++;
                        }
                        foreach (BigInteger n in result.output)
                        {
                            CountPollWithoutOutput[i] = 0;
                            if (OutBuffers[i].Addr == null)
                            {
                                if (n > int.MaxValue)
                                {
                                    throw new Exception("address is too large");
                                }
                                OutBuffers[i].Addr = (int)n;
                            }
                            else if (OutBuffers[i].X == null)
                            {
                                OutBuffers[i].X = n;
                            }
                            else
                            {
                                if (OutBuffers[i].Addr == 255)
                                {
                                    natX = OutBuffers[i].X.Value;
                                    natY = n;
                                    OutBuffers[i].Addr = null;
                                    OutBuffers[i].X = null;
                                }
                                else
                                {
                                    Inputs[OutBuffers[i].Addr.Value].Enqueue(OutBuffers[i].X.Value);
                                    Inputs[OutBuffers[i].Addr.Value].Enqueue(n);
                                    OutBuffers[i].Addr = null;
                                    OutBuffers[i].X = null;
                                }
                            }
                        }
                    }
                }
                if (CountPollWithoutOutput.Min() > idleThreshold)
                {
                    if (!natX.HasValue || !natY.HasValue)
                    {
                        throw new Exception("Needing Nat message before setting Nat state");
                    }
                    Inputs[0].Enqueue(natX.Value);
                    Inputs[0].Enqueue(natY.Value);
                    if (natX == prevNatX && natY == prevNatY)
                    {
                        return prevNatY.Value;
                    }
                    prevNatX = natX;
                    prevNatY = natY;
                    foreach (int i in Enumerable.Range(0, CountPollWithoutOutput.Count))
                    {
                        CountPollWithoutOutput[i] = 0;
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string nicProgram = File.ReadLines("../../../input.txt").First();
            Network network = new Network(nicProgram);
            Console.WriteLine($"Part 2: {network.Simulate()}");
        }
    }
}
