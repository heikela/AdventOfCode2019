using System;
using Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day20
{
    class PortalMaze
    {
        Dictionary<IntPoint2D, char> Map;

        GraphByFunction<IntPoint2D> AvailableMoves;

        Dictionary<IntPoint2D, string> PortalsByPos;
        Dictionary<string, List<IntPoint2D>> PortalsByName;

        static List<IntPoint2D> Directions = new List<IntPoint2D>()
        {
            new IntPoint2D(0, -1),
            new IntPoint2D(-1, 0),
            new IntPoint2D(0, 1),
            new IntPoint2D(1, 0)
        };

        private bool HasPortal(IntPoint2D pos)
        {
            return CanEnter(pos) &&
                OrthogonalNeighbours(pos).Any(p => IsInPortal(GetTile(p)));
        }

        private string GetPortal(IntPoint2D pos)
        {
            foreach (IntPoint2D dir in Directions)
            {
                List<char> result = new List<char>();
                Action<char> add = c => result.Prepend(c);
                if (dir.X >= 0 && dir.Y >= 0)
                {
                    add = c => result.Add(c);
                }
                else
                {
                    add = c => result.Insert(0, c);
                }
                IntPoint2D p = pos;
                for (int i = 0; i < 2; ++i)
                {
                    p = p + dir;
                    char tile = GetTile(p);
                    if (IsInPortal(tile))
                    {
                        add(tile);
                    }
                    else
                    {
                        break;
                    }
                }
                if (result.Count == 2)
                {
                    return new string(result.ToArray());
                }
            }
            return null;
        }

        private bool CanEnter(IntPoint2D pos)
        {
            return GetTile(pos) == '.';
        }

        public bool IsInPortal(char tile)
        {
            return tile >= 'A' && tile <= 'Z';
        }

        public PortalMaze(string fileName)
        {
            Map = SparseGrid.ReadFromFile(fileName);
            PortalsByPos = new Dictionary<IntPoint2D, string>();
            PortalsByName = new Dictionary<string, List<IntPoint2D>>();

            foreach (IntPoint2D pos in Map.Keys)
            {
                if (HasPortal(pos))
                {
                    string portal = GetPortal(pos);
                    PortalsByPos.Add(pos, portal);
                    if (!PortalsByName.ContainsKey(portal))
                    {
                        PortalsByName.Add(portal, new List<IntPoint2D>());
                    }
                    PortalsByName[portal].Add(pos);
                }
            }

            AvailableMoves = new GraphByFunction<IntPoint2D>(MovesFrom);
        }

        public IntPoint2D GetStartPos()
        {
            return PortalsByName["AA"].First();
        }

        public IntPoint2D GetEndPos()
        {
            return PortalsByName["ZZ"].First();
        }

        public void Print()
        {
            SparseGrid.Print(Map, c => c);
        }

        public IEnumerable<IntPoint2D> OrthogonalNeighbours(IntPoint2D pos)
        {
            return Directions.Select(d => d + pos);
        }

        private IEnumerable<IntPoint2D> NonPortalMovesFrom(IntPoint2D pos)
        {
            return OrthogonalNeighbours(pos).Where(CanEnter);
        }

        private IEnumerable<IntPoint2D> MovesFrom(IntPoint2D pos)
        {
            if (PortalsByPos.ContainsKey(pos))
            {
                return NonPortalMovesFrom(pos).Concat(PortalsByName[PortalsByPos[pos]]);
            }
            else
            {
                return NonPortalMovesFrom(pos);
            }
        }

        public char GetTile(IntPoint2D pos)
        {
            return Map.GetOrElse(pos, '#');
        }

        public int Solve()
        {
            return AvailableMoves.ShortestPathTo(GetStartPos(), GetEndPos()).GetLength();
        }
    }

    class RecursiveMaze
    {
        Dictionary<IntPoint2D, char> Map;

        GraphByFunction<(IntPoint2D, int)> AvailableMoves;

        Dictionary<IntPoint2D, string> PortalsByPos;
        Dictionary<string, List<IntPoint2D>> PortalsByName;

        int MidX, MidY, XTolerance, YTolerance;

        static List<IntPoint2D> Directions = new List<IntPoint2D>()
        {
            new IntPoint2D(0, -1),
            new IntPoint2D(-1, 0),
            new IntPoint2D(0, 1),
            new IntPoint2D(1, 0)
        };

        private bool HasPortal(IntPoint2D pos)
        {
            return CanEnter(pos) &&
                OrthogonalNeighbours(pos).Any(p => IsInPortal(GetTile(p)));
        }

        private string GetPortal(IntPoint2D pos)
        {
            foreach (IntPoint2D dir in Directions)
            {
                List<char> result = new List<char>();
                Action<char> add = c => result.Prepend(c);
                if (dir.X >= 0 && dir.Y >= 0)
                {
                    add = c => result.Add(c);
                }
                else
                {
                    add = c => result.Insert(0, c);
                }
                IntPoint2D p = pos;
                for (int i = 0; i < 2; ++i)
                {
                    p = p + dir;
                    char tile = GetTile(p);
                    if (IsInPortal(tile))
                    {
                        add(tile);
                    }
                    else
                    {
                        break;
                    }
                }
                if (result.Count == 2)
                {
                    return new string(result.ToArray());
                }
            }
            return null;
        }

        private bool CanEnter(IntPoint2D pos)
        {
            return GetTile(pos) == '.';
        }

        public bool IsInPortal(char tile)
        {
            return tile >= 'A' && tile <= 'Z';
        }

        public RecursiveMaze(string fileName)
        {
            Map = SparseGrid.ReadFromFile(fileName);
            PortalsByPos = new Dictionary<IntPoint2D, string>();
            PortalsByName = new Dictionary<string, List<IntPoint2D>>();

            foreach (IntPoint2D pos in Map.Keys)
            {
                if (HasPortal(pos))
                {
                    string portal = GetPortal(pos);
                    PortalsByPos.Add(pos, portal);
                    if (!PortalsByName.ContainsKey(portal))
                    {
                        PortalsByName.Add(portal, new List<IntPoint2D>());
                    }
                    PortalsByName[portal].Add(pos);
                }
            }

            AvailableMoves = new GraphByFunction<(IntPoint2D, int)>(MovesFrom);
            MidX = Map.Keys.Max(p => p.X) / 2;
            MidY = Map.Keys.Max(p => p.Y) / 2;
            XTolerance = MidX * 4 / 5;
            YTolerance = MidY * 4 / 5;
        }

        public (IntPoint2D, int) GetStartPos()
        {
            return (PortalsByName["AA"].First(), 0);
        }

        public (IntPoint2D, int) GetEndPos()
        {
            return (PortalsByName["ZZ"].First(), 0);
        }

        public void Print()
        {
            SparseGrid.Print(Map, c => c);
        }

        public IEnumerable<IntPoint2D> OrthogonalNeighbours(IntPoint2D pos)
        {
            return Directions.Select(d => d + pos);
        }

        private IEnumerable<(IntPoint2D, int)> NonPortalMovesFrom((IntPoint2D pos, int level) x)
        {
            return OrthogonalNeighbours(x.pos).Where(CanEnter).Select(p => (p, x.level));
        }

        private IEnumerable<(IntPoint2D, int)> PortalMovesFrom((IntPoint2D pos, int level) x)
        {
            IEnumerable<IntPoint2D> potentialDestinations = PortalsByName[PortalsByPos[x.pos]].Where(pos => pos != x.pos);
            if (!potentialDestinations.Any())
            {
                yield break;
            }
            IntPoint2D other = potentialDestinations.First();
            if (IsInner(x.pos))
            {
                yield return (other, x.level + 1);
            }
            else
            {
                if (x.level > 0)
                {
                    yield return (other, x.level - 1);
                }
            }
            yield break;
        }

        private bool IsInner(IntPoint2D pos)
        {
            return Math.Abs(pos.X - MidX) < XTolerance && Math.Abs(pos.Y - MidY) < YTolerance;
        }

        private IEnumerable<(IntPoint2D, int)> MovesFrom((IntPoint2D pos, int level) x)
        {
            if (PortalsByPos.ContainsKey(x.pos))
            {
                return NonPortalMovesFrom(x).Concat(PortalMovesFrom(x));
            }
            else
            {
                return NonPortalMovesFrom(x);
            }
        }

        public char GetTile(IntPoint2D pos)
        {
            return Map.GetOrElse(pos, '#');
        }

        public int Solve()
        {
            return AvailableMoves.ShortestPathTo(GetStartPos(), GetEndPos()).GetLength();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PortalMaze maze = new PortalMaze("../../../input.txt");
//            maze.Print();
            Console.WriteLine($"Part 1 {maze.Solve()}");
            RecursiveMaze maze2 = new RecursiveMaze("../../../input.txt");
            Console.WriteLine($"Part 2 {maze2.Solve()}");
        }
    }
}
