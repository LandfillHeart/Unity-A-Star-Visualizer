using System;

namespace Interview.Pathfinding
{
	public class GridCell
	{
		public Coordinates coords { get; private set; }
		public bool IsBlocked { get; private set; }

		public GridCell(int x, int y)
		{
			coords = new Coordinates(x, y);
		}

		public void ToggleBlock(bool state)
		{
			IsBlocked = state;
		}

	}
}