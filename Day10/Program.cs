﻿using System;
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

        static Dictionary<IntPoint2D, List<IntPoint2D>> AsteroidsByDirectionAndRadius(IntPoint2D asteroid)
        {
            Dictionary<IntPoint2D, List<(IntPoint2D pos, IntPoint2D dist)>> asteroidsAndDistancesByDirection =
                new Dictionary<IntPoint2D, List<(IntPoint2D, IntPoint2D)>>();
            foreach (IntPoint2D other in Asteroids)
            {
                if (asteroid == other)
                {
                    continue;
                }
                IntPoint2D distance = other - asteroid;
                int gcd = GCD(Math.Abs(distance.X), Math.Abs(distance.Y));
                IntPoint2D direction = new IntPoint2D(distance.X / gcd, distance.Y / gcd);
                if (!asteroidsAndDistancesByDirection.ContainsKey(direction))
                {
                    asteroidsAndDistancesByDirection.Add(direction, new List<(IntPoint2D pos, IntPoint2D dist)>());
                }
                asteroidsAndDistancesByDirection[direction].Add((other, distance));
            }

            return asteroidsAndDistancesByDirection
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value
                        .OrderBy<(IntPoint2D pos, IntPoint2D dist), int>(pair => pair.dist.ManhattanDist())
                        .Select(pair => pair.pos)
                        .ToList());
        }

        static void Main(string[] args)
        {
            Asteroids = SparseGrid
                .ReadFromFile("../../../input.txt")
                .Where(kv => kv.Value == '#')
                .Select(kv => kv.Key)
                .ToHashSet();

            Dictionary<IntPoint2D, List<IntPoint2D>> asteroidsFromStation = Asteroids
                .Select(AsteroidsByDirectionAndRadius)
                .MaximalElements(dict => dict.Keys.Count).First();
            Console.WriteLine($"Best asteroid can detect {asteroidsFromStation.Keys.Count}");

            // -Atan2 as opposed to -X due to how the boundary conditions work
            IntPoint2D chosen = asteroidsFromStation
                .OrderBy(kv => -Math.Atan2(kv.Key.X, kv.Key.Y))
                .Select(kv => kv.Value)
                .Transpose()
                .Flatten()
                .ElementAt(199);
            int score = chosen.X * 100 + chosen.Y;

            Console.WriteLine($"Part2 {score}");
        }
    }
}