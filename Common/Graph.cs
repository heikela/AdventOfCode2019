using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;

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
            Dictionary<T, T> frontier = new Dictionary<T, T>();
            frontier.Add(start, start);
            int depth = 0;
            bool earlyExitRequested = false;
            while (frontier.Any() && !earlyExitRequested)
            {
                Dictionary<T, T> newFrontier = new Dictionary<T, T>();
                foreach ((T node, T predecessor) in frontier)
                {
                    if (visited.ContainsKey(node))
                    {
                        // How does this happen?
                        continue;
                    }
                    visited.Add(node, predecessor);
                    VisitPath path = new VisitPath(node, start, depth, visited);
                    earlyExitRequested = visit(node, path) || earlyExitRequested;
                    foreach (T neighbour in GetNeighbours(node))
                    {
                        if (!visited.ContainsKey(neighbour) && !newFrontier.ContainsKey(neighbour))
                        {
                            newFrontier.Add(neighbour, node);
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

        public GraphByFunction(Func<T, IEnumerable<T>> getEdges)
        {
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


    public abstract class WeightedGraph<T>
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

        public abstract IEnumerable<(T, int)> GetNeighbours(T node);

        public void DijkstraFrom(T start, Func<T, VisitPath, bool> visit)
        {
            SimplePriorityQueue<T, int> queue = new SimplePriorityQueue<T, int>();
            Dictionary<T, int> bestSoFar = new Dictionary<T, int>();
            Dictionary<T, T> predecessor = new Dictionary<T, T>();

            queue.Enqueue(start, 0);
            bestSoFar.Add(start, 0);
            predecessor.Add(start, start);
            while (queue.Count != 0)
            {
                T node = queue.Dequeue();
                if (visit(node, new VisitPath(node, start, bestSoFar[node], predecessor)))
                {
                    return;
                }
                foreach ((T node, int edgeWeight) neighbour in GetNeighbours(node))
                {
                    int potentialDist = bestSoFar[node] + neighbour.edgeWeight;
                    if (potentialDist < bestSoFar.GetOrElse(neighbour.node, int.MaxValue))
                    {
                        bestSoFar.AddOrSet(neighbour.node, potentialDist);
                        predecessor.AddOrSet(neighbour.node, node);
                        if (queue.Contains(neighbour.node))
                        {
                            queue.UpdatePriority(neighbour.node, potentialDist);
                        }
                        else
                        {
                            queue.Enqueue(neighbour.node, potentialDist);
                        }
                    }
                }
            }
        }

        public void DijkstraFrom(T start, Action<T, VisitPath> visit)
        {
            Func<T, VisitPath, bool> visitNoShortCircuit = (node, path) =>
            {
                visit(node, path);
                return false;
            };
            DijkstraFrom(start, visitNoShortCircuit);
        }

        public VisitPath ShortestPathTo(T start, T end)
        {
            return ShortestPathTo(start, n => n.Equals(end));
        }

        public VisitPath ShortestPathTo(T start, Func<T, bool> predicate)
        {
            VisitPath pathToGoal = null;
            DijkstraFrom(start, (node, path) =>
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
    }

    public class ConcreteWeightedGraph<T> : WeightedGraph<T>
    {
        Dictionary<T, List<(T, int)>> Edges;

        public ConcreteWeightedGraph()
        {
            Edges = new Dictionary<T, List<(T, int)>>();
        }

        public override IEnumerable<T> GetNodes()
        {
            return Edges.Keys;
        }

        public override IEnumerable<(T, int)> GetNeighbours(T node)
        {
            return Edges[node];
        }

        public void AddEdge(T from, T to, int dist)
        {
            if (Edges.ContainsKey(from))
            {
                Edges[from].Add((to, dist));
            }
            else
            {
                Edges.Add(from, new List<(T, int)>() { (to, dist) });
            }
            if (!Edges.ContainsKey(to))
            {
                Edges.Add(to, new List<(T, int)>());
            }
        }
    }

    public class WeightedGraphByFunction<T> : WeightedGraph<T> where T : IEquatable<T>
    {
        private Func<T, IEnumerable<(T, int)>> GetEdges;

        public WeightedGraphByFunction(Func<T, IEnumerable<(T, int)>> getEdges)
        {
            GetEdges = getEdges;
        }

        public override IEnumerable<(T, int)> GetNeighbours(T node)
        {
            return GetEdges(node);
        }

        public override IEnumerable<T> GetNodes()
        {
            throw new Exception("Not implemented");
        }
    }
}

