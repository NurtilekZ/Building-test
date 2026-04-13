using System;
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
        private readonly BuildingInteractor _buildingInteractor;

        public bool IsPlacing => _placementManager && _placementManager.IsPlacing;
        public event Action<BuildingData, Vector2Int> OnBuildingPlaced;
        
        private readonly IGridService _gridService = ServiceLocator.Instance.Get<IGridService>();

        public BuildingService(
            BuildingPlacementManager placementManager,
            BuildingsListSO buildingsList,
            BuildingInteractor buildingInteractor)
        {
            _placementManager = placementManager;
            _buildingsList = buildingsList;
            _buildingInteractor = buildingInteractor;
        }

        public bool TryUpgrade(Building building)
        {
            return _placementManager.TryUpgrade(building);
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

            Building building = _buildingInteractor.PlaceBuilding(data.Prefab, cell);
            if (building != null)
            {
                OnBuildingPlaced?.Invoke(data, cell);
                return true;
            }

            return false;
        }

        public void CancelCurrentPlacement()
        {
            _placementManager?.CancelPlacement();
        }
    }
}
