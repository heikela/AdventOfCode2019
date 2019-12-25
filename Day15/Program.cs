﻿using System;
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
        static IntPoint2D Pos = new IntPoint2D();
        static int ShortestDistance = int.MaxValue;
        static int LongestDistance = int.MinValue;
        static IntPoint2D? OxygenPos = null;

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

        static void Main(string[] args)
        {
            IntCodeComputer droidControl = new IntCodeComputer(File.ReadLines("../../../input.txt").First());

            Map.Add(Pos, Tile.Start);

            List<IntPoint2D> pathToUnexplored = null;

            Graph<IntPoint2D> knownMapGraph = new GraphByFunction<IntPoint2D>(KnownEdges);
            Graph<IntPoint2D> explorationGraph = new GraphByFunction<IntPoint2D>(ExplorationEdges);
            pathToUnexplored = explorationGraph.ShortestPathTo(Pos, pos => !Map.ContainsKey(pos)).GetNodesOnPath().ToList();

            (bool running, List<BigInteger> output) result;

            while (OxygenPos == null)
            {
                while (pathToUnexplored.Any())
                {
                    Queue<BigInteger> input = new Queue<BigInteger>();
                    IntPoint2D stepTarget = pathToUnexplored.First();
                    pathToUnexplored.RemoveAt(0);
                    IntPoint2D direction = stepTarget - Pos;
                    input.Enqueue(Moves.IndexOf(direction) + 1);
                    result = droidControl.RunIntCodeV11(input);
                    if (result.output.Count != 1)
                    {
                        throw new Exception($"Expected one status code as output, got {result.output.Count}");
                    }
                    BigInteger status = result.output[0];
                    switch ((int)status)
                    {
                        case 0:
                            {
                                Map.Add(stepTarget, Tile.Wall);
                                break;
                            }
                        case 1:
                            {
                                Map.AddOrSet(stepTarget, Tile.Room);
                                Pos = stepTarget;
                                break;
                            }
                        case 2:
                            {
                                Map.AddOrSet(stepTarget, Tile.OxygenSystem);
                                Pos = stepTarget;
                                OxygenPos = stepTarget;
                                break;
                            }
                        default:
                            throw new Exception($"Unexpected status code {status}");
                    }
                }
                SparseGrid.Print(Map, ShowTile);

                pathToUnexplored = explorationGraph.ShortestPathTo(Pos, pos => !Map.ContainsKey(pos)).GetNodesOnPath().ToList();
            }

            Console.WriteLine($"Among the paths randomly explored, the shortest to oxygen is {knownMapGraph.ShortestPathTo(new IntPoint2D(), OxygenPos.Value).GetLength()} steps");

            while (pathToUnexplored != null)
            {
                while (pathToUnexplored.Any())
                {
                    Queue<BigInteger> input = new Queue<BigInteger>();
                    IntPoint2D stepTarget = pathToUnexplored.First();
                    pathToUnexplored.RemoveAt(0);
                    IntPoint2D direction = stepTarget - Pos;
                    input.Enqueue(Moves.IndexOf(direction) + 1);
                    result = droidControl.RunIntCodeV11(input);
                    if (result.output.Count != 1)
                    {
                        throw new Exception($"Expected one status code as output, got {result.output.Count}");
                    }
                    BigInteger status = result.output[0];
                    switch ((int)status)
                    {
                        case 0:
                            {
                                Map.Add(stepTarget, Tile.Wall);
                                break;
                            }
                        case 1:
                            {
                                Map.AddOrSet(stepTarget, Tile.Room);
                                Pos = stepTarget;
                                break;
                            }
                        case 2:
                            {
                                Map.AddOrSet(stepTarget, Tile.OxygenSystem);
                                Pos = stepTarget;
                                OxygenPos = stepTarget;
                                break;
                            }
                        default:
                            throw new Exception($"Unexpected status code {status}");
                    }
                }
                SparseGrid.Print(Map, ShowTile);

                pathToUnexplored = explorationGraph.ShortestPathTo(Pos, pos => !Map.ContainsKey(pos))?.GetNodesOnPath()?.ToList();
            }

            knownMapGraph.BfsFrom(OxygenPos.Value, LongestDistanceFromOxygen);
            Console.WriteLine($"After exploring the map, the longest distance to oxygen is {LongestDistance} steps");
        }
    }
}
