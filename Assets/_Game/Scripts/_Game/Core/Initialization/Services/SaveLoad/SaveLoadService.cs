using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Initialization.SaveLoad;
using Game.Assets;
using Game.Buildings.Core;
using Game.Grid;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Initialization.Services.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private GridManager _gridManager;
        private BuildingPlacementManager _buildingPlacementManager;
        private readonly BuildingsListSO _buildingsList;

        private string _saveFileName = "buildings-save.json";
        private bool _clearSavesOnLoad;

        private List<Building> _allBuildings = new List<Building>();

        public SaveLoadService(GridManager gridManager, BuildingPlacementManager placementManager,
            BuildingsListSO buildingsList, bool clearSavesOnLoad)
        {
            _gridManager = gridManager;
            _buildingPlacementManager = placementManager;
            _buildingsList = buildingsList;
            _clearSavesOnLoad = clearSavesOnLoad;
        }

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);
        public bool HasSaveFile => File.Exists(SaveFilePath);

        public void SaveBuilding(Building building)
        {
            _allBuildings.Add(building);
        }
        
        public void RemoveFromSaveList(Building building)
        {
            _allBuildings.Remove(building);
        }
        
        public void SaveBuildings()
        {
            GameSaveData saveData = CollectCurrentState();
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Saved {saveData.Buildings.Count} buildings to {SaveFilePath}");
        }

        public void LoadBuildings()
        {
            TryLoadBuildings();
        }

        public bool TryLoadBuildings()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning($"No save file found at {SaveFilePath}");
                return false;
            }

            string json = File.ReadAllText(SaveFilePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            if (saveData == null)
            {
                Debug.LogWarning("Save file is invalid or empty.");
                return false;
            }

            if (saveData.Buildings == null)
            {
                saveData.Buildings = new List<BuildingSaveData>();
            }

            if (_clearSavesOnLoad)
            {
                ClearPlacedBuildingsInScene();
            }

            if (_gridManager)
            {
                _gridManager.InitializeGrid();
            }

            RestoreState(saveData);
            return true;
        }

        [ContextMenu("Delete Save File")]
        public void DeleteSaveFile()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }
        }

        public bool TryLoadFromStartupMode(GameInitializer.StartupLoadMode loadMode)
        {
            switch (loadMode)
            {
                case GameInitializer.StartupLoadMode.None:
                    return false;
                case GameInitializer.StartupLoadMode.LoadIfSaveExists:
                    return HasSaveFile && TryLoadBuildings();
                case GameInitializer.StartupLoadMode.ForceLoad:
                    return TryLoadBuildings();
                default:
                    return false;
            }
        }

        private GameSaveData CollectCurrentState()
        {
            GameSaveData saveData = new GameSaveData();

            for (int i = 0; i < _allBuildings.Count; i++)
            {
                Building building = _allBuildings[i];
                if (building == null || !building.IsPlaced)
                {
                    continue;
                }

                string id = building.Id;
                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning($"Skipping building without Id: {building.name}", building);
                    continue;
                }

                saveData.Buildings.Add(new BuildingSaveData
                {
                    Id = id,
                    DisplayName = building.Data.DisplayName,
                    CellX = building.OriginCell.x,
                    CellY = building.OriginCell.y,
                    RotationSteps = building.PlacementRotationSteps,
                    Level = building.CurrentLevel
                });
            }

            return saveData;
        }

        private void RestoreState(GameSaveData saveData)
        {
            if (_gridManager == null)
            {
                Debug.LogError("GridManager is not assigned for BuildingSaveLoadManager.");
                return;
            }

            for (int i = 0; i < saveData.Buildings.Count; i++)
            {
                BuildingSaveData record = saveData.Buildings[i];
                if (record == null || string.IsNullOrWhiteSpace(record.Id))
                {
                    continue;
                }

                GameObject prefab = _buildingsList.buildings.First(x => x.DisplayName != record.DisplayName).Prefab;

                if (!prefab)
                {
                    Debug.LogWarning($"No prefab configured for building id '{record.Id}'.");
                    continue;
                }

                Building instance = Object.Instantiate(prefab).GetComponent<Building>();
                Vector2Int originCell = new Vector2Int(record.CellX, record.CellY);
                if (!_buildingPlacementManager.TryPlace(instance, originCell, record))
                {
                    Debug.LogWarning($"Failed to place building '{record.Id}' at {originCell}.");
                    Object.Destroy(instance.gameObject);
                    continue;
                }

                instance.RestoreProgress(record);
            }
        }

        private void ClearPlacedBuildingsInScene()
        {
            foreach (var building in _allBuildings)
            {
                if (!building || !building.IsPlaced)
                {
                    continue;
                }

                _buildingPlacementManager.RemoveFromGrid(building);
            }
        }
    }
}
