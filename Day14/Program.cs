using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Common;

namespace Day14
{
    struct Ingredient {
        public int Quantity;
        public string Element;
    }

    class Conversion
    {
        public int QuantityProduced;
        public string ElementProduced;
        public List<Ingredient> Ingredients;
    }

    class Program
    {
        static Dictionary<string, int> Surpluses = new Dictionary<string, int>();
        static long OreSpent;
        static Dictionary<string, List<Conversion>> Conversions = new Dictionary<string, List<Conversion>>();

        static void UseSurplusOrMake(Ingredient requirement)
        {
            int availableSurplus = Surpluses.GetOrElse(requirement.Element, 0);
            if (availableSurplus > 0)
            {
                int surplusUsed = Math.Min(requirement.Quantity, availableSurplus);
                requirement.Quantity -= surplusUsed;
                Surpluses[requirement.Element] -= surplusUsed;
            }
            if (requirement.Element == "ORE")
            {
                OreSpent += requirement.Quantity;
                return;
            }
            if (requirement.Quantity > 0)
            {
                Make(requirement);
            }
        }

        static void Make(Ingredient requirement)
        {
            // There's only one in the data for each element!
            Conversion conversion = Conversions[requirement.Element].First();

            while (requirement.Quantity > 0)
            {
                foreach (Ingredient i in conversion.Ingredients)
                {
                    // We never produce really large numbers of anything at once
                    {
                        UseSurplusOrMake(i);
                    }
                }
                requirement.Quantity -= conversion.QuantityProduced;
                if (requirement.Quantity < 0)
                {
                    if (Surpluses.ContainsKey(requirement.Element))
                    {
                        Surpluses[requirement.Element] -= requirement.Quantity;
                    } else
                    {
                        Surpluses.Add(requirement.Element, -requirement.Quantity);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Regex reactionPattern = new Regex(@"(.*?) => (\d+) (\w+)");
            Regex ingredientPattern = new Regex(@"(\d+) (\w+)");
            Conversions = File
                .ReadLines("../../../input.txt")
                .Select(line =>
                {
                    Match leftAndRight = reactionPattern.Match(line);
                    if (!leftAndRight.Success)
                    {
                        throw new Exception($"Unable to parse {line}");
                    }
                    List<Ingredient> ingredients = ingredientPattern
                        .Matches(leftAndRight.Groups[1].Value)
                        .Select(ingredientMatch => new Ingredient()
                        {
                            Quantity = int.Parse(ingredientMatch.Groups[1].Value),
                            Element = ingredientMatch.Groups[2].Value
                        })
                        .ToList();
                    return new Conversion()
                        {
                            ElementProduced = leftAndRight.Groups[3].Value,
                            QuantityProduced = int.Parse(leftAndRight.Groups[2].Value),
                            Ingredients = ingredients,
                        };

                })
                .GroupBy(conversion => conversion.ElementProduced)
                .ToDictionary();
            Ingredient requirement = new Ingredient()
            {
                Element = "FUEL",
                Quantity = 1
            };

            foreach (var c in Conversions)
            {
                Console.WriteLine($"Thre are {c.Value.Count} ways to make {c.Key}");
            }
            Make(requirement);
            Console.WriteLine($"1 FUEL can be produced from {OreSpent} ORE");
            int fuelProduced = 1;
            while (OreSpent < 1000000000000)
            {
                Make(requirement);
                fuelProduced += 1;
                if (Surpluses.All(kv => kv.Value == 0))
                {
                    Console.WriteLine($"{fuelProduced} FUEL can be produced from {OreSpent} ORE with no surpluses!");
                }
            }
            Console.WriteLine($"{fuelProduced} FUEL can be produced from {OreSpent} ORE");
        }
    }
}
