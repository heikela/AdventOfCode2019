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
                .Zip(Enumerable.Range(0, lines.Count()))
                .Select(lineAndY =>
                {
                    string line = lineAndY.First;
                    int y = lineAndY.Second;
                    return line
                        .AsEnumerable()
                        .Zip(Enumerable.Range(0, line.Length))
                        .Select(charAndX =>
                        {
                            char c = charAndX.First;
                            int x = charAndX.Second;
                            return (pos: new IntPoint2D(x, y), c);
                        });
                })
                .Flatten()
                .ToDictionary(kv => kv.pos, kv => kv.c);
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
