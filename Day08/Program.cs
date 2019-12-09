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
            IEnumerable<IEnumerable<IEnumerable<Char>>> pixelRows = layers.Transpose().Slices(W);
            foreach (var row in pixelRows)
            {
                foreach (var pixel in row)
                {
                    Char firstNonTransparentLayer = pixel.First(c => c != '2');
                    Console.Write(firstNonTransparentLayer == '1' ? "#" : " ");
                }
                Console.WriteLine();
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
