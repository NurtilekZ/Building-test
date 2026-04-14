using Game.Buildings.Core;

namespace Game.Buildings
{
    public class UpgradableBuilding : Building
    {
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

        private void OnUpgraded(int newLevel)
        {
            SetLevelModel();
        }
    }
}