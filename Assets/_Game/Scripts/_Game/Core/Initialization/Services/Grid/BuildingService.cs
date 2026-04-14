using System;
using System.Linq;
using Game.Assets;
using Game.Buildings.Core;
using Game.Buildings.Data;
using UnityEngine;

namespace Core.Initialization.Services.Grid
{
    public class BuildingService : IBuildingService
    {
        private readonly BuildingPlacementManager _placementManager;
        private readonly BuildingsListSO _buildingsList;

        public bool IsPlacing => _placementManager && _placementManager.IsPlacing;
        public event Action<BuildingData, Vector2Int> OnBuildingPlaced;
        
        private readonly IGridService _gridService = ServiceLocator.Instance.Get<IGridService>();

        public BuildingService(BuildingPlacementManager placementManager, BuildingsListSO buildingsList)
        {
            _placementManager = placementManager;
            _buildingsList = buildingsList;
        }
        
        public void StartPlacement(BuildingData buildingData)
        {
            var buildingPrefab = _buildingsList.buildings.First(x=> x.DisplayName == buildingData.DisplayName).Prefab.GetComponent<Building>();
            buildingPrefab.BindData(buildingData);
            if (buildingPrefab != null)
            {
                _placementManager.enabled = true;
                _placementManager.StartPlacement(buildingPrefab);
            }
        }

        public void CancelCurrentPlacement()
        {
            _placementManager?.CancelPlacement();
            if (_placementManager != null) 
                _placementManager.enabled = false;
        }

        public bool TryUpgrade(Building building)
        {
            return _placementManager.TryUpgrade(building);
        }

        public bool RemoveBuilding(Building building)
        {
            return _placementManager.RemoveFromGrid(building);
        }

        public BuildingData[] GetAvailableBuildings()
        {
            if (!_buildingsList || _buildingsList.buildings == null)
            {
                return Array.Empty<BuildingData>();
            }
            return _buildingsList.buildings;
        }

        public bool TryBuildBuilding(BuildingData data, Vector2Int cell)
        {
            if (!data.Prefab || _gridService == null)
            {
                return false;
            }

            Vector2Int footprint = data.GetSafeFootprint();
            if (!_gridService.CanPlaceAtCell(cell, footprint))
            {
                return false;
            }

            Building building = data.Prefab.GetComponent<Building>();
            building.BindData(data);
            if (_placementManager.TryPlace(building, cell))
            {
                OnBuildingPlaced?.Invoke(data, cell);
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            
        }
    }
}
