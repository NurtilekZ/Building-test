using System;
using Core.Initialization.SaveLoad;
using Game.Buildings.Core;
using Game.Buildings.Data;
using UnityEngine;

namespace Core.Initialization.Services
{
    public interface IGridService
    {
        int Width { get; }
        int Height { get; }
        float CellSize { get; }
        Vector3 Origin { get; }
        Vector2Int WorldToCell(Vector3 worldPosition);
        Vector3 CellToWorld(Vector2Int cell);
        Vector3 GetCellCenterWorld(Vector2Int cell);
        bool IsOccupied(Vector2Int cell);
        bool IsInBounds(Vector2Int cell);
        bool TryPlaceAtCell(Vector2Int cell, Vector2Int size);
        bool CanPlaceAtCell(Vector2Int cell, Vector2Int size);
        void RemoveAtCell(Vector2Int cell, Vector2Int size);
        event Action<Vector2Int> OnCellClicked;
    }
    
    public interface ISaveLoadService
    {
        void SaveBuildings();
        void LoadBuildings();
        void DeleteSaveFile();
        bool TryLoadFromStartupMode(GameInitializer.StartupLoadMode loadMode);
        void SaveBuilding(Building building);
        void RemoveFromSaveList(Building building);
    }

    public interface IBuildingService
    {
        BuildingData[] GetAvailableBuildings();
        bool TryBuildBuilding(BuildingData data, Vector2Int cell);
        void CancelCurrentPlacement();
        bool IsPlacing { get; }
        event Action<BuildingData, Vector2Int> OnBuildingPlaced;
        bool TryUpgrade(Building building);
    }

    public interface IUIService
    {
        void ShowBuildingList(Vector2Int cell);
        void ShowMapWindow();
        void ShowBuildMode(Vector2Int cell);
        void ShowActionOverlay(Building building, Transform transform);
        void CloseWindow(WindowID id);
        void ShowBuildingInfo(Building building);
    }
}
