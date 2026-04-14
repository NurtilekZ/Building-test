using UnityEngine;

namespace Core.UI.CoreMVP.Overlay
{
    public interface IOverlayController : IController
    {
        void BindTransform(Transform transform);
    }
}