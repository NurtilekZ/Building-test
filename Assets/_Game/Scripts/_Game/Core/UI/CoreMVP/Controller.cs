using System;
using System.Collections.Generic;
using Core.Initialization.SaveLoad;
using Core.Initialization.Services;

namespace Core.UI.CoreMVP
{
    public abstract class Controller<TModel, TView> : IController where TModel : Model where TView : View<TModel>
    {
        public TModel Model { get; private set; }
        public TView View { get; private set; }

        protected abstract WindowID windowId { get; }

        protected readonly IUIService _uiService = ServiceLocator.Instance.Get<IUIService>();

        protected readonly List<IDisposable> _disposableList = new();

        public bool IsOpen { get; private set; }

        public virtual void Bind(Model model, IView view = null)
        {
            Model = (TModel)model;
            if (view != null)
            {
                View = (TView)view;
            }
        }

        public virtual void Open()
        {
            View.Bind(Model);
            View.Show();
            IsOpen = true;
        }

        public virtual void Close()
        {
            View.Hide();
            IsOpen = false;
            Dispose();
        }

        protected virtual void OnClickClose()
        {
            _uiService.CloseWindow(windowId);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposableList)
                disposable?.Dispose();
            _disposableList.Clear();
            Model?.Dispose();
            View?.Dispose();
        }
    }
}
