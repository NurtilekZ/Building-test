using UnityEngine;

namespace Game.Grid
{
    [DisallowMultipleComponent]
    public class GridGizmos : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private bool drawGrid = true;
        [SerializeField] private bool drawOccupiedCells = true;
        [SerializeField] private Color gridLineColor = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField] private Color occupiedCellColor = new Color(1f, 0f, 0f, 0.25f);

        private void Reset()
        {
            gridManager = GetComponent<GridManager>();
        }

        private void OnDrawGizmos()
        {
            if (gridManager == null)
            {
                return;
            }

            if (gridManager.RuntimeGrid == null)
            {
                gridManager.InitializeGrid();
            }

            DrawGridLines();

            if (drawOccupiedCells)
            {
                DrawOccupiedCells();
            }
        }

        private void DrawGridLines()
        {
            if (!drawGrid)
            {
                return;
            }

            Grid grid = gridManager.RuntimeGrid;
            Vector3 origin = grid.Origin;
            float widthInUnits = grid.Width * grid.CellSize;
            float heightInUnits = grid.Height * grid.CellSize;

            Gizmos.color = gridLineColor;

            for (int x = 0; x <= grid.Width; x++)
            {
                Vector3 start = origin + new Vector3(x * grid.CellSize, 0f, 0f);
                Vector3 end = start + new Vector3(0f, 0f, heightInUnits);
                Gizmos.DrawLine(start, end);
            }

            for (int y = 0; y <= grid.Height; y++)
            {
                Vector3 start = origin + new Vector3(0f, 0f, y * grid.CellSize);
                Vector3 end = start + new Vector3(widthInUnits, 0f, 0f);
                Gizmos.DrawLine(start, end);
            }
        }

        private void DrawOccupiedCells()
        {
            Grid grid = gridManager.RuntimeGrid;
            Gizmos.color = occupiedCellColor;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    if (!grid.IsOccupied(cell))
                    {
                        continue;
                    }

                    Vector3 center = grid.GetCellCenterWorld(cell);
                    Vector3 size = new Vector3(grid.CellSize, 0.02f, grid.CellSize);
                    Gizmos.DrawCube(center, size);
                }
            }
        }
    }
}
