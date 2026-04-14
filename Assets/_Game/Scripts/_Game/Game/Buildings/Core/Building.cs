using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using Game.Buildings.Data;
using Unity.Collections;
using UnityEngine;

namespace Game.Buildings.Core
{
    public abstract class Building : MonoBehaviour, IBuilding
    {
        [Header("Defaults")]
        [SerializeField][ReadOnly] private BuildingData _data;
        [SerializeField] private GameObject[] _levelModels;

        public string Id { get; private set; }

        public BuildingData Data => _data;
        public int CurrentLevel { get; private set; } = 1;
        public bool IsPlaced { get; private set; }
        public Vector2Int OriginCell { get; private set; }
        public Vector2Int Footprint => GetRotatedFootprint(_data.GetSafeFootprint(), PlacementRotationSteps);
        public int PlacementRotationSteps { get; private set; }

        public void BindData(BuildingData data)
        {
            _data = data;
        }

        public void SetPlacementRotation(int clockwiseQuarterTurns)
        {
            PlacementRotationSteps = ((clockwiseQuarterTurns % 4) + 4) % 4;
            transform.rotation = Quaternion.Euler(0f, PlacementRotationSteps * 90f, 0f);
        }

        public void RestoreProgress(BuildingSaveData saveData)
        {
            Id = saveData.Id;
            
            SetPlacementRotation(saveData.RotationSteps);
            CurrentLevel = saveData.Level;
            SetLevelModel(CurrentLevel);
        }

        public virtual void Interact()
        {
            // TryUpgrade();
            ServiceLocator.Instance.Get<IUIService>().ShowActionOverlay(this, transform);
        }

        private static Vector2Int GetRotatedFootprint(Vector2Int footprint, int quarterTurns)
        {
            int normalizedQuarterTurns = ((quarterTurns % 4) + 4) % 4;
            if (normalizedQuarterTurns % 2 == 0)
            {
                return footprint;
            }

            return new Vector2Int(footprint.y, footprint.x);
        }

        public void Place(Vector2Int originCell, BuildingData buildingData, BuildingSaveData record)
        {
            _data = buildingData;
            if (record != null)
            {
                Id = record.Id;
                CurrentLevel = record.Level;
                SetPlacementRotation(record.RotationSteps);
            }
            else
            {
                Id = GUID.Generate().ToString();
                CurrentLevel = 1;
            }
            OriginCell = originCell;
            IsPlaced = true;
            OnPlaced();
        }

        protected virtual void OnPlaced()
        {
            SetLevelModel(CurrentLevel);
        }

        public bool CanUpgrade()
        {
            return CurrentLevel < _data.GetSafeMaxLevel();
        }

        public int GetCurrentUpgradeCost()
        {
            return _data.GetUpgradeCostForLevel(CurrentLevel);
        }

        public bool Upgrade()
        {
            CurrentLevel++;
            OnUpgraded(CurrentLevel);
            return true;
        }

        protected virtual void OnUpgraded(int newLevel)
        {
            SetLevelModel(newLevel);
        }

        private void SetLevelModel(int newLevel)
        {
            foreach (var levelModel in _levelModels)
            {
                levelModel.SetActive(CurrentLevel == newLevel);
            }
        }

        public void Remove()
        {
            OnRemovedFromGrid();
            Destroy(gameObject);
        }

        protected virtual void OnRemovedFromGrid()
        {
            IsPlaced = false;
        }
    }
}
