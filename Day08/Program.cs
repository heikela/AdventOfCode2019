using System;
using System.Linq;
using System.Collections.Generic;
using Common;
using System.IO;
using static Common.Extensions;

namespace Day08
{
    class Program
    {
        static readonly int W = 25;
        static readonly int H = 6;
        static readonly int LAYER_SIZE = W * H;

        static void PrintImage(IEnumerable<IEnumerable<Char>> layers)
        {
            IEnumerator<IEnumerable<Char>> pixels = layers.Transpose().GetEnumerator();
            int x = 0;
            while (pixels.MoveNext())
            {
                Char firstNonTransparentLayer = pixels.Current.First(c => c != '2');
                Console.Write(firstNonTransparentLayer == '1' ? "#" : " ");
                ++x;
                if (x == W)
                {
                    x = 0;
                    Console.WriteLine();
                }
            }
        }

        static void Main(string[] args)
        {
            Char[] imageData = File.ReadLines("../../../input.txt").First().ToCharArray();
            IEnumerable<IEnumerable<Char>> layers = imageData.Slices(LAYER_SIZE);
            IEnumerable<Char> bestLayer = layers.MinimalElements(l => l.Count(c => c == '0')).First();
            Console.WriteLine($"Part 1: {bestLayer.Count(c => c == '1') * bestLayer.Count(c => c == '2')}");
            PrintImage(layers);
        }
    }
}
