using Core.Initialization.SaveLoad;
using Core.UI.CoreMVP;

namespace Core.UI.Windows.BuildingInfo
{
    public class BuildingInfoController : Controller<BuildingInfoModel, BuildingInfoView>
    {
        protected override WindowID windowId => WindowID.BuildingInfo;

        public override void Open()
        {
            base.Open();
            View.OnCloseClick += OnClickClose;
        }

        public override void Close()
        {
            base.Close();
            View.OnCloseClick -= OnClickClose;
        }
    }
}