using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.CoreMVP
{
    public abstract class View<TModel> : MonoBehaviour, IView where TModel : IModel
    {
        protected TModel Model { get; private set; }
        
        public bool IsOpen { get; private set; }
        
        protected readonly List<IDisposable> _disposableList = new();
        
        public abstract event Action OnCloseClicked;

        public virtual void Bind(IModel model)
        {
            UnbindCurrentModel();
            Model = (TModel)model;

            if (Model != null)
            {
                Model.Changed += HandleModelChanged;
            }
        }

        public virtual void Show()
        {
            IsOpen = true;
            gameObject.SetActive(true);
            Render();
        }

        public virtual void Hide()
        {
            IsOpen = false;
            gameObject.SetActive(false);
        }

        protected abstract void Render();

        protected virtual void HandleModelChanged()
        {
            Render();
        }

        protected virtual void OnDestroy()
        {
            UnbindCurrentModel();
        }

        private void UnbindCurrentModel()
        {
            if (Model != null)
            {
                Model.Changed -= HandleModelChanged;
            }
        }

        public virtual void Dispose()
        {
            foreach (var disposable in _disposableList)
                disposable?.Dispose();
        }
    }
}