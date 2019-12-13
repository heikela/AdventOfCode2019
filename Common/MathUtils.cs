using System;
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
	}
}
