using System;
using Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day15
{
    enum Tile
    {
        Start,
        Room,
        Wall,
        OxygenSystem
    }

    class Program
    {
        static Dictionary<IntPoint2D, Tile> Map = new Dictionary<IntPoint2D, Tile>();
        static IntPoint2D Start = new IntPoint2D();
        static IntPoint2D Pos = Start;
        static int LongestDistance = int.MinValue;
        static IntPoint2D? OxygenPos = null;
        static Graph<IntPoint2D> KnownMapGraph = new GraphByFunction<IntPoint2D>(KnownEdges);
        static Graph<IntPoint2D> ExplorationGraph = new GraphByFunction<IntPoint2D>(ExplorationEdges);

        static List<IntPoint2D> Moves = new List<IntPoint2D>()
        {
            new IntPoint2D(0, -1),
            new IntPoint2D(0, 1),
            new IntPoint2D(-1, 0),
            new IntPoint2D(1, 0)
        };

        static char ShowTile(Tile t)
        {
            switch (t)
            {
                case Tile.OxygenSystem: return 'O';
                case Tile.Room: return '.';
                case Tile.Wall: return '#';
                case Tile.Start: return 'S';
                default: return '?';
            }
        }

        static IEnumerable<IntPoint2D> ExplorationEdges(IntPoint2D pos)
        {
            if (!Map.ContainsKey(pos))
            {
                yield break;
            } else
            {
                foreach (IntPoint2D potentialMove in Moves)
                {
                    IntPoint2D neighbour = pos + potentialMove;
                    if (Map.GetOrElse(neighbour, Tile.Room) != Tile.Wall)
                    {
                        yield return neighbour;
                    }
                }
                yield break;
            }
        }

        static IEnumerable<IntPoint2D> KnownEdges(IntPoint2D pos)
        {
            if (!Map.ContainsKey(pos))
            {
                yield break;
            }
            else
            {
                foreach (IntPoint2D potentialMove in Moves)
                {
                    IntPoint2D neighbour = pos + potentialMove;
                    if (Map.GetOrElse(neighbour, Tile.Wall) != Tile.Wall)
                    {
                        yield return neighbour;
                    }
                }
                yield break;
            }
        }

        static void LongestDistanceFromOxygen(IntPoint2D pos, Graph<IntPoint2D>.VisitPath path)
        {
            if (LongestDistance < path.GetLength())
            {
                LongestDistance = path.GetLength();
            }
        }

        static IEnumerable<IntPoint2D> RouteToUnexploredNearestToStart()
        {
            Graph<IntPoint2D>.VisitPath startToUnexplored =
                ExplorationGraph.ShortestPathTo(Start, pos => !Map.ContainsKey(pos));
            if (startToUnexplored == null)
            {
                return new List<IntPoint2D>();
            }
            int startToUnexploredLen = startToUnexplored.GetLength();
            IntPoint2D target = startToUnexplored.GetNodesOnPath().Last();
            IntPoint2D lastKnownPoint = startToUnexploredLen > 1 ?
                startToUnexplored.GetNodesOnPath().ElementAt(startToUnexploredLen - 2) :
                Start;
            return KnownMapGraph.ShortestPathTo(Pos, lastKnownPoint).GetNodesOnPath().Append(target);
        }

        static void NavigatePath(IEnumerable<IntPoint2D> path, IntCodeComputer droidControl)
        {
            foreach (IntPoint2D step in path)
            {
                Queue<BigInteger> input = new Queue<BigInteger>();
                IntPoint2D direction = step - Pos;
                input.Enqueue(Moves.IndexOf(direction) + 1);
                var result = droidControl.RunIntCodeV11(input);
                if (result.output.Count != 1)
                {
                    throw new Exception($"Expected one status code as output, got {result.output.Count}");
                }
                BigInteger status = result.output[0];
                switch ((int)status)
                {
                    case 0:
                        {
                            Map.Add(step, Tile.Wall);
                            break;
                        }
                    case 1:
                        {
                            Map.AddOrSet(step, Tile.Room);
                            Pos = step;
                            break;
                        }
                    case 2:
                        {
                            Map.AddOrSet(step, Tile.OxygenSystem);
                            Pos = step;
                            OxygenPos = Pos;
                            break;
                        }
                    default:
                        throw new Exception($"Unexpected status code {status}");
                }
            }
        }

        static void Main(string[] args)
        {
            IntCodeComputer droidControl = new IntCodeComputer(File.ReadLines("../../../input.txt").First());

            Map.Add(Start, Tile.Start);

            IEnumerable<IntPoint2D> pathToUnexplored = RouteToUnexploredNearestToStart();

            while (OxygenPos == null)
            {
                NavigatePath(pathToUnexplored, droidControl);
//                SparseGrid.Print(Map, ShowTile);
                pathToUnexplored = RouteToUnexploredNearestToStart();
            }

            Console.WriteLine($"The shortest path to oxygen is {KnownMapGraph.ShortestPathTo(Start, OxygenPos.Value).GetLength()} steps");

            while (pathToUnexplored.Any())
            {
                NavigatePath(pathToUnexplored, droidControl);
//                SparseGrid.Print(Map, ShowTile);
                pathToUnexplored = RouteToUnexploredNearestToStart();
            }

            /* The task in part two reveals that the maze is not infinite. This means
             * that an exhaustive search would be a possible way to solve part 1 as well,
             * but I have gone for an approach that would solve part 1 even
             * in an infinite maze */

            KnownMapGraph.BfsFrom(OxygenPos.Value, LongestDistanceFromOxygen);
            Console.WriteLine($"After exploring the map, the longest distance to oxygen is {LongestDistance} steps");
        }
    }
}
