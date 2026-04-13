using System;
using System.Collections.Generic;

namespace Core.UI.CoreMVP
{
    [Serializable]
    public abstract class Model : IModel
    {
        protected readonly List<IDisposable> _disposableList = new();
        
        public event Action Changed;

        protected void NotifyChanged()
        {
            Changed?.Invoke();
        }

        public virtual void Dispose()
        {
            foreach (var disposable in _disposableList)
                disposable?.Dispose();
            _disposableList.Clear();
        }
    }

    public interface IModel : IDisposable
    {
        public event Action Changed;
    }
}