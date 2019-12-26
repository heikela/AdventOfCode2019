using System;
using System.IO;
using Common;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day13
{
    class ArcadeMachine
    {
        IntCodeComputer Computer;
        Dictionary<IntPoint2D, Tile> Screen;
        BigInteger Score;
        BigInteger BallX;
        BigInteger PaddleX;

        public ArcadeMachine(string program, bool hacked = false)
        {
            Computer = new IntCodeComputer(program);
            Screen = new Dictionary<IntPoint2D, Tile>();
            Score = 0;
            BallX = 0;
            PaddleX = 0;
            if (hacked)
            {
                Computer.Patch(0, 2);
            }
        }

        public enum Tile
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

        public Dictionary<IntPoint2D, Tile> DrawFrame()
        {
            Queue<BigInteger> inputs = new Queue<BigInteger>();
            (bool running, IEnumerable<BigInteger> output) result = Computer.RunIntCodeV11(inputs);
            IEnumerator<BigInteger> output = result.output.GetEnumerator();
            UpdateScreen(output);

            return Screen;
        }

        void UpdateScreen(IEnumerator<BigInteger> output)
        {
            while (output.MoveNext())
            {
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
                if (x == -1)
                {
                    Score = output.Current;
                }
                else
                {
                    Tile tile = (Tile)(int)output.Current;

                    Screen.AddOrSet(new IntPoint2D(x, y), tile);
                }
            }
        }

        int RemainingBlocks()
        {
            return Screen.Count(kv => kv.Value == Tile.Block);
        }

        void AnalyzeFrame()
        {
            BallX = Screen.First(kv => kv.Value == Tile.Ball).Key.X;
            PaddleX = Screen.First(kv => kv.Value == Tile.Paddle).Key.X;
        }

        void ShowFrame()
        {
            SparseGrid.Print(Screen, ShowTile);
        }

        BigInteger DecideInput()
        {
            if (BallX < PaddleX)
            {
                return -1;
            } else if (BallX > PaddleX)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        Queue<BigInteger> MakeInput()
        {
            Queue<BigInteger> input = new Queue<BigInteger>();
            input.Enqueue(DecideInput());
            return input;
        }

        public BigInteger RunGame()
        {
            (bool running, IEnumerable<BigInteger> output) result;
            do
            {
                Queue<BigInteger> inputs = MakeInput();
                result = Computer.RunIntCodeV11(inputs);
                UpdateScreen(result.output.GetEnumerator());
                AnalyzeFrame();
                //ShowFrame();
            } while (result.running && RemainingBlocks() > 0);
            return Score;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string program = File.ReadLines("../../../input.txt").First();
            Dictionary<IntPoint2D, ArcadeMachine.Tile> screen = new ArcadeMachine(program).DrawFrame();

            Console.WriteLine($"The frame has {screen.Count(kv => kv.Value == ArcadeMachine.Tile.Block)} blocks at start");

            var score = new ArcadeMachine(program, true).RunGame();

            Console.WriteLine($"Winning score = {score}");
        }
    }
}
