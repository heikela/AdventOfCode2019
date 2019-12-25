using System;
using Common;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Day25
{

    class Program
    {
        /* weather machine is lighter than klein bottle, spool of cat6,
antenna. Those four together are too heavy. WIthout weather machine too heavy, without
any one of the others too light

After that we found many dangerous items, and a mug. Turned out the five items mentioned
minus klein bottle provided the right weight */

        static List<string> Inputs = new List<string>() {
            "east",
            "take antenna",
            "west",
            "north",
            "take weather machine",
            "north",
            "take klein bottle",
            "east",
            "take spool of cat6",
            "east",
            "south",
            "take mug",
            "north",
            "west", // the checkpoint is south of here
            "south",
            "south",
            "drop klein bottle",
            "east"
        };
        static bool Exit = false;
        static int CommandIndex = 0;

        static Queue<BigInteger> GetAndRecordInput() {
            if (CommandIndex < Inputs.Count)
            {
                return MakeInput(Inputs[CommandIndex++]);
            }
            else
            {
                string input = Console.ReadLine();
                if (input == "quit")
                {
                    Exit = true;
                }
                Inputs.Add(input);
                CommandIndex++;
                return MakeInput(input);
            }
        }

        static Queue<BigInteger> MakeInput(string s)
        {
            Queue<BigInteger> input = new Queue<BigInteger>();
            foreach (char c in s)
            {
                input.Enqueue(c);
            }
            if (s.Length > 0)
            {
                input.Enqueue(10);
            }
            return input;
        }

        static void Main(string[] args)
        {
            IntCodeComputer droid = new IntCodeComputer(File.ReadLines("../../../input.txt").First());
            Queue<BigInteger> input = MakeInput("");
            while (!Exit)
            {
                var result = droid.RunIntCode(input);
                foreach (BigInteger c in result.output)
                {
                    if (c == 10)
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write((char)c);
                    }
                }
                input = GetAndRecordInput();
            }
            foreach (string s in Inputs)
            {
                Console.WriteLine(s);
            }
        }
    }
}
