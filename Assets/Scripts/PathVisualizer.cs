using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interview.Pathfinding.Visualization
{
	public class PathVisualizer : MonoBehaviour
	{
		[SerializeField] private GameObject nodePrefab;
		private Dictionary<Coordinates, GameObject> nodesInstantiated = new Dictionary<Coordinates, GameObject>();
		public Action VisualizerReset;

		private void Start()
		{
			InitializeGrid();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{ 
				VisualizePath();
			}
		}

		private void InitializeGrid()
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Coordinates coords = new Coordinates(i, j);
					GameObject newNode = Instantiate(nodePrefab, PathfindingManager.Instance.GridToWorld(coords), Quaternion.identity);
					InteractableGridCell worldCell = newNode.GetComponent<InteractableGridCell>();
					worldCell.SetCell(coords);
					VisualizerReset += worldCell.ResetVisuals;
					
					nodesInstantiated.Add(coords, newNode);
					if (PathfindingManager.Instance.IsCellBlocked(coords))
					{
						newNode.GetComponent<MeshRenderer>().material.color = Color.black;
						continue;
					}
				}
			}
		}

		private void VisualizePath()
		{
			PathfindingManager.Instance.ToggleCellBlocked(new Coordinates(3, 9), false);
			PathfindingManager.Instance.ToggleCellBlocked(new Coordinates(6, 0), false);
			VisualizerReset?.Invoke();

			Queue<Coordinates> path = PathfindingManager.Instance.GetPath(new Coordinates(4, 9), new Coordinates(6, 0));
			
			
			while (path.Count > 0)
			{
				Coordinates coords = path.Dequeue();
				nodesInstantiated[coords].GetComponent<MeshRenderer>().material.color = Color.green;
			}
		}
	}

	
}