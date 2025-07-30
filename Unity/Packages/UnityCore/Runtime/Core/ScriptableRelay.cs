using UnityEngine;

namespace SecretPlan.Core
{
    /// <summary>
    ///     ScriptableObject that stores a State object that persists with the scene
    /// </summary>
    public abstract class ScriptableRelay<TState> : ScriptableObject
    {
        [SerializeField]
        private LifetimeType _lifetimeType;

        private TState? _cachedState;

        /// <summary>
        ///     If the proxy exists, we reuse cachedState, if the proxy does not exist, we generate new state. We expect the proxy
        ///     to be destroyed via scene loading. The proxy itself holds no state nor components.
        /// </summary>
        private GameObject? _proxyObject;

        protected abstract TState CreateState();

        /// <summary>
        ///     Optional way to manually destroy the state.
        /// </summary>
        public void ClearState()
        {
            Destroy(_proxyObject);
        }

        public TState State()
        {
            if (_proxyObject == null)
            {
                _cachedState = CreateState();
                _proxyObject = new GameObject($"{GetType().Name} Proxy");

                if (_lifetimeType == LifetimeType.Application)
                {
                    DontDestroyOnLoad(_proxyObject);
                }
            }

            return GetCachedState();
        }

        private TState GetCachedState()
        {
            if (_cachedState == null)
            {
                _cachedState = CreateState();
            }

            return _cachedState;
        }

        private enum LifetimeType
        {
            CurrentScene = 0,
            Application = 1
        }
    }
}