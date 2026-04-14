using Core.Initialization.SaveLoad;
using Game.Buildings.Data;
using Vector2Int = UnityEngine.Vector2Int;

namespace Game.Buildings.Core
{
    public interface IBuilding
    {
        BuildingData Data { get; }
        int CurrentLevel { get; }
        bool IsPlaced { get; }
        Vector2Int OriginCell { get; }
        Vector2Int Footprint { get; }
        int PlacementRotationSteps { get; }

        void SetPlacementRotation(int clockwiseQuarterTurns);
        void RestoreProgress(BuildingSaveData saveData);

        void Interact();
    }
}
