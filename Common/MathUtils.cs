using System;
using System.Numerics;

namespace Common
{
    public class MathUtils
    {
		public static long GCD(long a, long b)
		{
			if (a == 0)
			{
				return b;
			}
			if (b == 0)
			{
				return a;
			}
			if (a > b)
			{
				return GCD(b, a % b);
			}
			else
			{
				return GCD(a, b % a);
			}
		}

        public static long LCD(long a, long b)
        {
            if (a == 0 && b == 0)
            {
                return 0;
            }
            else
            {
                return (a / GCD(a, b)) * b;
            }
        }

        private static (BigInteger r, BigInteger s, BigInteger t) ExtendedEuler(BigInteger a, BigInteger b)
        {
            BigInteger r0 = BigInteger.Max(a, b);
            BigInteger r1 = BigInteger.Min(a, b);
            BigInteger s0 = 1;
            BigInteger t0 = 0;
            BigInteger s1 = 0;
            BigInteger t1 = 1;
            while (r1 != 0)
            {
                BigInteger q = r0 / r1;
                BigInteger mod = r0 % r1;
                r0 = r1;
                r1 = mod;
                BigInteger newS = s0 - s1 * q;
                s0 = s1;
                s1 = newS;
                BigInteger newT = t0 - t1 * q;
                t0 = t1;
                t1 = newT;
            }
            return (r0, s0, t0);
        }

        public static BigInteger MultiplicativeInverse(BigInteger a, BigInteger mod)
        {
            (BigInteger r, BigInteger s, BigInteger t) = ExtendedEuler(a, mod);
            if (r != 1)
            {
                throw new Exception($"{a} and {mod} are not coprime, there is no multiplicative inverse for {a} modulo {mod}.");
            }
            else
            {
                return (t + mod) % mod;
            }
        }
	}
}
