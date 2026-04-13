using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Buildings.Core
{
    [DisallowMultipleComponent]
    public class BuildingPlacementManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private LayerMask placementSurfaceMask = ~0;

        [Header("Placement")]
        [SerializeField] private Building defaultBuildingPrefab;
        [SerializeField] private float rayDistance = 1000f;
        [SerializeField] private bool startPlacementOnAwake;
        [SerializeField] private InputActionReference placeAction;
        [SerializeField] private InputActionReference cancelAction;
        [SerializeField] private InputActionReference rotateAction;
        [SerializeField] private Color validPreviewColor = new Color(0f, 1f, 0f, 0.55f);
        [SerializeField] private Color invalidPreviewColor = new Color(1f, 0f, 0f, 0.55f);

        private readonly ISaveLoadService _saveLoadService = ServiceLocator.Instance.Get<ISaveLoadService>();
        private readonly IGridService _gridService = ServiceLocator.Instance.Get<IGridService>();
        
        public bool IsPlacing => _previewBuilding != null;

        private Building _selectedPrefab;
        private Building _previewBuilding;
        private Vector2Int _currentCell;
        private bool _canPlaceCurrentCell;
        private int _rotationSteps;
        private Renderer[] _previewRenderers;

        private void OnEnable()
        {
            placeAction?.action?.Enable();
            cancelAction?.action?.Enable();
            rotateAction?.action?.Enable();
        }

        private void OnDisable()
        {
            placeAction?.action?.Disable();
            cancelAction?.action?.Disable();
            rotateAction?.action?.Disable();
        }

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (startPlacementOnAwake && defaultBuildingPrefab != null)
            {
                StartPlacement(defaultBuildingPrefab);
            }
        }

        private void Update()
        {
            if (!_previewBuilding)
            {
                return;
            }

            UpdatePreviewPositionAndValidity();
            HandlePlacementInput();
        }
        
        private void HandleGridCellClicked(Vector2Int cell)
        {
            IUIService uiService = ServiceLocator.Instance.Get<IUIService>();
            IGridService gridService = ServiceLocator.Instance.Get<IGridService>();

            if (!gridService.IsOccupied(cell))
            {
                uiService.ShowBuildingList(cell);
            }
        }

        public void StartPlacement(Building buildingPrefab)
        {
            CancelPlacement();

            if (buildingPrefab == null || _gridService == null)
            {
                return;
            }

            _selectedPrefab = buildingPrefab;
            _previewBuilding = Instantiate(_selectedPrefab);
            _previewBuilding.gameObject.name = _selectedPrefab.gameObject.name + "_Preview";
            _previewBuilding.gameObject.SetActive(true);
            _previewBuilding.SetPlacementRotation(_rotationSteps);
            _previewRenderers = _previewBuilding.GetComponentsInChildren<Renderer>();

            ApplyPreviewColor(invalidPreviewColor);
        }

        public void CancelPlacement()
        {
            if (_previewBuilding)
            {
                Destroy(_previewBuilding.gameObject);
            }

            _previewBuilding = null;
            _selectedPrefab = null;
            _previewRenderers = null;
            _rotationSteps = 0;
        }

        private void HandleRotationInput()
        {
            if (rotateAction?.action != null && rotateAction.action.WasPressedThisFrame())
            {
                _rotationSteps = (_rotationSteps + 1) % 4;
                _previewBuilding.SetPlacementRotation(_rotationSteps);
            }
        }

        private void UpdatePreviewPositionAndValidity()
        {
            if (!gameObject)
            {
                return;
            }

            if (!TryGetMouseWorldPosition(out Vector3 worldPosition))
            {
                _canPlaceCurrentCell = false;
                ApplyPreviewColor(invalidPreviewColor);
                return;
            }

            _currentCell = _gridService.WorldToCell(worldPosition);
            _canPlaceCurrentCell = _gridService.CanPlaceAtCell(_currentCell, _previewBuilding.Footprint);

            Vector3 snappedCenter = GetBuildingCenterForCell(_currentCell, _previewBuilding.Footprint);
            _previewBuilding.transform.position = snappedCenter;
            ApplyPreviewColor(_canPlaceCurrentCell ? validPreviewColor : invalidPreviewColor);
        }

        private void HandlePlacementInput()
        {
            if (cancelAction?.action != null && cancelAction.action.WasPressedThisFrame())
            {
                CancelPlacement();
                return;
            }

            if (placeAction?.action == null || !placeAction.action.WasPressedThisFrame() || !_canPlaceCurrentCell)
            {
                return;
            }

            Building placedBuilding = Instantiate(_selectedPrefab);
            placedBuilding.SetPlacementRotation(_rotationSteps);
            if (TryPlace(placedBuilding, _currentCell))
            {
                return;
            }

            Destroy(placedBuilding.gameObject);
        }

        public bool TryPlace(Building building, Vector2Int originCell, BuildingSaveData record = null)
        {
            if (!_gridService.TryPlaceAtCell(originCell, building.Footprint))
            {
                return false;
            }

            SnapToGridCenter(building, originCell);
            building.Place(originCell, building.Data, record);
            _saveLoadService.SaveBuilding(building);
            return true;
        }
        
        private void SnapToGridCenter(Building building, Vector2Int originCell)
        {
            Vector3 baseCenter = _gridService.GetCellCenterWorld(originCell);
            Vector3 footprintOffset = new Vector3(
                (building.Footprint.x - 1) * _gridService.CellSize * 0.5f,
                0f,
                (building.Footprint.y - 1) * _gridService.CellSize * 0.5f);

            building.transform.position = baseCenter + footprintOffset;
        }

        public void RemoveFromGrid(Building building)
        {
            if (!building.IsPlaced)
            {
                return;
            }

            _gridService.RemoveAtCell(building.OriginCell, building.Footprint);
            building.Remove();
        }

        public bool TryUpgrade(Building building)
        {
            if (!building.CanUpgrade())
            {
                return false;
            }

            if (building.Upgrade())
            {
                _saveLoadService.SaveBuildings();
            }
            return true;
        }

        private bool TryGetMouseWorldPosition(out Vector3 worldPosition)
        {
            worldPosition = default;
            if (!targetCamera)
            {
                return false;
            }

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }
            
            Vector2 mousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            Ray ray = targetCamera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, placementSurfaceMask))
            {
                return false;
            }

            worldPosition = hit.point;
            return true;
        }

        private Vector3 GetBuildingCenterForCell(Vector2Int originCell, Vector2Int footprint)
        {
            Vector3 baseCenter = _gridService.GetCellCenterWorld(originCell);
            Vector3 footprintOffset = new Vector3(
                (footprint.x - 1) * _gridService.CellSize * 0.5f,
                0f,
                (footprint.y - 1) * _gridService.CellSize * 0.5f);

            return baseCenter + footprintOffset;
        }

        private void ApplyPreviewColor(Color color)
        {
            if (_previewRenderers == null)
            {
                return;
            }

            for (int i = 0; i < _previewRenderers.Length; i++)
            {
                Renderer rendererRef = _previewRenderers[i];
                if (!rendererRef || !rendererRef.material)
                {
                    continue;
                }

                rendererRef.material.color = color;
            }
        }
    }
}
