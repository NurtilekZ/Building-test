using System;
using System.Collections.Generic;

namespace Core.Initialization.Services
{
    public interface IServiceLocator
    {
        T Get<T>() where T : class;
        void Register<T>(T service) where T : class;
        void Unregister<T>() where T : class;
        bool IsRegistered<T>() where T : class;
    }

    public sealed class ServiceLocator : IServiceLocator
    {
        private static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();

        private ServiceLocator() { }

        public T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (_factories.TryGetValue(type, out Func<object> factory))
            {
                return factory() as T;
            }

            if (_services.TryGetValue(type, out object service))
            {
                return service as T;
            }

            return null;
        }

        public void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        public void RegisterFactory<T>(Func<T> factory) where T : class
        {
            _factories[typeof(T)] = () => factory();
        }

        public void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
            _factories.Remove(typeof(T));
        }

        public bool IsRegistered<T>() where T : class
        {
            return _services.ContainsKey(typeof(T)) || _factories.ContainsKey(typeof(T));
        }

        public void Clear()
        {
            _services.Clear();
            _factories.Clear();
        }

        public static void Shutdown()
        {
            _instance?.Clear();
            _instance = null;
        }
    }
}
