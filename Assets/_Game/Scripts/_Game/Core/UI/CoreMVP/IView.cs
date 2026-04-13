using System;

namespace Core.UI.CoreMVP
{
    public interface IView  : IDisposable
    {
        void Bind(IModel model);
        bool IsOpen { get; }
        void Show();
        void Hide();
    }
}