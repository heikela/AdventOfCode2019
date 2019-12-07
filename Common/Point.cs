using System;

namespace Common
{
	struct IntPoint2D : IEquatable<IntPoint2D>
	{
		public int X { get; set; }
		public int Y { get; set; }
		public IntPoint2D(int x, int y)
		{
			X = x;
			Y = y;
		}
		public static IntPoint2D operator +(IntPoint2D a, IntPoint2D b)
		{
			return new IntPoint2D(a.X + b.X, a.Y + b.Y);
		}
		public static IntPoint2D operator -(IntPoint2D a, IntPoint2D b)
		{
			return new IntPoint2D(a.X - b.X, a.Y - b.Y);
		}

		public static IntPoint2D operator *(int m, IntPoint2D point)
		{
			return new IntPoint2D(point.X * m, point.Y * m);
		}

		public bool Equals(IntPoint2D p)
		{
			return X == p.X && Y == p.Y;
		}
		public static bool operator ==(IntPoint2D a, IntPoint2D b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(IntPoint2D a, IntPoint2D b)
		{
			return !(a.Equals(b));
		}

		public int ManhattanDist()
		{
			return Math.Abs(X) + Math.Abs(Y);
		}
		public int ManhattanDist(IntPoint2D other)
		{
			return (this - other).ManhattanDist();
		}
	}

	struct IntPoint3D : IEquatable<IntPoint3D>
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public IntPoint3D(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public static IntPoint3D operator +(IntPoint3D a, IntPoint3D b)
		{
			return new IntPoint3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		public static IntPoint3D operator -(IntPoint3D a, IntPoint3D b)
		{
			return new IntPoint3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static IntPoint3D operator *(int m, IntPoint3D point)
		{
			return new IntPoint3D(point.X * m, point.Y * m, point.Z * m);
		}

		public override bool Equals(object obj)
		{
			if (obj is IntPoint3D)
			{
				return this.Equals((IntPoint3D)obj);
			}
			return false;
		}
		public bool Equals(IntPoint3D p)
		{
			return X == p.X && Y == p.Y && Z == p.Z;
		}
		public static bool operator ==(IntPoint3D a, IntPoint3D b)
		{
			return a.Equals(b);
		}
		public static bool operator !=(IntPoint3D a, IntPoint3D b)
		{
			return !(a.Equals(b));
		}
		public int ManhattanDist()
		{
			long d = (long)Math.Abs(X) + (long)Math.Abs(Y) + (long)Math.Abs(Z);
			if (d > int.MaxValue)
			{
				return int.MaxValue;
			}
			return (int)d;
		}
		public int ManhattanDist(IntPoint3D other)
		{
			return (this - other).ManhattanDist();
		}
	}
}
