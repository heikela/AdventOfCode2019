using System;
using Common;
using System.Collections.Generic;
using System.Linq;

namespace Day18
{
    class KeyMaze
    {
        Dictionary<IntPoint2D, char> Map;
		ConcreteWeightedGraph<IntPoint2D> PoiGraph;
        public uint AllKeys { get; private set; }

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
                tile == '@';
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

        public KeyMaze(string fileName, bool separateQuadrants = false)
        {
            Map = SparseGrid.ReadFromFile(fileName);
            if (separateQuadrants)
            {
                IntPoint2D middle = GetStartPos().First();
                Map.AddOrSet(middle + new IntPoint2D(-1, -1), '@');
                Map.AddOrSet(middle + new IntPoint2D(1, -1), '@');
                Map.AddOrSet(middle + new IntPoint2D(1, 1), '@');
                Map.AddOrSet(middle + new IntPoint2D(-1, 1), '@');
                Map.AddOrSet(middle + new IntPoint2D(1, 0), '#');
                Map.AddOrSet(middle + new IntPoint2D(-1, 0), '#');
                Map.AddOrSet(middle + new IntPoint2D(0, 1), '#');
                Map.AddOrSet(middle + new IntPoint2D(0, -1), '#');
                Map.AddOrSet(middle, '#');
            }
            AllKeys = 0;

			Dictionary<IntPoint2D, Dictionary<IntPoint2D, int>> mapGraph = new Dictionary<IntPoint2D, Dictionary<IntPoint2D, int>>();

            foreach (IntPoint2D pos in Map.Keys)
            {
                if (CanEverEnter(pos))
                {
					mapGraph.Add(pos, new Dictionary<IntPoint2D, int>());
                    if (IsKey(Map[pos]))
                    {
                        AllKeys = State.AddKey(AllKeys, Map[pos]);
                    }
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

        public IEnumerable<IntPoint2D> GetStartPos()
        {
            return Map.Where(kv => kv.Value == '@').Select(kv => kv.Key);
        }

        public void Print()
        {
            SparseGrid.Print(Map, c => c);
        }

        public IEnumerable<IntPoint2D> OrthogonalNeighbours(IntPoint2D pos)
        {
            return Directions.Select(d => d + pos);
        }

        public IEnumerable<(State, int)> CurrentMovesFrom(State s)
        {
            return PoiGraph
                .GetNeighbours(s.Pos)
                .Where<(IntPoint2D p, int dist)>(neighbour => CanEnter(new State(s, neighbour.p)))
                .Select(neighbour => {
                    State newState;
                    if (IsKey(Map[neighbour.p]))
                    {
                        newState = new State(s, neighbour.p, Map[neighbour.p]);
                    }
                    else
                    {
                        newState = new State(s, neighbour.p);
                    }
                    return (newState, neighbour.dist);
                });
        }

        public IEnumerable<IntPoint2D> EventualMovesFrom(IntPoint2D pos)
        {
            return OrthogonalNeighbours(pos).Where(CanEverEnter);
        }

        private bool CanEnter(State s)
        {
            char tile = GetTile(s.Pos);
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
                return s.CanOpenDoor(tile);
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
    }

    struct State : IEquatable<State>
    {
        public readonly IntPoint2D Pos;
        public readonly uint Keys;

        public State(IntPoint2D pos, uint keys = 0)
        {
            Pos = pos;
            Keys = keys;
        }

        public State(State previous, IntPoint2D newPos)
        {
            Pos = newPos;
            Keys = previous.Keys;
        }

        public State(State previous, IntPoint2D newPos, char additionalKey)
        {
            Pos = newPos;
            Keys = AddKey(previous.Keys, additionalKey);
        }

        public static bool operator ==(State left, State right) =>
        Equals(left, right);

        public static bool operator !=(State left, State right) =>
            !Equals(left, right);

        public override bool Equals(object obj) =>
            (obj is State s) && Equals(s);

        public bool Equals(State other) =>
            (Pos, Keys) == (other.Pos, other.Keys);

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Keys);
        }

        public static uint AddKey(uint keys, char c)
        {
            return keys |= BitMask(c, 'a');
        }

        public bool CanOpenDoor(char c)
        {
            return (Keys & BitMask(c, 'A')) != 0;
        }

        private static uint BitMask(char c, char categoryStart)
        {
            return 1u << (c - categoryStart);
        }

        public bool HasAllKeysIn(uint otherKeys)
        {
            return (otherKeys & (~Keys)) == 0;
        }
    }

    struct FourRobotState : IEquatable<FourRobotState>
    {
        public readonly List<IntPoint2D> Pos;
        public readonly uint Keys;

        public FourRobotState(IEnumerable<IntPoint2D> pos)
        {
            if (pos.Count() != 4)
            {
                throw new Exception("Must supply four robot positions");
            }
            Pos = pos.ToList();
            Keys = 0;
        }

        public FourRobotState(FourRobotState previous, int botToMove, State robotState)
        {
            Pos = previous.Pos.ToList();
            Pos[botToMove] = robotState.Pos;
            Keys = robotState.Keys;
        }

        public State GetRobotState(int robot) {
            return new State(Pos[robot], Keys);
        }

        public static bool operator ==(FourRobotState left, FourRobotState right) =>
        Equals(left, right);

        public static bool operator !=(FourRobotState left, FourRobotState right) =>
            !Equals(left, right);

        public override bool Equals(object obj) =>
            (obj is FourRobotState s) && Equals(s);

        public bool Equals(FourRobotState other) =>
            (Pos, Keys) == (other.Pos, other.Keys);

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos[0], Pos[1], Pos[2], Pos[3], Keys);
        }

