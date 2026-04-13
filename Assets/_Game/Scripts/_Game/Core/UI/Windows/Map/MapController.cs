using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using UnityEngine;

namespace Core.UI.Windows.Map
{
    public class MapController : Controller<MapModel, MapView>
    {
        private readonly IGridService _gridService = ServiceLocator.Instance.Get<IGridService>();

        protected override WindowID windowId => WindowID.Map;
        public override void Open()
        {
            base.Open();
            View.OnCloseBtnPress += OnClickClose;
        }

        public override void Close()
        {
            base.Close();
            View.OnCloseBtnPress -= OnClickClose;
        }
    }
}
