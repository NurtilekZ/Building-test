using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using Core.UI.Windows.BuildingInfo;
using Game.Buildings;
using Game.Buildings.Core;

namespace Core.UI.Windows.BuildingActions
{
    public class BuildingActionsController : Controller<BuildingInfoModel, BuildingActionsView>
    {
        protected override WindowID windowId => WindowID.ActionsOverlay;
        private readonly IBuildingService _buildingService = ServiceLocator.Instance.Get<IBuildingService>();

        public override void Open()
        {
            base.Open();
            View.OnInfoClick += OpenInfo;
            View.OnUpgradeClick += Upgrade;
            View.OnRemoveClick += Remove;
        }

        public override void Close()
        {
            View.OnInfoClick -= OpenInfo;
            View.OnUpgradeClick -= Upgrade;
            View.OnRemoveClick -= Remove;
            base.Close();
        }
   

        private void Remove(Building building)
        {
            if (_buildingService.RemoveBuilding(building))
            {
                Close();
            }
        }

        private void Upgrade(UpgradableBuilding building)
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