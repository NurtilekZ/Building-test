using UnityEngine;

namespace Core.UI.CoreMVP.Overlay
{
    public abstract class OverlayController<TModel, TView> : Controller<TModel, TView>, IOverlayController
        where TModel : Model where TView : OverlayView<TModel>
    {
        public virtual void BindTransform(Transform transform)
        {
            View.Bind(transform);
        }
    }
}