using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using Core.UI.Windows.BuildingInfo;
using Game.Buildings.Core;

namespace Core.UI.Overlays.BuildingActions
{
    public class ActionsOverlayController : Controller<BuildingInfoModel, ActionsOverlayView>
    {
        protected override WindowID windowId => WindowID.ActionsOverlay;
        private IBuildingService _buildingService = ServiceLocator.Instance.Get<IBuildingService>();

        public override void Open()
        {
            base.Open();
            View.OnInfoClick += OpenInfo;
            View.OnUpgradeClick += UpgradeBuilding;
        }

        private void UpgradeBuilding(Building building)
        {
            if (_buildingService.TryUpgrade(building))
            {
                _uiService.CloseWindow(WindowID.ActionsOverlay);
            }
        }

        private void OpenInfo(Building building)
        {
            _uiService.ShowBuildingInfo(building);
            Close();
        }
    }
}