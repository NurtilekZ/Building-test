using Core.Initialization.SaveLoad;
using Core.UI.CoreMVP;

namespace Core.UI.Windows.BuildingInfo
{
    public class BuildingInfoController : Controller<BuildingInfoModel, BuildingInfoView>
    {
        protected override WindowID windowId => WindowID.BuildingInfo;
    }
}