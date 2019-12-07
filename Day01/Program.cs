using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Day01
{
    class Program
    {
        static int FuelForMass(int mass)
        {
            return Math.Max(0, mass / 3 - 2);
        }

        static int FuelForPayload(int mass)
        {
            int fuelMass = FuelForMass(mass);
            if (fuelMass <= 0)
            {
                return 0;
            } else
            {
                return fuelMass + FuelForPayload(fuelMass);
            }
        }

        static void Main(string[] args)
        {
            IEnumerable<string> lines = File.ReadLines("../../../input.txt");

            IEnumerable<int> moduleMasses = lines.Select(int.Parse);

            Console.WriteLine($"Part1 = {moduleMasses.Select(FuelForMass).Sum()}");
            Console.WriteLine($"Part2 = {moduleMasses.Select(FuelForPayload).Sum()}");
        }
    }
}
