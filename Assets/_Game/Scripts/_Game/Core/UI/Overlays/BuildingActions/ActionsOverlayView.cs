using System;
using Core.UI.CoreMVP;
using Core.UI.Windows.BuildingInfo;
using Game.Buildings.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Overlays.BuildingActions
{
    public class ActionsOverlayView : OverlayView<BuildingInfoModel>
    {
        [SerializeField] private Button _infoBtn;
        [SerializeField] private Button _upgradeBtn;
        [SerializeField] private Button _removeBtn;
        
        public event Action<Building> OnInfoClick;
        public event Action<Building> OnUpgradeClick;
        public event Action<Building> OnRemoveClick;

        protected override void Awake()
        {
            base.Awake();
            _infoBtn.onClick.AddListener(() => OnInfoClick?.Invoke(Model.Building));
            _upgradeBtn.onClick.AddListener(()  => OnUpgradeClick?.Invoke(Model.Building));
            _removeBtn.onClick.AddListener(()  => OnRemoveClick?.Invoke(Model.Building));
        }

        protected override void OnDestroy()
        {
            _infoBtn.onClick.RemoveAllListeners();
            _upgradeBtn.onClick.RemoveAllListeners();
            _removeBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }
    }
}