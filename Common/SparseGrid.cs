using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Common
{
    public static class SparseGrid
    {
        public static Dictionary<IntPoint2D, char> Read(IEnumerable<string> lines)
        {
            return lines
                .ZipWithIndex()
                .Select(indexedLine =>
                indexedLine.Value
                    .ZipWithIndex()
                    .Select(indexedChar => KeyValuePair.Create<IntPoint2D, char>(
                        new IntPoint2D(indexedChar.Key, indexedLine.Key), indexedChar.Value))
                )
                .Flatten()
                .ToDictionary();
        }

        public static Dictionary<IntPoint2D, char> ReadFromFile(string filename)
        {
            return Read(File.ReadLines(filename));
        }

        public static void Print<T>(Dictionary<IntPoint2D, T> grid, Func<T, char> elemPrinter)
        {
            int minY = grid.Keys.Min(p => p.Y);
            int maxY = grid.Keys.Max(p => p.Y);
            int minX = grid.Keys.Min(p => p.X);
            int maxX = grid.Keys.Max(p => p.X);
            for (int y = minY; y <= maxY; ++y)
            {
                for (int x = minX; x <= maxX; ++x)
                {
                    IntPoint2D pos = new IntPoint2D(x, y);
                    if (grid.ContainsKey(pos))
                    {
                        Console.Write(elemPrinter(grid[pos]));
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
