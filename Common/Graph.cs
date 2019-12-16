using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public abstract class Graph<T>
    {
        public class VisitPath
        {
            private int Length;
            private T Start;
            private T End;
            private Dictionary<T, T> Predecessors;

            public VisitPath(T end, T start, int length, Dictionary<T, T> predecessors)
            {
                End = end;
                Length = length;
                Start = start;
                Predecessors = predecessors;
            }

            public int GetLength()
            {
                return Length;
            }

            public IEnumerable<T> GetNodesOnPath()
            {
                T current = End;
                Stack<T> reversePath = new Stack<T>();
                while (!current.Equals(Start))
                {
                    T pred = Predecessors[current];
                    reversePath.Push(current);
                    current = pred;
                }
                while (reversePath.Any())
                {
                    yield return reversePath.Pop();
                }
                yield break;
            }
        }

        public abstract IEnumerable<T> GetNodes();

        public abstract IEnumerable<T> GetNeighbours(T node);

        public void BfsFrom(T start, Func<T, VisitPath, bool> visit)
        {
            Dictionary<T, T> visited = new Dictionary<T, T>();
            HashSet<(T, T)> frontier = new HashSet<(T, T)>() { (start, start) };
            int depth = 0;
            bool earlyExitRequested = false;
            while (frontier.Any() && !earlyExitRequested)
            {
                HashSet<(T, T)> newFrontier = new HashSet<(T, T)>();
                foreach ((T node, T predecessor) in frontier)
                {
                    visited.Add(node, predecessor);
                    VisitPath path = new VisitPath(node, start, depth, visited);
                    earlyExitRequested = visit(node, path) || earlyExitRequested;
                    foreach (T neighbour in GetNeighbours(node))
                    {
                        if (!visited.ContainsKey(neighbour))
                        {
                            newFrontier.Add((neighbour, node));
                        }
                    }
                }
                frontier = newFrontier;
                depth += 1;
            }
        }

        public void BfsFrom(T start, Action<T, VisitPath> visit)
        {
            Func<T, VisitPath, bool> visitNoShortCircuit = (node, path) =>
            {
                visit(node, path);
                return false;
            };
            BfsFrom(start, visitNoShortCircuit);
        }

        public VisitPath ShortestPathTo(T start, T end)
        {
            return ShortestPathTo(start, n => n.Equals(end));
        }

        public VisitPath ShortestPathTo(T start, Func<T, bool> predicate)
        {
            VisitPath pathToGoal = null;
            BfsFrom(start, (node, path) =>
            {
                if (predicate(node))
                {
                    pathToGoal = path;
                    return true;
                }
                else
                {
                    return false;
                }
            });
            return pathToGoal;
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
            foreach (T neighbour in GetNeighbours(node))
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

    public class ConcreteGraph<T> : Graph<T>
    {
        Dictionary<T, List<T>> Edges;

        public ConcreteGraph()
        {
            Edges = new Dictionary<T, List<T>>();
        }

        public override IEnumerable<T> GetNodes()
        {
            return Edges.Keys;
        }

        public override IEnumerable<T> GetNeighbours(T node)
        {
            return Edges[node];
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
    }

    public class GraphByFunction<T> : Graph<T> where T : IEquatable<T>
    {
        private Func<T, IEnumerable<T>> GetEdges;

        public GraphByFunction(Func<T, IEnumerable<T>> getEdges) {
            GetEdges = getEdges;
        }

        public override IEnumerable<T> GetNeighbours(T node)
        {
            return GetEdges(node);
        }

        public override IEnumerable<T> GetNodes()
        {
            throw new Exception("Not implemented");
        }
    }

}
