using System;
using Core.UI.Windows;
using Core.UI.Windows.BuildingInfo;
using Core.UI.Windows.BuildingList;
using Core.UI.Windows.BuildMode;
using Core.UI.Windows.HUD;
using Core.UI.Windows.Map;
using Game.Assets;
using Game.Buildings.Core;
using Game.Buildings.Data;
using UnityEngine;
using WindowID = Core.Initialization.SaveLoad.WindowID;

namespace Core.Initialization.Services.UI
{
    public class UIService : IUIService
    {
        private readonly UIWindowManager _windowManager;
        private readonly BuildingsListSO _buildingsList;

        public UIService(UIWindowManager windowManager, BuildingsListSO buildingsList)
        {
            _windowManager = windowManager;
            _buildingsList = buildingsList;
        }

        public void ShowBuildingList(Vector2Int cell)
        {
            BuildingData[] buildings = _buildingsList != null ? _buildingsList.buildings : Array.Empty<BuildingData>();
            BuildingListModel model = new BuildingListModel(buildings, cell);
            _windowManager.OpenWindow(WindowID.BuildingsList, model, closeOthers: false);
        }

        public void ShowMapWindow()
        {
            MapModel model = new MapModel();
            _windowManager.OpenWindow(WindowID.Map, model, closeOthers: false);
        }

        public void ShowBuildMode()
        {
            _windowManager.OpenWindow(WindowID.BuildingMode, new BuildModeModel(), false);
        }

        public void ShowActionOverlay(Building building, Transform transform)
        {
            _windowManager.OpenOverlay(WindowID.ActionsOverlay, new BuildingInfoModel(building), transform);
        }

        public void CloseWindow(WindowID id)
        {
            _windowManager.CloseWindow(id);
        }

        public void ShowBuildingInfo(Building building)
        {
            BuildingInfoModel model = new BuildingInfoModel(building);
            _windowManager.OpenWindow(WindowID.BuildingInfo, model);
        }

        public void ShowHUD()
        {
            _windowManager.OpenWindow(WindowID.HUD, new HUDModel(), false, true);
        }

        public void Dispose()
        {
            
        }
    }
}
