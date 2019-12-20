using System;
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
        WeightedGraphByFunction<IntPoint2D> AvailableRoutes;

        public KeyFinder(KeyCollection current, KeyMaze maze)
        {
            Current = current;
            Maze = maze;
            AvailableRoutes = new WeightedGraphByFunction<IntPoint2D>(p => Maze.CurrentMovesFrom(p, Current.GetKeys()));
        }

        public IEnumerable<(KeyCollection, int)> PossibleNextKeys()
        {
            List<(KeyCollection, int)> foundKeys = new List<(KeyCollection, int)>();
            AvailableRoutes.DijkstraFrom(Current.GetPos(),
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
		ConcreteWeightedGraph<IntPoint2D> PoiGraph;

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

			Dictionary<IntPoint2D, Dictionary<IntPoint2D, int>> mapGraph = new Dictionary<IntPoint2D, Dictionary<IntPoint2D, int>>();

            foreach (IntPoint2D pos in Map.Keys)
            {
                if (CanEverEnter(pos))
                {
					mapGraph.Add(pos, new Dictionary<IntPoint2D, int>());
					foreach (IntPoint2D neighbour in EventualMovesFrom(pos))
                    {
                        mapGraph[pos].Add(neighbour, 1);
                    }
                }
            }

            while (mapGraph.Keys.Any(pos => !IsPOI(pos)))
            {
                IntPoint2D toSimplify = mapGraph.Keys.First(pos => !IsPOI(pos));
				// Here we assume summetry, which we have
				List<KeyValuePair<IntPoint2D, int>> neighbours = mapGraph[toSimplify].ToList();
				int neighbourCount = neighbours.Count;
                for (int i = 0; i < neighbourCount; ++i)
				{
                    (IntPoint2D pos1, int dist1) = neighbours[i];
                    for (int j = i + 1; j < neighbourCount; ++j)
					{
						(IntPoint2D pos2, int dist2) = neighbours[j];
						int existingDist = mapGraph[pos1].GetOrElse(pos2, int.MaxValue);
						int distanceThroughCurrent = dist1 + dist2;
                        if (distanceThroughCurrent < existingDist)
						{
							mapGraph[pos1].AddOrSet(pos2, distanceThroughCurrent);
							mapGraph[pos2].AddOrSet(pos1, distanceThroughCurrent);
						}
					}
					mapGraph[pos1].Remove(toSimplify);
				}
                mapGraph.Remove(toSimplify);
			}

			PoiGraph = new ConcreteWeightedGraph<IntPoint2D>(mapGraph);
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

        public IEnumerable<(IntPoint2D pos, int dist)> CurrentMovesFrom(IntPoint2D pos, HashSet<char> collectedKeys)
        {
            return PoiGraph.GetNeighbours(pos).Where<(IntPoint2D p, int dist)>(neighbour => CanEnter(neighbour.p, collectedKeys));
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
