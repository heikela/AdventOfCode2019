using System;

using Common;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day17
{
    class Program
    {
        static Dictionary<IntPoint2D, char> Routes = new Dictionary<IntPoint2D, char>();
        static IntPoint2D Pos;
        static IntPoint2D Dir;

        static List<IntPoint2D> Directions = new List<IntPoint2D>()
        {
            new IntPoint2D(0, -1),
            new IntPoint2D(1, 0),
            new IntPoint2D(0, 1),
            new IntPoint2D(-1, 0)
        };

        static bool IsIntersection(IntPoint2D pos)
        {
            return Directions.All(d => Routes.ContainsKey(pos + d));
        }

        static int Alignment(IntPoint2D pos)
        {
            return pos.X * pos.Y;
        }

        static IntPoint2D LeftDir(IntPoint2D currentDir)
        {
            return Directions[(Directions.FindIndex(d => d == currentDir) + 3) % 4];
        }

        static IntPoint2D RightDir(IntPoint2D currentDir)
        {
            return Directions[(Directions.FindIndex(d => d == currentDir) + 1) % 4];
        }

        static IEnumerable<string> RouteWithFewestTurns()
        {
            if (!Routes.ContainsKey(Pos + Dir))
            {
                if (Routes.ContainsKey(Pos + RightDir(Dir)))
                {
                    yield return "R";
                    Dir = RightDir(Dir);
                }
                else if (Routes.ContainsKey(Pos + LeftDir(Dir)))
                {
                    yield return "L";
                    Dir = LeftDir(Dir);
                }
                else if (Routes.ContainsKey(Pos - Dir))
                {
                    yield return "R";
                    yield return "R";
                    Dir = new IntPoint2D() - Dir;
                }
            }
            while (true)
            {
                int moveLength = 0;
                while (Routes.ContainsKey(Pos + Dir))
                {
                    moveLength++;
                    Pos = Pos + Dir;
                }
                yield return moveLength.ToString();
                if (Routes.ContainsKey(Pos + RightDir(Dir)))
                {
                    yield return "R";
                    Dir = RightDir(Dir);
                }
                else if (Routes.ContainsKey(Pos + LeftDir(Dir)))
                {
                    yield return "L";
                    Dir = LeftDir(Dir);
                }
                else
                {
                    yield break;
                }
            }
        }

        static void Main(string[] args)
        {
            IntCodeComputer computer = new IntCodeComputer(File.ReadLines("../../../input.txt").First());
            (bool running, List<BigInteger> output) result = computer.RunIntCode(new Queue<BigInteger>());
            int y = 0;
            int x = 0;
            foreach (var n in result.output)
            {
                switch ((char)n)
                {
                    case 'X':
                    case '.':
                        break;
                    case '<':
                        Routes.Add(new IntPoint2D(x, y), (char)n);
                        Pos = new IntPoint2D(x, y);
                        Dir = new IntPoint2D(-1, 0);
                        break;
                    case '>':
                        Routes.Add(new IntPoint2D(x, y), (char)n);
                        Pos = new IntPoint2D(x, y);
                        Dir = new IntPoint2D(1, 0);
                        break;
                    case 'V':
                        Routes.Add(new IntPoint2D(x, y), (char)n);
                        Pos = new IntPoint2D(x, y);
                        Dir = new IntPoint2D(0, 1);
                        break;
                    case '^':
                        Routes.Add(new IntPoint2D(x, y), (char)n);
                        Pos = new IntPoint2D(x, y);
                        Dir = new IntPoint2D(0, -1);
                        break;
                    case '#':
                        Routes.Add(new IntPoint2D(x, y), (char)n);
                        break;
                    case '\n':
                        y++;
                        x = -1;
                        break;
                    default:
                        throw new Exception();
                }
                ++x;
            }

            Console.WriteLine($"Calibration {Routes.Keys.Where(IsIntersection).Select(Alignment).Sum()}");

            SparseGrid.Print(Routes, c => c);

            List<string> commands = RouteWithFewestTurns().ToList();

            Console.WriteLine("Commands");
            Console.WriteLine(string.Join(",", commands));

            int bestsaving = 0;
            int bestStart = -1;
            int bestEnd = -1;
            for (int start = 0; start < commands.Count - 1; ++start)
            {
                for (int end = start + 2; end <= commands.Count; ++end)
                {
                    int length = end - start;
                    int repeats = 0;
                    for (int potentialRepeatStart = end; potentialRepeatStart + length <= commands.Count; ++potentialRepeatStart)
                    {
                        bool repeatingSoFar = true;
                        for (int repeatPos = 0; repeatPos < length; ++repeatPos)
                        {
                            if (commands[start + repeatPos] != commands[potentialRepeatStart + repeatPos])
                            {
                                repeatingSoFar = false;
                                break;
                            }
                        }
                        if (repeatingSoFar)
                        {
                            repeats++;
                            potentialRepeatStart += length - 1;
                        }
                    }
                    if (repeats * length > bestsaving)
                    {
                        bestsaving = repeats * length;
                        bestStart = start;
                        bestEnd = end;
                    }
                }
            }
            Console.WriteLine($"Found repeat between elements {bestStart}, {bestEnd}");
            Console.WriteLine(string.Join(",", commands.Skip(bestStart).Take(bestEnd - bestStart)));

            IntCodeComputer controlComputer = new IntCodeComputer("2" + File.ReadLines("../../../input.txt").First().Substring(1));
            List<string> program = new List<string>()
            {
                "A,B,A,C,B,C,B,C,A,C",
                "R,12,L,6,R,12",
                "L,8,L,6,L,10",
                "R,12,L,10,L,6,R,10",
                "N"
            };
            IEnumerable<char> inputChars = program.SelectMany(s => s.AsEnumerable().Append((char)10));
            Queue<BigInteger> input = new Queue<BigInteger>();
            foreach (var c in inputChars)
            {
                input.Enqueue(c);
            }
            result = controlComputer.RunIntCode(input);
            foreach (BigInteger n in result.output)
            {
                Console.WriteLine($"Control program output = {n}");
            }
        }
    }
}
