using UnityEngine;

namespace Game.Grid
{
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        public Vector3 Origin { get; }

        private readonly bool[,] _occupiedCells;

        public Grid(int width, int height, float cellSize, Vector3 origin)
        {
            Width = Mathf.Max(1, width);
            Height = Mathf.Max(1, height);
            CellSize = Mathf.Max(0.01f, cellSize);
            Origin = origin;

            _occupiedCells = new bool[Width, Height];
        }

        public bool IsInBounds(Vector2Int cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < Width && cell.y < Height;
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return Origin + new Vector3(cell.x * CellSize, 0f, cell.y * CellSize);
        }

        public Vector3 GetCellCenterWorld(Vector2Int cell)
        {
            return CellToWorld(cell) + new Vector3(CellSize * 0.5f, 0f, CellSize * 0.5f);
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            Vector3 localPosition = worldPosition - Origin;
            int x = Mathf.FloorToInt(localPosition.x / CellSize);
            int y = Mathf.FloorToInt(localPosition.z / CellSize);
            return new Vector2Int(x, y);
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return IsInBounds(cell) && _occupiedCells[cell.x, cell.y];
        }

        public bool TryOccupyCell(Vector2Int cell)
        {
            if (!IsInBounds(cell) || _occupiedCells[cell.x, cell.y])
            {
                return false;
            }

            _occupiedCells[cell.x, cell.y] = true;
            return true;
        }

        public void ReleaseCell(Vector2Int cell)
        {
            if (!IsInBounds(cell))
            {
                return;
            }

            _occupiedCells[cell.x, cell.y] = false;
        }

        public bool CanOccupyArea(Vector2Int originCell, Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0)
            {
                return false;
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int cell = new Vector2Int(originCell.x + x, originCell.y + y);
                    if (!IsInBounds(cell) || _occupiedCells[cell.x, cell.y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool TryOccupyArea(Vector2Int originCell, Vector2Int size)
        {
            if (!CanOccupyArea(originCell, size))
            {
                return false;
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    _occupiedCells[originCell.x + x, originCell.y + y] = true;
                }
            }

            return true;
        }

        public void ReleaseArea(Vector2Int originCell, Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0)
            {
                return;
            }

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int cell = new Vector2Int(originCell.x + x, originCell.y + y);
                    if (IsInBounds(cell))
                    {
                        _occupiedCells[cell.x, cell.y] = false;
                    }
                }
            }
        }

        public void ClearAllOccupancy()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _occupiedCells[x, y] = false;
                }
            }
        }

        public void CopyOccupancyTo(Grid targetGrid)
        {
            if (targetGrid == null)
            {
                return;
            }

            int copyWidth = Mathf.Min(Width, targetGrid.Width);
            int copyHeight = Mathf.Min(Height, targetGrid.Height);

            for (int x = 0; x < copyWidth; x++)
            {
                for (int y = 0; y < copyHeight; y++)
                {
                    if (_occupiedCells[x, y])
                    {
                        targetGrid.TryOccupyCell(new Vector2Int(x, y));
                    }
                }
            }
        }
    }
}
