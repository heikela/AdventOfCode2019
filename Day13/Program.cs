using System;
using System.IO;
using Common;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day13
{
    class Program
    {
        enum Tile
        {
            Empty,
            Wall,
            Block,
            Paddle,
            Ball
        }

        static char ShowTile(Tile t)
        {
            switch (t)
            {
                case Tile.Empty: return ' ';
                case Tile.Wall: return '\u2588';
                case Tile.Block: return '#';
                case Tile.Paddle: return '_';
                case Tile.Ball: return 'o';
                default: throw new Exception($"Unprintable tile type {t.ToString()}");
            }
        }

        static Dictionary<IntPoint2D, Tile> DrawFrame(string program, Dictionary<IntPoint2D, Tile> prev)
        {
            IntCodeComputer computer = new IntCodeComputer(program);
            Dictionary<IntPoint2D, Tile> screen = new Dictionary<IntPoint2D, Tile>();
            Queue<BigInteger> inputs = new Queue<BigInteger>();
            (bool running, IEnumerable<BigInteger> output) result = computer.RunIntCode(inputs);
            IEnumerator<BigInteger> output = result.output.GetEnumerator();
            while (output.MoveNext()) {
                int x = (int)output.Current;
                if (!output.MoveNext())
                {
                    throw new Exception("Expected output count to be divisible by 3");
                }
                int y = (int)output.Current;
                if (!output.MoveNext())
                {
                    throw new Exception("Expected output count to be divisible by 3");
                }
                Tile tile = (Tile)(int)output.Current;

                screen.AddOrSet(new IntPoint2D(x, y), tile);
            }

            return screen;
        }

        static void Main(string[] args)
        {
            string program = File.ReadLines("../../../input.txt").First();
            Dictionary<IntPoint2D, Tile> screen = DrawFrame(program, new Dictionary<IntPoint2D, Tile>());

            Console.WriteLine($"The frame has {screen.Count(kv => kv.Value == Tile.Block)} blocks at start");
        }
    }
}
