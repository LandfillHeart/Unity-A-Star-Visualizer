using UnityEngine;

namespace Interview.Pathfinding.Visualization
{
	public class InteractableGridCell : MonoBehaviour
	{
		private int x;
		private int y;
		private Renderer r;

		private void Awake()
		{
			r = GetComponent<Renderer>();
		}

		public void SetCell(Coordinates coords)
		{
			this.x = coords.x;
			this.y = coords.y;
		}

		public void ResetVisuals()
		{
			Coordinates coords = new Coordinates(x, y);
			this.r.material.color = PathfindingManager.Instance.IsCellBlocked(coords) ? Color.black : Color.gray;
		}

		private void OnMouseDown()
		{
			ToggleBlock();
		}

		private void ToggleBlock()
		{
			Coordinates coords = new Coordinates(x, y);
			PathfindingManager.Instance.ToggleCellBlocked(coords);
			if (PathfindingManager.Instance.IsCellBlocked(coords))
			{
				r.material.color = Color.black;
				return;
			}

			r.material.color = Color.gray;
		}
	}
}