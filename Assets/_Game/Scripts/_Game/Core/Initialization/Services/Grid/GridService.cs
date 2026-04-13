using System;
using Game.Grid;
using UnityEngine;

namespace Core.Initialization.Services.Grid
{
    public class GridService : IGridService
    {
        private readonly GridManager _gridManager;

        public int Width => _gridManager.Width;
        public int Height => _gridManager.Height;
        public float CellSize => _gridManager.CellSize;
        public Vector3 Origin => _gridManager.Origin;
        public event Action<Vector2Int> OnCellClicked;

        public GridService(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            return _gridManager.WorldToCell(worldPosition);
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            return _gridManager.CellToWorld(cell);
        }

        public Vector3 GetCellCenterWorld(Vector2Int cell)
        {
            return _gridManager.GetCellCenterWorld(cell);
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return _gridManager.IsOccupied(cell);
        }

        public bool IsInBounds(Vector2Int cell)
        {
            return _gridManager.IsInBounds(cell);
        }

        public bool TryPlaceAtCell(Vector2Int cell, Vector2Int size)
        {
            return _gridManager.TryPlaceAtCell(cell, size);
        }

        public bool CanPlaceAtCell(Vector2Int cell, Vector2Int size)
        {
            return _gridManager.CanPlaceAtCell(cell, size);
        }

        public void RemoveAtCell(Vector2Int cell, Vector2Int size)
        {
            _gridManager.RemoveAtCell(cell, size);
        }

        public void NotifyCellClicked(Vector2Int cell)
        {
            OnCellClicked?.Invoke(cell);
        }
    }
}
