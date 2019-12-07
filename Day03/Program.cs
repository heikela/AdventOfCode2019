using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Day03
{
    class Program
    {
        static void SetWire(Dictionary<IntPoint2D, int> wire, IntPoint2D pos, int d)
        {
            if (!wire.ContainsKey(pos))
            {
                wire.Add(pos, d);
            }
        }

        static void TraceMoves(Dictionary<IntPoint2D, int> wire, IEnumerable<string> moves)
        {
            IntPoint2D pos = new IntPoint2D();
            int d = 0;
            foreach (string move in moves)
            {
                Char dir = move[0];
                int dist = int.Parse(move.Substring(1));
                IntPoint2D step = new IntPoint2D();

                switch (dir)
                {
                    case 'R':
                        step = new IntPoint2D(1, 0);
                        break;
                    case 'L':
                        step = new IntPoint2D(-1, 0);
                        break;
                    case 'U':
                        step = new IntPoint2D(0, -1);
                        break;
                    case 'D':
                        step = new IntPoint2D(0, 1);
                        break;
                }
                for (int i = 0; i < dist; ++i)
                {
                    pos += step;
                    ++d;
                    SetWire(wire, pos, d);
                }
            }
        }

        static void Main(string[] args)
        {
            List<string> lines = File.ReadLines("../../../input.txt").ToList();

            Dictionary<IntPoint2D, int> wire1 = new Dictionary<IntPoint2D, int>();
            Dictionary<IntPoint2D, int> wire2 = new Dictionary<IntPoint2D, int>();

            IEnumerable<string> moves1 = lines[0].Split(',');
            IEnumerable<string> moves2 = lines[1].Split(',');

            TraceMoves(wire1, moves1);
            TraceMoves(wire2, moves2);

            HashSet<IntPoint2D> intersections = wire1.Keys.ToHashSet();
            intersections.IntersectWith(wire2.Keys);

            Console.WriteLine($"Part1 = {intersections.Select(p => p.ManhattanDist()).Min()}");
            Console.WriteLine($"Part2 = {intersections.Select(p => wire1[p] + wire2[p]).Min()}");
        }
    }
}
