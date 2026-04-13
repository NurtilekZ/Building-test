using System;
using Core.UI.CoreMVP;
using Core.UI.Windows.BuildingList.Items;
using Game.Buildings.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.BuildingList
{
    public class BuildingListView : View<BuildingListModel>
    {
        [SerializeField] private Transform _listContainer;
        [SerializeField] private BuildingItemView _buildingItemPrefab;
        [SerializeField] private Button _closeBtn;
        
        public event Action OnCloseBtnPress;
        public event Action<BuildingData> OnBuildingSelect;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(() => OnCloseBtnPress?.Invoke());
        }

        protected override void Render()
        {
            if (_listContainer.childCount != 0) return;
            foreach (var buildingData in Model.BuildingDataList)
            {
                var buildingItem = Instantiate(_buildingItemPrefab, _listContainer);
                buildingItem.BindData(buildingData);
                buildingItem.OnBtnClick += OnBuildingSelect;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _closeBtn.onClick.RemoveAllListeners();
        }
    }
}