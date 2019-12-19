﻿using System;
using Common;
using System.Collections.Generic;
using System.Linq;

namespace Day18
{
    class KeyCollection : IEquatable<KeyCollection>
    {
        IntPoint2D Pos;
        int MovesSpent;
        HashSet<char> CollectedKeys;

        public KeyCollection(IntPoint2D pos)
        {
            Pos = pos;
            MovesSpent = 0;
            CollectedKeys = new HashSet<char>();
        }

        public KeyCollection(KeyCollection previous, IntPoint2D pos, int additionalMoves, char additionalKey)
        {
            Pos = pos;
            MovesSpent = previous.MovesSpent + additionalMoves;
            CollectedKeys = previous.CollectedKeys.ToHashSet();
            CollectedKeys.Add(additionalKey);
        }

        public HashSet<char> GetKeys()
        {
            return CollectedKeys;
        }

        public IntPoint2D GetPos()
        {
            return Pos;
        }

        public int GetMoves()
        {
            return MovesSpent;
        }

        bool IEquatable<KeyCollection>.Equals(KeyCollection other)
        {
            return other.Pos == Pos
                && other.MovesSpent == MovesSpent
                && other.CollectedKeys.SetEquals(CollectedKeys);
        }
    }

    class KeyFinder
    {
        KeyCollection Current;
        KeyMaze Maze;
        GraphByFunction<IntPoint2D> AvailableRoutes;

        public KeyFinder(KeyCollection current, KeyMaze maze)
        {
            Current = current;
            Maze = maze;
            AvailableRoutes = new GraphByFunction<IntPoint2D>(p => Maze.CurrentMovesFrom(p, Current.GetKeys()));
        }

        public IEnumerable<(KeyCollection, int)> PossibleNextKeys()
        {
            List<(KeyCollection, int)> foundKeys = new List<(KeyCollection, int)>();
            AvailableRoutes.BfsFrom(Current.GetPos(),
                (pos, path) => {
                    char newKey = Maze.GetTile(pos);
                    if (Maze.IsKey(newKey) && !Current.GetKeys().Contains(newKey))
                    {
                        foundKeys.Add((
                            new KeyCollection(Current, pos, path.GetLength(), newKey),
                            path.GetLength()));
                    }
                });
            return foundKeys;
        }
    }

    class KeyMaze
    {
        Dictionary<IntPoint2D, char> Map;
        ConcreteWeightedGraph<(IntPoint2D pos, char tile)> SimplifiedMap;

        static List<IntPoint2D> Directions = new List<IntPoint2D>()
        {
            new IntPoint2D(0, -1),
            new IntPoint2D(0, 1),
            new IntPoint2D(-1, 0),
            new IntPoint2D(1, 0)
        };

        private bool IsPOI(IntPoint2D pos)
        {
            char tile = GetTile(pos);
            return IsKey(tile) || IsDoor(tile) ||
                tile == '@' ||
                (tile == '.' && OrthogonalNeighbours(pos).Count(p => CanEverEnter(p)) > 2);
        }

        private bool CanEverEnter(IntPoint2D pos)
        {
            return GetTile(pos) != '#';
        }

        public bool IsKey(char tile)
        {
            return tile >= 'a' && tile <= 'z';
        }

        private bool IsDoor(char tile)
        {
            return tile >= 'A' && tile <= 'Z';
        }

        public KeyMaze(string fileName)
        {
            Map = SparseGrid.ReadFromFile(fileName);
            SimplifiedMap = new ConcreteWeightedGraph<(IntPoint2D pos, char tile)>();

            Dictionary<(IntPoint2D, char), List<((IntPoint2D, char), int)>> MapGraph = new Dictionary<(IntPoint2D, char), List<((IntPoint2D, char), int)>>();

            foreach (IntPoint2D pos in Map.Keys)
            {
                if (CanEverEnter(pos))
                {
                    foreach (IntPoint2D neighbour in EventualMovesFrom(pos))
                    {
                        MapGraph.Add((pos, Map[pos]), ((neighbour, Map[neighbour]), 1));
                    }
                }
            }

            while (MapGraph.Any(kv => !IsPOI(kv.Key.Item1)))
            {
                KeyValuePair<(IntPoint2D pos, IntPoint2D tile), List<((IntPoint2D, char), int)>> toSimplify = MapGraph.First(kv => !IsPOI(kv.Key.Item1));
            }

            HashSet<IntPoint2D> POIPositions = new HashSet<IntPoint2D>();
            POIPositions.Add(GetStartPos());

            GraphByFunction<IntPoint2D> EventualMoveGraph = new GraphByFunction<IntPoint2D>(EventualMovesFrom);

            EventualMoveGraph.BfsFrom()
        }

        public IntPoint2D GetStartPos()
        {
            return Map.First(kv => kv.Value == '@').Key;
        }

        public void Print()
        {
            SparseGrid.Print(Map, c => c);
        }

        public IEnumerable<IntPoint2D> OrthogonalNeighbours(IntPoint2D pos)
        {
            return Directions.Select(d => d + pos);
        }

        public IEnumerable<IntPoint2D> CurrentMovesFrom(IntPoint2D pos, HashSet<char> collectedKeys)
        {
            return OrthogonalNeighbours(pos).Where(p => CanEnter(p, collectedKeys));
        }

        public IEnumerable<IntPoint2D> EventualMovesFrom(IntPoint2D pos)
        {
            return OrthogonalNeighbours(pos).Where(CanEverEnter);
        }

        private bool CanEnter(IntPoint2D p, HashSet<char> collectedKeys)
        {
            char tile = GetTile(p);
            if (tile == '.')
            {
                return true;
            }
            if (tile == '#')
            {
                return false;
            }
            if (IsKey(tile))
            {
                return true;
            }
            if (tile >= 'A' && tile <= 'Z')
            {
                return collectedKeys.Contains(tile.ToString().ToLower()[0]);
            }
            if (tile == '@')
            {
                return true;
            }
            throw new Exception($"Unknown tile type '{tile}'");
        }

        public char GetTile(IntPoint2D pos)
        {
            return Map.GetOrElse(pos, '#');
        }

        public IEnumerable<char> GetAllKeys()
        {
            return Map.Values.Where(IsKey);
        }
    }

    class MazeSolver
    {
        WeightedGraphByFunction<KeyCollection> KeyCollectionOrders;
        KeyMaze Maze;
        KeyCollection Start;

        public MazeSolver(string fileName)
        {
            Maze = new KeyMaze(fileName);
            Start = new KeyCollection(Maze.GetStartPos());
            KeyCollectionOrders = new WeightedGraphByFunction<KeyCollection>(NextKeysPossible);
            Maze.Print();
            throw new Exception("Exiting early");
        }

        IEnumerable<(KeyCollection, int)> NextKeysPossible(KeyCollection current)
        {
            return new KeyFinder(current, Maze).PossibleNextKeys();
        }

        public int Solve()
        {
            int bestLength = int.MaxValue;
            KeyCollectionOrders.DijkstraFrom(Start,
                (keys, earlier) =>
                {
                    if (keys.GetKeys().IsSupersetOf(Maze.GetAllKeys())) {
                        if (keys.GetMoves() < bestLength)
                        {
                            bestLength = keys.GetMoves();
                        }
                    }
                });
            return bestLength;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MazeSolver solver = new MazeSolver("../../../input.txt");
            Console.WriteLine(solver.Solve());
        }
    }
}
