using System;
using Core.UI.CoreMVP;
using Core.UI.Windows.BuildingInfo;
using Game.Buildings;
using Game.Buildings.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.BuildingActions
{
    public class BuildingActionsView : View<BuildingInfoModel>
    {
        [SerializeField] private TMP_Text _buildingName;
        [SerializeField] private Button _infoBtn;
        [SerializeField] private Button _upgradeBtn;
        [SerializeField] private Button _removeBtn;
        [SerializeField] private Button _closeBtn;
        
        public event Action<Building> OnInfoClick;
        public event Action<UpgradableBuilding> OnUpgradeClick;
        public event Action<Building> OnRemoveClick;
        public override event Action OnCloseClicked;

        private void Awake()
        {
            _infoBtn.onClick.AddListener(() => OnInfoClick?.Invoke(Model.Building));
            _upgradeBtn.onClick.AddListener(()  => OnUpgradeClick?.Invoke((UpgradableBuilding)Model.Building));
            _removeBtn.onClick.AddListener(()  => OnRemoveClick?.Invoke(Model.Building));
            _closeBtn.onClick.AddListener(()  => OnCloseClicked?.Invoke());
        }

        protected override void Render()
        {
            var upgradableBuilding = (UpgradableBuilding)Model.Building;
            _upgradeBtn.gameObject.SetActive(upgradableBuilding != null && upgradableBuilding.CanUpgrade());
            _buildingName.text = $"{Model.Building.Data.DisplayName} - Level {Model.Building.CurrentLevel}";
        }

        protected override void OnDestroy()
        {
            _infoBtn.onClick.RemoveAllListeners();
            _upgradeBtn.onClick.RemoveAllListeners();
            _removeBtn.onClick.RemoveAllListeners();
            _closeBtn.onClick.RemoveAllListeners();
        }
    }
}