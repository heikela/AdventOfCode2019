using System.Collections.Generic;
using System.Linq;
using System.IO;

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
    }
}
