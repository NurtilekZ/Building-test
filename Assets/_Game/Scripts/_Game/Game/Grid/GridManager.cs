using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Grid
{
    [DisallowMultipleComponent]
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int width = 20;
        [SerializeField] private int height = 20;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 origin;

        [Header("Runtime")]
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private bool rebuildOnValidateDuringPlay = true;
        [SerializeField] private bool centerOriginToTransform = true;
        [SerializeField] private bool enableCellClickEvents = true;

        [Header("Input")]
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionReference clickAction;

        public Grid RuntimeGrid { get; private set; }
        public UnityEvent GridRebuilt;
        public event Action<Vector2Int> OnCellClicked;

        public int Width => Mathf.Max(1, width);
        public int Height => Mathf.Max(1, height);
        public float CellSize => Mathf.Max(0.01f, cellSize);
        public Vector3 Origin => centerOriginToTransform ? origin + new Vector3((-width * 0.5f) * cellSize, 0f, (-height * 0.5f) * cellSize) : origin + transform.position;

        private bool _hasClickedThisFrame;

        private void Awake()
        {
            if (initializeOnAwake)
            {
                InitializeGrid();
            }
        }

        private void OnEnable()
        {
            if (clickAction != null)
            {
                clickAction.action.Enable();
                clickAction.action.performed += HandleClickAction;
            }
        }

        private void OnDisable()
        {
            if (clickAction?.action != null)
            {
                clickAction.action.performed -= HandleClickAction;
                clickAction.action.Disable();
            }
        }

        private void HandleClickAction(InputAction.CallbackContext context)
        {
            if (!enableCellClickEvents || _hasClickedThisFrame)
            {
                return;
            }

            _hasClickedThisFrame = true;
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Vector2Int cell = WorldToCell(hit.point);
                if (IsInBounds(cell))
                {
                    Debug.Log("Click 3333");
                    OnCellClicked?.Invoke(cell);
                }
            }
        }

        private void LateUpdate()
        {
            _hasClickedThisFrame = false;
        }

        public void InitializeGrid()
        {
            RuntimeGrid = new Grid(Width, Height, CellSize, Origin);
            GridRebuilt?.Invoke();
        }

        public void RebuildGrid(bool preserveOccupancy = true)
        {
            Grid previousGrid = RuntimeGrid;
            Grid newGrid = new Grid(Width, Height, CellSize, Origin);

            if (preserveOccupancy && previousGrid != null)
            {
                previousGrid.CopyOccupancyTo(newGrid);
            }

            RuntimeGrid = newGrid;
            GridRebuilt?.Invoke();
        }

        public void SetGridSettings(int newWidth, int newHeight, float newCellSize, Vector3 newOrigin, bool rebuildNow = true, bool preserveOccupancy = true)
        {
            width = Mathf.Max(1, newWidth);
            height = Mathf.Max(1, newHeight);
            cellSize = Mathf.Max(0.01f, newCellSize);

            // Stored as local offset so world origin follows manager transform.
            origin = newOrigin + transform.position;

            if (rebuildNow)
            {
                RebuildGrid(preserveOccupancy);
            }
        }

        public void SetDimensions(int newWidth, int newHeight, bool rebuildNow = true, bool preserveOccupancy = true)
        {
            SetGridSettings(newWidth, newHeight, cellSize, Origin, rebuildNow, preserveOccupancy);
        }

        public void SetCellSize(float newCellSize, bool rebuildNow = true, bool preserveOccupancy = true)
        {
            SetGridSettings(width, height, newCellSize, Origin, rebuildNow, preserveOccupancy);
        }

        public void SetOrigin(Vector3 newOrigin, bool rebuildNow = true, bool preserveOccupancy = true)
        {
            SetGridSettings(width, height, cellSize, newOrigin, rebuildNow, preserveOccupancy);
        }

        public Vector2Int WorldToCell(Vector3 worldPosition)
        {
            EnsureGrid();
            return RuntimeGrid.WorldToCell(worldPosition);
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            EnsureGrid();
            return RuntimeGrid.CellToWorld(cell);
        }

        public Vector3 GetCellCenterWorld(Vector2Int cell)
        {
            EnsureGrid();
            return RuntimeGrid.GetCellCenterWorld(cell);
        }

        public bool TryPlaceAtCell(Vector2Int cell, Vector2Int size)
        {
            EnsureGrid();
            return RuntimeGrid.TryOccupyArea(cell, size);
        }

        public bool CanPlaceAtCell(Vector2Int cell, Vector2Int size)
        {
            EnsureGrid();
            return RuntimeGrid.CanOccupyArea(cell, size);
        }

        public void RemoveAtCell(Vector2Int cell, Vector2Int size)
        {
            EnsureGrid();
            RuntimeGrid.ReleaseArea(cell, size);
        }

        public bool IsOccupied(Vector2Int cell)
        {
            EnsureGrid();
            return RuntimeGrid.IsOccupied(cell);
        }

        public bool IsInBounds(Vector2Int cell)
        {
            EnsureGrid();
            return RuntimeGrid.IsInBounds(cell);
        }

        private void EnsureGrid()
        {
            if (RuntimeGrid == null)
            {
                InitializeGrid();
            }
        }

        private void OnValidate()
        {
            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
            cellSize = Mathf.Max(0.01f, cellSize);

            if (!rebuildOnValidateDuringPlay || RuntimeGrid == null)
            {
                return;
            }

            RebuildGrid(true);
        }
    }
}
