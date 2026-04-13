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

        public override void Open()
        {
            base.Open();
            View.OnCloseBtnPress += OnClickClose;
            View.OnStartBuild += StartPlacement;
        }

        public override void Close()
        {
            base.Close();
            View.OnCloseBtnPress -= OnClickClose;
            View.OnStartBuild -= StartPlacement;
        }

        private void StartPlacement(BuildingData buildingData)
        {
            IGridService gridService = ServiceLocator.Instance.Get<IGridService>();
            Vector2Int footprint = buildingData.GetSafeFootprint();

            if (gridService.CanPlaceAtCell(Model.SelectedCell, footprint))
            {
                _buildingService.StartPlacement(buildingData);
                _uiService.CloseWindow(WindowID.BuildingsList);
            }
        }
    }
}