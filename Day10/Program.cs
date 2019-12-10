using System;
using System.Linq;
using Common;
using System.IO;
using System.Collections.Generic;

namespace Day10
{
    class Program
    {
        static HashSet<IntPoint2D> Asteroids;

        static int GCD(int a, int b)
        {
            if (a == 0)
            {
                return b;
            }
            if (b == 0)
            {
                return a;
            }
            if (a > b)
            {
                return GCD(b, a % b);
            }
            else
            {
                return GCD(a, b % a);
            }
        }

        private class ClockwiseComparer : IComparer<IntPoint2D>
        {
            int IComparer<IntPoint2D>.Compare(IntPoint2D a, IntPoint2D b)
            {
                if (a == b)
                {
                    return 0;
                }
                if (a.X == 0 && a.Y < 0 && (b.X != 0 || b.Y >= 0))
                {
                    return -1;
                }
                if (b.X == 0 && b.Y < 0 && (a.X != 0 || a.Y >= 0))
                {
                    return 1;
                }
                if (a.X == 0 && a.Y > 0 && b.X > 0)
                {
                    return 1;
                }
                if (a.X == 0 && a.Y > 0 && b.X < 0)
                {
                    return -1;
                }
                if (b.X == 0 && b.Y > 0 && a.X > 0)
                {
                    return -1;
                }
                if (b.X == 0 && b.Y > 0 && a.X < 0)
                {
                    return 1;
                }
                if (a.X > 0 && b.X <= 0) {
                    return -1;
                }
                if (b.X > 0 && a.X <= 0)
                {
                    return 1;
                }
                decimal tanA = decimal.Divide(a.Y, a.X);
                decimal tanB = decimal.Divide(b.Y, b.X);
                return decimal.Compare(tanA, tanB);
            }
        }

        static Dictionary<IntPoint2D, List<(IntPoint2D, IntPoint2D)>> ListDirections(IntPoint2D asteroid)
        {
            Dictionary<IntPoint2D, List<(IntPoint2D, IntPoint2D)>> distancesByDirection = new Dictionary<IntPoint2D, List<(IntPoint2D, IntPoint2D)>>();
            foreach (IntPoint2D other in Asteroids)
            {
                if (asteroid == other)
                {
                    continue;
                }
                IntPoint2D distance = other - asteroid;
                int gcd = GCD(Math.Abs(distance.X), Math.Abs(distance.Y));
                IntPoint2D direction = new IntPoint2D(distance.X / gcd, distance.Y / gcd);
                if (!distancesByDirection.ContainsKey(direction))
                {
                    distancesByDirection.Add(direction, new List<(IntPoint2D, IntPoint2D)>());
                }
                distancesByDirection[direction].Add((distance, other));
            }
            foreach (var dir in distancesByDirection.Keys.ToList())
            {
                distancesByDirection[dir] = distancesByDirection[dir].OrderBy(pair => pair.Item1.ManhattanDist()).ToList();
            }
            return distancesByDirection;
        }

        static void Main(string[] args)
        {
            IEnumerable<string> lines = File.ReadLines("../../../input.txt");
            int x = 0;
            int y = 0;
            Asteroids = new HashSet<IntPoint2D>();
            foreach (string line in lines)
            {
                x = 0;
                foreach (Char c in line.AsEnumerable())
                {
                    if (c == '#')
                    {
                        Asteroids.Add(new IntPoint2D(x, y));
                    }
                    ++x;
                }
                ++y;
            }
            Dictionary<IntPoint2D, List<(IntPoint2D, IntPoint2D)>> directionsFromStation = Asteroids
                .Select(ListDirections)
                .MaximalElements(dict => dict.Keys.Count).First();
            Console.WriteLine($"Best asteroid can detect {directionsFromStation.Keys.Count}");
            List<List<(IntPoint2D, IntPoint2D)>>directionsOrdered = directionsFromStation
                .OrderBy(kv => kv.Key, new ClockwiseComparer())
                .Select(kv => kv.Value)
                .ToList();
            int dirIndex = 0;
            int result = 0;
            for (int i = 0; i < 200; ++i)
            {
                while (!directionsOrdered[dirIndex].Any())
                {
                    ++dirIndex;
                }
                (IntPoint2D dist, IntPoint2D pos) point = directionsOrdered[dirIndex].First();
                Console.WriteLine($"destroying asteroid at {point.pos.X}, {point.pos.Y}");
                result = point.pos.X * 100 + point.pos.Y;
                directionsOrdered[dirIndex].RemoveAt(0);
                dirIndex++;
            }
            Console.WriteLine($"Part2 {result}");
        }
    }
}
