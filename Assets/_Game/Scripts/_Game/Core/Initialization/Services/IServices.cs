using System;
using Core.Initialization.SaveLoad;
using Game.Buildings.Core;
using Game.Buildings.Data;
using UnityEngine;

namespace Core.Initialization.Services
{
    public interface IGridService : IService
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
        void EnableGridClick(bool enable);
    }

    public interface IService : IDisposable
    {
    }

    public interface ISaveLoadService : IService
    {
        void SaveBuildings();
        void LoadBuildings();
        void DeleteSaveFile();
        bool TryLoadFromStartupMode(GameInitializer.StartupLoadMode loadMode);
        void SaveBuilding(Building building);
        void RemoveFromSaveList(Building building);
        void ClearPlacedBuildingsInScene();
    }

    public interface IBuildingService : IService
    {
        BuildingData[] GetAvailableBuildings();
        bool IsPlacing { get; }
        event Action<BuildingData, Vector2Int> OnBuildingPlaced;
        void StartPlacement(BuildingData building);
        void CancelCurrentPlacement();
        bool TryBuildBuilding(BuildingData data, Vector2Int cell);
        bool TryUpgrade(Building building);
        bool RemoveBuilding(Building building);
    }

    public interface IUIService : IService
    {
        void ShowBuildingList(Vector2Int cell);
        void ShowMapWindow();
        void ShowBuildMode();
        void CloseWindow(WindowID id);
        void ShowBuildingInfo(Building building);
        void ShowHUD();
        void ShowActionsWindow(Building building);
    }
}
