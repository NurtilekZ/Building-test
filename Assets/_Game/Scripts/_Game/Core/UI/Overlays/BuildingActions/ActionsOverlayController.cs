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
            View.OnUpgradeClick += Upgrade;
            View.OnRemoveClick += Remove;
        }

        private void Remove(Building building)
        {
            if (_buildingService.RemoveBuilding(building))
            {
                Close();
            }
        }

        private void Upgrade(Building building)
        {
            if (_buildingService.TryUpgrade(building))
            { 
                Close();
            }
        }

        private void OpenInfo(Building building)
        {
            Close();
            _uiService.ShowBuildingInfo(building);
        }
    }
}