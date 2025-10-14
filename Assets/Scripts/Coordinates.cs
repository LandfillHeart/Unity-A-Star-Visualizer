using System;

namespace Interview.Pathfinding
{
	public struct Coordinates
	{
		public int x;
		public int y;
		public Coordinates(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static Coordinates operator -(Coordinates minuend, Coordinates subtraend)
		{
			return new Coordinates(Math.Abs(minuend.x - subtraend.x), Math.Abs(minuend.x - subtraend.y));
		}

		public static bool operator ==(Coordinates caller, Coordinates other)
		{
			return caller.x == other.x && caller.y == other.y;
		}

		public static bool operator !=(Coordinates caller, Coordinates other)
		{
			return caller.x != other.x || caller.y != other.y;
		}

		public float Magnitude
		{
			get
			{
				return MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2));
			}
		}

		public override string ToString()
		{
			return $"X: {x}, Y: {y}";
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(x, y);
		}

		public override bool Equals(object obj)
		{
			if(!(obj is Coordinates)) return false;
			return this == (Coordinates)obj;
		}
	}
}