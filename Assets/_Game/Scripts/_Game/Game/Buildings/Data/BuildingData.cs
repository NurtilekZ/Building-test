using UnityEngine;

namespace Game.Buildings.Data
{
    [System.Serializable]
    public struct BuildingData
    {
        [Header("Identity")]
        public string DisplayName;
        public string Description;

        [Header("Visual")]
        public GameObject Prefab;
        public Sprite[] Icon;

        [Header("Layout")]
        public Vector2Int Footprint;

        [Header("Stats")]
        public int MaxLevel;

        [Header("Economy")]
        public int BuildCost;
        public int UpgradeCostStep;

        public Vector2Int GetSafeFootprint()
        {
            return new Vector2Int(Mathf.Max(1, Footprint.x), Mathf.Max(1, Footprint.y));
        }

        public int GetUpgradeCostForLevel(int currentLevel)
        {
            int clampedLevel = Mathf.Max(1, currentLevel);
            return Mathf.Max(0, BuildCost + ((clampedLevel - 1) * Mathf.Max(0, UpgradeCostStep)));
        }

        public int GetSafeMaxLevel()
        {
            return Mathf.Max(1, MaxLevel);
        }
    }
}