        public static uint AddKey(uint keys, char c)
        {
            return keys |= BitMask(c, 'a');
        }

        public bool CanOpenDoor(char c)
        {
            return (Keys & BitMask(c, 'A')) != 0;
        }

        private static uint BitMask(char c, char categoryStart)
        {
            return 1u << (c - categoryStart);
        }

        public bool HasAllKeysIn(uint otherKeys)
        {
            return (otherKeys & (~Keys)) == 0;
        }
    }

    class MazeSolver
    {
        WeightedGraphByFunction<State> InterestingStates;
        KeyMaze Maze;
        State Start;

        public MazeSolver(string fileName)
        {
            Maze = new KeyMaze(fileName);
            Start = new State(Maze.GetStartPos().First());
            InterestingStates = new WeightedGraphByFunction<State>(NextPOIsPossible);
            Maze.Print();
        }

        private IEnumerable<(State, int)> NextPOIsPossible(State s)
        {
            return Maze.CurrentMovesFrom(s);
        }

        public int Solve()
        {
            int bestLength = int.MaxValue;
            InterestingStates.DijkstraFrom(Start,
                (state, earlier) =>
                {
                    if (state.HasAllKeysIn(Maze.AllKeys)) {
                        if (earlier.GetLength() < bestLength)
                        {
                            bestLength = earlier.GetLength();
                        }
                    }
                });
            return bestLength;
        }
    }

    public class FourQuadrantSolver
    {
        WeightedGraphByFunction<FourRobotState> InterestingStates;
        KeyMaze Maze;
        FourRobotState Start;

        public FourQuadrantSolver(string fileName)
        {
            Maze = new KeyMaze(fileName, true);
            Start = new FourRobotState(Maze.GetStartPos());
            InterestingStates = new WeightedGraphByFunction<FourRobotState>(NextPOIsPossible);
            Maze.Print();
        }

        private IEnumerable<(FourRobotState, int)> NextPOIsPossible(FourRobotState s)
        {
            for (int i = 0; i < 4; ++i)
            {
                IEnumerable<(State, int)> movesForRobot = Maze.CurrentMovesFrom(s.GetRobotState(i));
                foreach ((State robotState, int moveLength) move in movesForRobot)
                {
                    yield return (new FourRobotState(s, i, move.robotState), move.moveLength);
                }
            }
        }

        private static int CountBits(uint n)
        {
            int c = 0;
            while (n != 0)
            {
                ++c;
                n = n & (n - 1);
            }
            return c;
        }

        public int Solve()
        {
            return InterestingStates.AStar(Start,
                state => 2 * CountBits(Maze.AllKeys & (~state.Keys)),
                state => state.HasAllKeysIn(Maze.AllKeys));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MazeSolver solver = new MazeSolver("../../../input.txt");
            Console.WriteLine(solver.Solve());
            FourQuadrantSolver solver2 = new FourQuadrantSolver("../../../input.txt");
            Console.WriteLine(solver2.Solve());
        }
    }
}
