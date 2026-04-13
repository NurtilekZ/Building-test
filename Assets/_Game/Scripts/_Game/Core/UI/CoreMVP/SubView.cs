using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.CoreMVP
{
    public abstract class SubView<TData> : MonoBehaviour, IDisposable
    {
        protected readonly List<IDisposable> _disposableList = new();

        public abstract void BindData(TData data);

        public virtual void Dispose()
        {
            foreach (var disposable in _disposableList)
                disposable?.Dispose();
            _disposableList.Clear();
        }
    }
}