using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace Day06
{
    public class Graph<T>
    {
        Dictionary<T, List<T>> Edges;

        public Graph() {
            Edges = new Dictionary<T, List<T>>();
        }

        public IEnumerable<T> GetNodes()
        {
            return Edges.Keys;
        }

        public void AddEdge(T from, T to)
        {
            if (Edges.ContainsKey(from))
            {
                Edges[from].Add(to);
            }
            else
            {
                Edges.Add(from, new List<T>() { to });
            }
            if (!Edges.ContainsKey(to))
            {
                Edges.Add(to, new List<T>());
            }
        }

        public void BfsFrom(T start, Action<T, int> visitAtDepth)
        {
            HashSet<T> visited = new HashSet<T>();
            HashSet<T> frontier = new HashSet<T>() { start };
            int depth = 0;
            while (frontier.Any())
            {
                HashSet<T> newFrontier = new HashSet<T>();
                foreach (T node in frontier)
                {
                    visitAtDepth(node, depth);
                    visited.Add(node);
                    foreach (T neighbour in Edges[node])
                    {
                        if (!visited.Contains(neighbour))
                        {
                            newFrontier.Add(neighbour);
                        }
                    }
                }
                frontier = newFrontier;
                depth += 1;
            }
        }
    }

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
