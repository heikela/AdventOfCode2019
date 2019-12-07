using System;
using System.Linq;
using System.Collections.Generic;

namespace Day04
{
    class Program
    {
        static void Main(string[] args)
        {
            int good = 0;
            for (int i = 158126; i <= 624574; ++i)
            {
                List<int> digits = i.ToString().ToCharArray().Select(c => int.Parse(c.ToString())).ToList();
                bool adjacents = false;
                bool nondecreasing = true;
                for (int d = 1; d < digits.Count; ++d)
                {

                    if (digits[d] == digits[d - 1] && (d == digits.Count - 1 || digits[d + 1] != digits[d]) && (d == 1 || digits[d - 2] != digits[d]))
                    {
                        adjacents = true;
                    }
                    if (digits[d] < digits[d - 1])
                    {
                        nondecreasing = false;
                    }
                }
                if (adjacents && nondecreasing)
                {
                    ++good;
                }
            }
            Console.WriteLine($"{good}");
        }
    }
}
