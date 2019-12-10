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

            // -Atan2 as opposed to -X due to how the boundary conditions work
            IntPoint2D chosen = directionsFromStation
                .OrderBy(kv => -Math.Atan2(kv.Key.X, kv.Key.Y))
                .Select(kv => kv.Value)
                .Transpose()
                .Flatten()
                .ElementAt(199).Item2;
            int score = chosen.X * 100 + chosen.Y;


            Console.WriteLine($"Part2 {score}");
        }
    }
}
