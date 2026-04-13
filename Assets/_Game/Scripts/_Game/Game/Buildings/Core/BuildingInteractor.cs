using Core.Initialization.Services.Grid;
using Game.Grid;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Buildings.Core
{
    [DisallowMultipleComponent]
    public class BuildingInteractor : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private BuildingPlacementManager _placementManager;
        [SerializeField] private LayerMask interactionMask = ~0;
        [SerializeField] private float maxDistance = 500f;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private GridManager gridManager;

        private void OnEnable()
        {
            interactAction?.action?.Enable();
        }

        private void OnDisable()
        {
            interactAction?.action?.Disable();
        }

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        public Building PlaceBuilding(GameObject buildingPrefab, Vector2Int cell)
        {
            if (buildingPrefab == null || gridManager == null)
            {
                return null;
            }

            Building building = Instantiate(buildingPrefab).GetComponent<Building>();
            if (building != null)
            {
                Vector3 position = gridManager.GetCellCenterWorld(cell);
                building.transform.position = position;

                Vector2Int footprint = building.Footprint;
                if (!gridManager.TryPlaceAtCell(cell, footprint))
                {
                    Destroy(building.gameObject);
                    return null;
                }
            }

            return building;
        }

        private void Update()
        {
            if (_placementManager && _placementManager.IsPlacing)
            {
                return;
            }

            if (!targetCamera || interactAction?.action == null || !interactAction.action.WasPressedThisFrame())
            {
                return;
            }

            Vector2 mousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            Ray ray = targetCamera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactionMask))
            {
                return;
            }

            Building building = hit.collider.GetComponentInParent<Building>();
            if (building)
            {
                building.Interact();
            }
        }
    }
}
