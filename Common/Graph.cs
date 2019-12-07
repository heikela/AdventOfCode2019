using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class Graph<T>
    {
        Dictionary<T, List<T>> Edges;

        public Graph()
        {
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
}
