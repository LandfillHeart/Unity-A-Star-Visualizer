using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Interview.Pathfinding
{
	public class PathfindingManager : MonoBehaviour
	{
		private static PathfindingManager instance;
		public static PathfindingManager Instance => instance;

		private float cellSize = 5f;
		private int mapSizeX = 10;
		private int mapSizeY = 10;

		private Vector3 startingPos = Vector3.zero;

		[NonSerialized] private GridCell[,] grid;

		// Unity Methods
		private void Awake()
		{
			instance = this;
			InitializeGrid();
			
		}

		private void Start()
		{
			/*
			Queue<Coordinates> path = GetPath(new Coordinates(1, 0), new Coordinates(2, 3));
			if (path.Count == 0) Debug.Log("We fucked up");
			while(path.Count > 0)
			{
				Coordinates coords = path.Dequeue();
				Debug.Log(coords.ToString());
			}
			*/
		}

		// Pathfinding
		// TO-DO: this should return bool and out Path
		public Queue<Coordinates> GetPath(Coordinates start, Coordinates goal)
		{
			
			// priority queue isn't supported by default so going with an hashset for now
			HashSet<Coordinates> openSet = new HashSet<Coordinates>();
			openSet.Add(start);

			Dictionary<Coordinates, Coordinates> cameFrom = new Dictionary<Coordinates, Coordinates>();

			// gScore[n] is the currently known cost of the cheapest path from start to n.
			float[,] gScore = new float[mapSizeX, mapSizeY];
			MapWithDefaultValueInfinity(gScore);
			gScore[start.x, start.y] = 0f;

			// fScore[n] = gScore[n] + h(n).
			// fScore[n] represents our current best guess as to how cheap a path could be from start to finish if it goes through n.
			float[,] fScore = new float[mapSizeX, mapSizeY];
			MapWithDefaultValueInfinity(fScore);
			fScore[start.x, start.y] = Heuristic(start);

			while (openSet.Count > 0)
			{

				// get lowest score
				float lowestH = Mathf.Infinity;
				Coordinates current = start; // default assignment
				foreach (Coordinates node in openSet)
				{
					float h = Heuristic(node);
					if (Heuristic(node) < lowestH)
					{
						current = node;
						lowestH = h;
					}
				}

				if (current == goal)
				{
					List<Coordinates> totalPath = new List<Coordinates>();
					totalPath.Add(current);
					while(cameFrom.ContainsKey(current))
					{
						current = cameFrom[current];
						totalPath.Add(current);
					}
					totalPath.Reverse();
					return new Queue<Coordinates>(totalPath);
				}

				openSet.Remove(current);
				// we're checking all 8 neighbors, considering a square grid
				// i and j are x and y offsets to current's coordinates
				for(int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						// dont check current, only looking at neighbors
						if (i == 0 && j == 0) continue;

						// going out-of-bounds of the grid
						if (current.x + i < 0 || current.y + j < 0 || current.x + i >= mapSizeX || current.y + j >= mapSizeY) continue; 

						// in out grid, we've placed a wall/obstacle on this tile, so it has to be skipped for another neighbor
						if (grid[current.x + i, current.y + j].IsBlocked) continue;

						// tentative gScore is the distance from start to the neighbor through current
						float tentative_gScore = gScore[current.x, current.y];
						// edge(n) - makes sure pathfinder doesnt return a diagonal when a straight line would be faster
						// if i or j is 0 then the neighbor is not diagonal, and cellSize is the distance from center of cell to center of cell
						// otherwise, the neighbor is diagonal, so sqrt of side^2 + side^2
						if(Mathf.Abs(i + j) != 1)
						{
							tentative_gScore += Mathf.Sqrt(Mathf.Pow(cellSize, 2) * 2);
						} else
						{
							tentative_gScore += cellSize;
						}

						if (tentative_gScore < gScore[current.x + i, current.y + j])
						{
							// this path to neighbor is better than any previous one
							cameFrom[grid[current.x + i, current.y + j].coords] = current;
							gScore[current.x + i, current.y + j] = tentative_gScore;
							fScore[current.x + i, current.y + j] = tentative_gScore + Heuristic(grid[current.x + i, current.y + j].coords);
							if (!openSet.Contains(grid[current.x + i, current.y + j].coords))
							{
								openSet.Add(grid[current.x + i, current.y + j].coords);
							}
						}
					}
				}
			}

			Debug.Log("Momentary empty return on goal not found, to be replaced with bool return and out path");
			return new Queue<Coordinates>();

			// estimates the cost to reach goal from node n
			float Heuristic(Coordinates n)
			{
				return (goal - n).Magnitude;
			}


		}

		// Utils
		private void InitializeGrid()
		{
			// 0, 0 = bottom left corner
			// N, N = top right
			grid = new GridCell[mapSizeX, mapSizeY];

			for(int i = 0;  i < mapSizeX; i++)
			{
				for(int j = 0; j < mapSizeY; j++)
				{
					grid[i, j] = new GridCell(i, j);
				}
			}
		}

		private void InitializeDefaultGrid()
		{
			// 0, 0 = bottom left corner
			// N, N = top right
			grid = new GridCell[mapSizeX, mapSizeY];

			for (int i = 0; i < mapSizeX; i++)
			{
				for (int j = 0; j < mapSizeY; j++)
				{
					grid[i, j] = new GridCell(i, j);
				}
			}
		}

		private void MapWithDefaultValueInfinity(float[,] map)
		{
			for (int i = 0; i < mapSizeX; i++)
			{
				for(int j = 0; j < mapSizeY; j++)
				{
					map[i, j] = Mathf.Infinity;
				}
			}
		}

		public void ToggleCellBlocked(Coordinates coords)
		{

			grid[coords.x, coords.y].ToggleBlock(!grid[coords.x, coords.y].IsBlocked);
		}

		public void ToggleCellBlocked(Coordinates coords, bool state)
		{
			grid[coords.x, coords.y].ToggleBlock(state);
		}

		public bool IsCellBlocked(Coordinates coords)
		{
			return grid[coords.x, coords.y].IsBlocked;
		}

		// take a virtual point (shifted world point) and transform it into a virtual grid coordinate
		private Coordinates PointToGrid(Vector2 worldPoint)
		{
			// if your cell is size 1 and your coord.x is 0.9f, then your are still closer to cell x = 0 than x = 1, you're within its bounds
			int x = Mathf.FloorToInt(worldPoint.x / cellSize);
			int y = Mathf.FloorToInt(worldPoint.y / cellSize);

			return new Coordinates(x, y);
		}

		public Vector2 GridToWorld(Coordinates coords)
		{
			// subtract cellSize/2 to receive the center of the cell and not the corner
			float x = coords.x * cellSize - cellSize/2;
			float y = coords.y * cellSize - cellSize/2;
			return new Vector2(x, y);
		}

		// take a world point and shift it so that (0, 0) is equal to the bottom-left corner of Grid[0, 0]
		/*
		private Vector2 WorldPointToVirtualCoord(Vector3 point)
		{
			
		}
		*/
	}
}

