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
        [SerializeField][ReadOnly]
        protected BuildingData _data;
        [SerializeField] private GameObject[] _levelModels;

        public string Id { get; private set; }

        public BuildingData Data => _data;
        public int CurrentLevel { get; protected set; } = 1;
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
            SetLevelModel();
        }

        public virtual void Interact()
        {
            ServiceLocator.Instance.Get<IUIService>().ShowActionsWindow(this);
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
                RestoreProgress(record);
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
            SetLevelModel();
        }

        protected void SetLevelModel()
        {
            if (_data.GetSafeMaxLevel() == 1)
            {
                return;
            }
            for (var i = 0; i < _levelModels.Length; i++)
            {
                _levelModels[i].SetActive(i == CurrentLevel - 1);
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
