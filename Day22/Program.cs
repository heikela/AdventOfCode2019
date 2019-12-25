using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using static Common.MathUtils;

namespace Day22
{
    class Shuffle {
        long DeckSize;
        long PostCut;
        long Stride;

        public Shuffle(long deckSize)
        {
            DeckSize = deckSize;
            PostCut = 0;
            Stride = 1;
        }

        public Shuffle(long deckSize, IEnumerable<string> steps)
        {
            DeckSize = deckSize;
            PostCut = 0;
            Stride = 1;
            foreach (string step in steps)
            {
                AppendStep(step);
            }
        }

        public Shuffle(Shuffle first, Shuffle second)
        {
            if (first.DeckSize != second.DeckSize)
            {
                throw new Exception("Cannot combine shuffles of different sized decks");
            }
            DeckSize = first.DeckSize;
            PostCut = first.PostCut;
            Stride = first.Stride;
            AppendStride(second.Stride);
            AppendCut(second.PostCut);
        }

        private long PreToPostCut(long preCut, long stride)
        {
            return (long)(((BigInteger)preCut * (BigInteger)stride) % DeckSize);
        }

        private void AppendCut(long cut)
        {
            PostCut += cut + DeckSize;
            PostCut %= DeckSize;
        }

        private void AppendStride(long newStride)
        {
            PostCut = PreToPostCut(PostCut, newStride);
            Stride = (long)(((BigInteger)newStride * (BigInteger)Stride) % DeckSize);
        }

        public void AppendStep(string step)
        {
            if (step.Substring(0, 3) == "cut")
            {
                AppendCut(int.Parse(step.Substring(4)));
            } else if (step == "deal into new stack")
            {
                AppendCut(-1);
                AppendStride(DeckSize - 1); 
            }
            else
            {
                AppendStride(int.Parse(step.Substring(20)));
            }
        }

        public long SolveSourceIndex(long index)
        {
            index = (index + PostCut) % DeckSize;
            return (long)(((BigInteger)index * ((BigInteger)MultiplicativeInverse(Stride, DeckSize))) % DeckSize);
        }
    }

    class Program
    {
        static void SolveSample(int sample, int deckSize = 10)
        {
            IEnumerable<string> shuffleLines = File.ReadLines($"../../../sample{sample}.txt").ToList();
            Shuffle shuffle = new Shuffle(deckSize, shuffleLines);
            Console.Write($"Sample {sample}: ");
            for (int i = 0; i < deckSize; ++i)
            {
                Console.Write($"{shuffle.SolveSourceIndex(i)} ");
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            SolveSample(1);
            SolveSample(2);
            SolveSample(3);
            SolveSample(4);
            SolveSample(5, 7);
            SolveSample(6, 7);
            IEnumerable<string> shuffleSteps = File.ReadLines("../../../input.txt").ToList();
            Shuffle shuffle = new Shuffle(10007, shuffleSteps);
            for (int pos = 0; pos < 10007; ++pos)
            {
                if (shuffle.SolveSourceIndex(pos) == 2019)
                {
                    Console.WriteLine(pos);
                }
            }
            
            long deckSize = 119315717514047;
            long shuffleCount = 101741582076661;
            Shuffle bigShuffle = new Shuffle(deckSize, shuffleSteps);
            long powerOfTwo = 1;
            Shuffle exponentiatedShuffle = bigShuffle;
            Shuffle totalShuffle = new Shuffle(deckSize);
            while (powerOfTwo < shuffleCount)
            {
                if ((shuffleCount & powerOfTwo) > 0)
                {
                    totalShuffle = new Shuffle(totalShuffle, exponentiatedShuffle);
                }
                powerOfTwo <<= 1;
                exponentiatedShuffle = new Shuffle(exponentiatedShuffle, exponentiatedShuffle);
            }
            Console.WriteLine(totalShuffle.SolveSourceIndex(2020));
        }
    }
}
