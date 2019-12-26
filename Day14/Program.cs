using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Common;

namespace Day14
{
    class NanoFactory
    {
        struct Ingredient
        {
            public int Quantity;
            public string Chemical;
        }

        class Conversion
        {
            public int QuantityProduced;
            public string ChemicalProduced;
            public List<Ingredient> Ingredients;
        }

        private Dictionary<string, Conversion> Conversions = new Dictionary<string, Conversion>();

        private Dictionary<string, long> Surpluses = new Dictionary<string, long>();
        private long OreSpent;

        private void Reset()
        {
            Surpluses = new Dictionary<string, long>();
            OreSpent = 0;
        }

        public NanoFactory(IEnumerable<string> conversionFormulas)
        {
            Regex reactionPattern = new Regex(@"(.*?) => (\d+) (\w+)");
            Regex ingredientPattern = new Regex(@"(\d+) (\w+)");
            Conversions = conversionFormulas
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
            Reset();
        }

        public long MakeFromOre(string chemical, long quantity)
        {
            Reset();
            Make(chemical, quantity);
            return OreSpent;
        }

        public long HowManyCanBeMadeFrom(string chemical, long oreQuantity)
        {
            Reset();
            Make(chemical, 1);
            long maxOreNeeded = OreSpent;
            long produced = 1;
            while (OreSpent < oreQuantity)
            {
                long productionLowerBound = Math.Max(1, (oreQuantity - OreSpent) / maxOreNeeded);
                Make(chemical, productionLowerBound);
                produced += productionLowerBound;
            }
            return produced - 1;
        }

        private void UseSurplusOrMake(string chemical, long quantity)
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

        private void Make(string chemical, long quantity)
        {
            Conversion conversion = Conversions[chemical];

            long multiplier = (quantity - 1 + conversion.QuantityProduced) / conversion.QuantityProduced;
            long producedQuantity = multiplier * conversion.QuantityProduced;
            long surplusProduced = producedQuantity - quantity;
            foreach (Ingredient i in conversion.Ingredients)
            {
                UseSurplusOrMake(i.Chemical, i.Quantity * multiplier);
            }
            Surpluses.AddOrSet(chemical, Surpluses.GetOrElse(chemical, 0) + surplusProduced);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            NanoFactory factory = new NanoFactory(File.ReadLines("../../../input.txt"));
            Console.WriteLine($"1 FUEL can be produced from {factory.MakeFromOre("FUEL", 1)} ORE");
            long trillion = 1000000000000;
            Console.WriteLine($"{factory.HowManyCanBeMadeFrom("FUEL", trillion)} should be the answer to how much fuel can be made from {trillion} ore");
        }
    }
}
