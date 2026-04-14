using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
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
            View.OnClearBtnClick += ClearGrid;
        }

        private void ClearGrid()
        {
            ServiceLocator.Instance.Get<ISaveLoadService>().ClearPlacedBuildingsInScene();
        }

        public override void Close()
        {
            base.Close();
            View.OnMapBtnClick -= OpenMapWindow;
            View.OnClearBtnClick -= ClearGrid;
        }

        private void OpenMapWindow()
        {
            _uiService.ShowMapWindow();
        }
    }
}