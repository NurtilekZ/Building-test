using Core.Initialization.SaveLoad;
using Core.UI.CoreMVP;

namespace Core.UI.Windows.BuildMode
{
    public class BuildModeController : Controller<BuildModeModel, BuildModeView>
    {
        protected override WindowID windowId => WindowID.BuildingMode;
        
        
    }
}