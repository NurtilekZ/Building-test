using Core.UI.CoreMVP;
using R3;

namespace Core.UI.Windows.HUD
{
    public class HUDModel : Model
    {
        public ReadOnlyReactiveProperty<int> Balance;
        
        public override void Dispose()
        {
            Balance?.Dispose();
        }
    }
}