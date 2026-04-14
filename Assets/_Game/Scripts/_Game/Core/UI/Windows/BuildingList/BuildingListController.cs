using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using Game.Buildings.Data;
using UnityEngine;

namespace Core.UI.Windows.BuildingList
{
    public class BuildingListController : Controller<BuildingListModel, BuildingListView>
    {
        protected override WindowID windowId => WindowID.BuildingsList;

        private readonly IBuildingService _buildingService = ServiceLocator.Instance.Get<IBuildingService>();
        private readonly IGridService _gridService = ServiceLocator.Instance.Get<IGridService>();

        public override void Open()
        {
            base.Open();
            View.OnStartBuild += StartPlacement;
        }

        public override void Close()
        {
            View.OnStartBuild -= StartPlacement;
            base.Close();
        }
        private void StartPlacement(BuildingData buildingData)
        {
            _buildingService.StartPlacement(buildingData);
            _uiService.CloseWindow(WindowID.BuildingsList);
        }
    }
}