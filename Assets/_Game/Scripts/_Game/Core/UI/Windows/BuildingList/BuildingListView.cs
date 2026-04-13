using System;
using System.Collections.Generic;
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
        [SerializeField] private Button _buildBtn;
        [SerializeField] private Button _closeBtn;
        
        private BuildingData _selectedBuilding;

        public event Action OnCloseBtnPress;
        public event Action<BuildingData> OnStartBuild;

        private readonly List<BuildingItemView> _buildingItems = new();

        private void Awake()
        {
            _closeBtn.onClick.AddListener(() => OnCloseBtnPress?.Invoke());
            _buildBtn.onClick.AddListener(() => OnStartBuild?.Invoke(_selectedBuilding));
        }

        protected override void Render()
        {
            if (_listContainer.childCount != 0) return;
            foreach (var buildingData in Model.BuildingDataList)
            {
                var buildingItem = Instantiate(_buildingItemPrefab, _listContainer);
                buildingItem.BindData(buildingData);
                buildingItem.OnBtnClick += SetSelectedBuilding;
                if (!_buildingItems.Contains(buildingItem)) 
                    _buildingItems.Add(buildingItem);
            }
        }

        private void SetSelectedBuilding(BuildingData building)
        {
            _selectedBuilding = building;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _closeBtn.onClick.RemoveAllListeners();
            _buildBtn.onClick.RemoveAllListeners();
        }

        public override void Dispose()
        {
            foreach (var buildingItem in _buildingItems)
            {
                buildingItem.Dispose();
            }
            base.Dispose();
        }
    }
}