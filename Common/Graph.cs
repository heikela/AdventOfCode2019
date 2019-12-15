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

        private void TopologicalSortVisit(HashSet<T> fullyVisited, HashSet<T> partiallyVisited, Stack<T> sorted, T node)
        {
            if (fullyVisited.Contains(node))
            {
                return;
            }
            if (partiallyVisited.Contains(node))
            {
                throw new Exception("Not a DAG");
            }
            partiallyVisited.Add(node);
            foreach (T neighbour in Edges[node])
            {
                TopologicalSortVisit(fullyVisited, partiallyVisited, sorted, neighbour);
            }
            partiallyVisited.Remove(node);
            fullyVisited.Add(node);
            sorted.Push(node);
        }

        public IEnumerable<T> TopologicalSort()
        {
            HashSet<T> fullyVisited = new HashSet<T>();
            HashSet<T> partiallyVisited = new HashSet<T>();
            Stack<T> sorted = new Stack<T>();
            foreach (T node in GetNodes())
            {
                TopologicalSortVisit(fullyVisited, partiallyVisited, sorted, node);
            }
            while (sorted.Any())
            {
                yield return sorted.Pop();
            }
            yield break;
        }

    }



    public class GraphByFunction
    {
        private static IEnumerable<T> RecoverPath<T>(T node, T start, Dictionary<T, T> predecessors) where T: IEquatable<T>
        {
            T current = node;
            Stack<T> reversePath = new Stack<T>();
            while (!current.Equals(start))
            {
                T pred = predecessors[current];
                reversePath.Push(current);
                current = pred;
            }
            while (reversePath.Any())
            {
                yield return reversePath.Pop();
            }
            yield break;
        }

        public static void BfsPathFrom<T>(T start, Func<T, IEnumerable<T>> getEdges, Action<T, List<T>> visitVia) where T : IEquatable<T>
        {
            Dictionary<T, T> visited = new Dictionary<T, T>();
            HashSet<(T, T)> frontier = new HashSet<(T, T)>() { (start, start) };
            while (frontier.Any())
            {
                HashSet<(T, T)> newFrontier = new HashSet<(T,T)>();
                foreach ((T node, T predecessor) in frontier)
                {
                    visited.Add(node, predecessor);
                    visitVia(node, RecoverPath(node, start, visited).ToList());
                    foreach (T neighbour in getEdges(node))
                    {
                        if (!visited.ContainsKey(neighbour))
                        {
                            newFrontier.Add((neighbour, node));
                        }
                    }
                }
                frontier = newFrontier;
            }
        }


    }

}
