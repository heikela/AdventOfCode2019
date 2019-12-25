using System;
using System.IO;
using Common;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day11
{
    class Program
    {
        static Dictionary<IntPoint2D, int> RunPaintRobot(string program, Dictionary<IntPoint2D, int> hullAtStart)
        {
            IntCodeComputer computer = new IntCodeComputer(program);
            Dictionary<IntPoint2D, int> hull = new Dictionary<IntPoint2D, int>(hullAtStart);
            Queue<BigInteger> inputs = new Queue<BigInteger>();
            IntPoint2D position = new IntPoint2D();
            List<IntPoint2D> directions = new List<IntPoint2D>()
            {
                new IntPoint2D(0, -1),
                new IntPoint2D(1, 0),
                new IntPoint2D(0, 1),
                new IntPoint2D(-1, 0)
            };
            int directionIndex = 0;
            (bool running, IEnumerable<BigInteger> output) result;
            do
            {
                inputs.Enqueue(hull.GetOrElse(position, 0));
                result = computer.RunIntCodeV11(inputs);
                if (result.output.Count() != 2)
                {
                    throw new Exception("Expected a painting and a movement command");
                }
                hull.AddOrSet(position, (int)result.output.ElementAt(0));
                BigInteger turn = result.output.ElementAt(1);
                if (turn == 0)
                {
                    --directionIndex;
                }
                else if (turn == 1)
                {
                    ++directionIndex;
                }
                directionIndex = (directionIndex + directions.Count) % directions.Count;
                position += directions[directionIndex];
            } while (result.running);

            return hull;
        }

        static void Main(string[] args)
        {
            string program = File.ReadLines("../../../input.txt").First();
            Dictionary<IntPoint2D, int> hull = RunPaintRobot(program, new Dictionary<IntPoint2D, int>());

            Console.WriteLine($"The robot painted {hull.Count} positions");

            hull = RunPaintRobot(program, new Dictionary<IntPoint2D, int>() { { new IntPoint2D(), 1 } });

            SparseGrid.Print(hull, i => i == 1 ? '#' : ' ');
        }
    }
}
