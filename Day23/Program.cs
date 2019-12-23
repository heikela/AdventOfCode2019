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
        List<bool> Running;

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
            Running = Enumerable.Range(0, 50).Select(i => true).ToList();
        }

        public BigInteger Simulate()
        {
            while (true)
            {
                for (int i = 0; i < 50; ++i)
                {
                    if (Running[i])
                    {
                        (bool running, IEnumerable<BigInteger> output) result =
                            Nics[i].RunIntCode(Inputs[i], false, 1);
                        if (!result.running)
                        {
                            Running[i] = false;
                        }
                        foreach (BigInteger n in result.output)
                        {
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
                                    return n;
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
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string nicProgram = File.ReadLines("../../../input.txt").First();
            Network network = new Network(nicProgram);
            Console.WriteLine($"Part 1: {network.Simulate()}");
        }
    }
}
