using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Common;
using System.IO;

namespace Day06
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> lines = File.ReadLines("../../../input.txt");
            Regex orbitPattern = new Regex(@"(\w+)\)(\w+)");
            Graph<string> orbits = new Graph<string>();
            foreach (string line in lines)
            {
                Match match = orbitPattern.Match(line);
                if (match.Success)
                {
                    string center = match.Groups[1].Value;
                    string orbiter = match.Groups[2].Value;
                    orbits.AddEdge(center, orbiter);
                    orbits.AddEdge(orbiter, center);
                }
            }
            int directAndIndirectOrbits = 0;
            orbits.BfsFrom("COM", (body, depth) => { directAndIndirectOrbits += depth; });

            Console.WriteLine($"Part 1: {directAndIndirectOrbits}");

            orbits.BfsFrom("YOU", (body, depth) => { if (body == "SAN") Console.WriteLine($"Part 2: {depth - 2}"); });
        }
    }
}
