using Game.Buildings.Data;
using UnityEngine;

namespace Game.Assets
{
    [CreateAssetMenu(fileName = "BuildingDataList", menuName = "_GameCustomSO/BuildingDataList")]
    public class BuildingsListSO : ScriptableObject
    {
        public BuildingData[] buildings;
    }
}