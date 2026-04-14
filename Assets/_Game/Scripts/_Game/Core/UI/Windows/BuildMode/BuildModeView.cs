using System;
using Core.UI.CoreMVP;

namespace Core.UI.Windows.BuildMode
{
    public class BuildModeView : View<BuildModeModel> 
    {
        public override event Action OnCloseClicked;

        protected override void Render()
        {
            
        }
    }
}