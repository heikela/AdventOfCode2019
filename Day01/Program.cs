using System;
using System.IO;
using System.Collections.Generic;

namespace Day01
{
    class Program
    {
        static int FuelForMass(int mass)
        {
            return Math.Max(0, mass / 3 - 2);
        }

        static void Main(string[] args)
        {
            IEnumerable<string> lines = File.ReadLines("../../../input.txt");

            int fuel1 = 0;
            int fuel2 = 0;
            foreach (string line in lines)
            {
                int mass = int.Parse(line);
                fuel1 += FuelForMass(mass);
                while (FuelForMass(mass) > 0)
                {
                    mass = FuelForMass(mass);
                    fuel2 += mass;
                }
            }

            Console.WriteLine($"Part1 = {fuel1}");
            Console.WriteLine($"Part2 = {fuel2}");
        }
    }
}
