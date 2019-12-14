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
        static Dictionary<string, long> Surpluses = new Dictionary<string, long>();
        static long OreSpent;
        static Dictionary<string, Conversion> Conversions = new Dictionary<string, Conversion>();

        static void UseSurplusOrMake(string element, long quantity)
        {
            long availableSurplus = Surpluses.GetOrElse(element, 0);
            if (availableSurplus > 0)
            {
                long surplusUsed = Math.Min(quantity, availableSurplus);
                quantity -= surplusUsed;
                Surpluses[element] -= surplusUsed;
            }
            if (element == "ORE")
            {
                OreSpent += quantity;
                return;
            }
            if (quantity > 0)
            {
                Make(element, quantity);
            }
        }

        static void Make(string element, long quantity)
        {
            Conversion conversion = Conversions[element];

            long multiplier = (quantity - 1 + conversion.QuantityProduced) / conversion.QuantityProduced;
            long producedQuantity = multiplier * conversion.QuantityProduced;
            long surplusProduced = producedQuantity - quantity;
            foreach (Ingredient i in conversion.Ingredients)
            {
                UseSurplusOrMake(i.Element, i.Quantity * multiplier);
            }
            if (surplusProduced > 0)
            {
                if (Surpluses.ContainsKey(element))
                {
                    Surpluses[element] += surplusProduced;
                } else
                {
                    Surpluses.Add(element, surplusProduced);
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
                    string elementProduced = leftAndRight.Groups[3].Value;
                    return KeyValuePair.Create(
                        elementProduced,
                        new Conversion()
                        {
                            ElementProduced = elementProduced,
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
