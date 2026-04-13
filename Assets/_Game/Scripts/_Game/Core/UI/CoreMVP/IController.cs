using System;

namespace Core.UI.CoreMVP
{
    public interface IController :  IDisposable
    {
        bool IsOpen { get; }
        void Bind(Model model, IView view = null);
        void Open();
        void Close();
    }
}