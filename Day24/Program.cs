using System;
using Common;
using System.Collections.Generic;
using System.Linq;


namespace Day24
{
    class Program
    {
        static List<IntPoint2D> Directions = new List<IntPoint2D>()
        {
            new IntPoint2D(0, -1),
            new IntPoint2D(0, 1),
            new IntPoint2D(-1, 0),
            new IntPoint2D(1, 0)
        };

        static int BioDiversity(Dictionary<IntPoint2D, char> grid)
        {
            int pow = 1;
            int result = 0;
            for (int y = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    if (grid[new IntPoint2D(x, y)] == '#')
                    {
                        result += pow;
                    }
                    pow <<= 1;
                }
            }
            return result;
        }

        static Dictionary<IntPoint2D, char> NextMinute(Dictionary<IntPoint2D, char> prev)
        {
            Dictionary<IntPoint2D, char> result = new Dictionary<IntPoint2D, char>();
            foreach (KeyValuePair<IntPoint2D, char> kv in prev)
            {
                int neighbouringBugs = Directions
                    .Select(d => kv.Key + d)
                    .Select(p => prev.GetOrElse(p, '.'))
                    .Count(c => c == '#');
                if (kv.Value == '#')
                {
                    if (neighbouringBugs == 1)
                    {
                        result.Add(kv.Key, '#');
                    }
                    else
                    {
                        result.Add(kv.Key, '.');
                    }
                }
                if (kv.Value == '.')
                {
                    if (neighbouringBugs == 1 || neighbouringBugs == 2)
                    {
                        result.Add(kv.Key, '#');
                    }
                    else
                    {
                        result.Add(kv.Key, '.');
                    }
                }
            }
            return result;
        }

        static IEnumerable<(int level, IntPoint2D pos)> InfiniteNeighbours(int level, IntPoint2D pos)
        {
            IntPoint2D centre = new IntPoint2D(2, 2);
            foreach (var d in Directions)
            {
                IntPoint2D newPos = d + pos;
                if (newPos == centre)
                {
                    for (int step = 0; step < 5; step++)
                    {
                        if (d == new IntPoint2D(-1, 0))
                        {
                            yield return (level - 1, new IntPoint2D(4, step));
                        }
                        if (d == new IntPoint2D(1, 0))
                        {
                            yield return (level - 1, new IntPoint2D(0, step));
                        }
                        if (d == new IntPoint2D(0, -1))
                        {
                            yield return (level - 1, new IntPoint2D(step, 4));
                        }
                        if (d == new IntPoint2D(0, 1))
                        {
                            yield return (level - 1, new IntPoint2D(step, 0));
                        }
                    }
                }
                else if (newPos.X >= 0 && newPos.Y >= 0 && newPos.X < 5 && newPos.Y < 5)
                {
                    yield return (level, newPos);
                }
                else // outward
                {
                    yield return (level + 1, centre + d);
                }
            }
            yield break;
        }

        static Dictionary<int, Dictionary<IntPoint2D, char>> NextMinuteInfinite(Dictionary<int, Dictionary<IntPoint2D, char>> prev)
        {
            int minLevel = prev.Keys.Min();
            int maxLevel = prev.Keys.Max();
            Dictionary<int, Dictionary<IntPoint2D, char>> result = new Dictionary<int, Dictionary<IntPoint2D, char>>();
            for (int thisLevel = minLevel - 1; thisLevel <= maxLevel + 1; ++thisLevel)
            {
                Dictionary<IntPoint2D, char> newPlane = new Dictionary<IntPoint2D, char>();
                foreach (IntPoint2D pos in prev[0].Keys)
                {
                    int neighbouringBugs = InfiniteNeighbours(thisLevel, pos)
                        .Select(n => prev.GetOrElse(n.level, new Dictionary<IntPoint2D, char>()).GetOrElse(n.pos, '.'))
                        .Count(c => c == '#');
                    char prevStatus = prev.GetOrElse(thisLevel, new Dictionary<IntPoint2D, char>()).GetOrElse(pos, '.');
                    if (prevStatus == '#')
                    {
                        if (neighbouringBugs == 1)
                        {
                            newPlane.Add(pos, '#');
                        }
                        else
                        {
                            newPlane.Add(pos, '.');
                        }
                    }
                    if (prevStatus == '.')
                    {
                        if (neighbouringBugs == 1 || neighbouringBugs == 2)
                        {
                            newPlane.Add(pos, '#');
                        }
                        else
                        {
                            newPlane.Add(pos, '.');
                        }
                    }
                }
                result.Add(thisLevel, newPlane);
            }

            return result;
        }

        static void Main(string[] args)
        {
            Dictionary<IntPoint2D, char> grid = SparseGrid.ReadFromFile("../../../input.txt");
            HashSet<int> diversities = new HashSet<int>();
            while (!diversities.Contains(BioDiversity(grid)))
            {
                diversities.Add(BioDiversity(grid));
                grid = NextMinute(grid);
            }
            Console.WriteLine($"Biodiversity {BioDiversity(grid)} repeats");

            Dictionary<int, Dictionary<IntPoint2D, char>> infiniteGrids = new Dictionary<int, Dictionary<IntPoint2D, char>>();
//            infiniteGrids.Add(0, SparseGrid.ReadFromFile("../../../sample1.txt"));
            infiniteGrids.Add(0, SparseGrid.ReadFromFile("../../../input.txt"));
            infiniteGrids[0].Remove(new IntPoint2D(2, 2));
//            for (int t = 0; t < 10; ++t)
            for (int t = 0; t < 200; ++t)
                {
                    infiniteGrids = NextMinuteInfinite(infiniteGrids);
            }
            Console.WriteLine($"{infiniteGrids.Sum(kv => kv.Value.Count(kv => kv.Value == '#'))} infested squares in infinite grids after 200 minutes");
            // 8 is not the right answer;
/*
            for (int i = infiniteGrids.Keys.Min(); i <= infiniteGrids.Keys.Max(); ++i)
            {
                Console.WriteLine($"Level {i}");
                SparseGrid.Print(infiniteGrids[i], c => c);
                Console.WriteLine();
            }*/
        }
    }
}
