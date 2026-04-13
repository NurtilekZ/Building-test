using Core.Initialization.SaveLoad;
using Core.UI.CoreMVP;

namespace Core.UI.Windows.HUD
{
    public class HUDController : Controller<HUDModel, HUDView>
    {
        protected override WindowID windowId => WindowID.HUD;
        
        public override void Open()
        {
            base.Open();
            View.OnMapBtnClick += OpenMapWindow;
        }

        public override void Close()
        {
            base.Close();
            View.OnMapBtnClick -= OpenMapWindow;
        }

        private void OpenMapWindow()
        {
            _uiService.ShowMapWindow();
        }
    }
}