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

        static void PrintImage(IEnumerable<IEnumerable<char>> layers)
        {
            List<List<Char>> layerData = layers.Select(l => l.ToList()).ToList();
            int layerCount = layerData.Count;
            for (int y = 0; y < H; ++y)
            {
                for (int x = 0; x < W; ++x)
                {
                    for (int l = 0; l < layerCount; ++l)
                    {
                        Char pix = layerData[l].First();
                        if (pix == '2')
                        {
                            continue;
                        } else if (pix == '0') {
                            Console.Write(' ');
                            break;
                        } else if (pix == '1')
                        {
                            Console.Write('#');
                            break;
                        }
                    }
                    foreach (List<Char> layer in layerData)
                    {
                        layer.RemoveAt(0);
                    }
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
