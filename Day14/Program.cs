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
        public string Chemical;
    }

    class Conversion
    {
        public int QuantityProduced;
        public string ChemicalProduced;
        public List<Ingredient> Ingredients;
    }

    class Program
    {
        static Dictionary<string, long> Surpluses = new Dictionary<string, long>();
        static long OreSpent;
        static Dictionary<string, Conversion> Conversions = new Dictionary<string, Conversion>();

        static void UseSurplusOrMake(string chemical, long quantity)
        {
            long availableSurplus = Surpluses.GetOrElse(chemical, 0);
            if (availableSurplus > 0)
            {
                long surplusUsed = Math.Min(quantity, availableSurplus);
                quantity -= surplusUsed;
                Surpluses[chemical] -= surplusUsed;
            }
            if (chemical == "ORE")
            {
                OreSpent += quantity;
                return;
            }
            if (quantity > 0)
            {
                Make(chemical, quantity);
            }
        }

        static void Make(string chemical, long quantity)
        {
            Conversion conversion = Conversions[chemical];

            long multiplier = (quantity - 1 + conversion.QuantityProduced) / conversion.QuantityProduced;
            long producedQuantity = multiplier * conversion.QuantityProduced;
            long surplusProduced = producedQuantity - quantity;
            foreach (Ingredient i in conversion.Ingredients)
            {
                UseSurplusOrMake(i.Chemical, i.Quantity * multiplier);
            }
            if (surplusProduced > 0)
            {
                if (Surpluses.ContainsKey(chemical))
                {
                    Surpluses[chemical] += surplusProduced;
                } else
                {
                    Surpluses.Add(chemical, surplusProduced);
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
                            Chemical = ingredientMatch.Groups[2].Value
                        })
                        .ToList();
                    string chemicalProduced = leftAndRight.Groups[3].Value;
                    return KeyValuePair.Create(
                        chemicalProduced,
                        new Conversion()
                        {
                            ChemicalProduced = chemicalProduced,
                            QuantityProduced = int.Parse(leftAndRight.Groups[2].Value),
                            Ingredients = ingredients,
                        });

                })
                .ToDictionary();
            Make("FUEL", 1);
            Console.WriteLine($"1 FUEL can be produced from {OreSpent} ORE");
            long fuelProduced = 1;
            long maxOreNeeded = OreSpent;
            long trillion = 1000000000000;
            while (OreSpent < trillion)
            {
                long fuelLowerBound = Math.Max(1, (trillion - OreSpent) / maxOreNeeded);
                Make("FUEL", fuelLowerBound);
                fuelProduced += fuelLowerBound;
            }
            Console.WriteLine($"{fuelProduced} FUEL can be produced from {OreSpent} ORE");
            Console.WriteLine($"Therefore {fuelProduced - 1} should be the answer");
        }
    }
}
