using Core.Initialization.SaveLoad;
using Core.UI.CoreMVP;

namespace Core.UI.Windows.Map
{
    public class MapController : Controller<MapModel, MapView>
    {
        protected override WindowID windowId => WindowID.Map;
    }
}
