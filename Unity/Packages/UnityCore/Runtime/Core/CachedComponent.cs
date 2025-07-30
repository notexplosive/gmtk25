using System;
using UnityEngine;

namespace SecretPlan.Core
{
    public class CachedComponent<TComponent> where TComponent : Component
    {
        private readonly ComponentSearchMode _searchMode;
        private TComponent? _cachedResult;

        public CachedComponent(ComponentSearchMode searchMode = ComponentSearchMode.Self)
        {
            _searchMode = searchMode;
        }

        /// <summary>
        ///     Confidently gets the component (throws if it's not there)
        /// </summary>
        public TComponent Get(Component self)
        {
            if (_cachedResult != null)
            {
                return _cachedResult;
            }

            var found = GetOrNull(self);

            if (found != null)
            {
                return found;
            }

            throw new Exception(
                $"CachedComponent failed to get {typeof(TComponent).Name} from GameObject {self.gameObject}");
        }

        public TComponent? GetOrNull(Component self)
        {
            if (_cachedResult != null)
            {
                return _cachedResult;
            }

            var found = InternalGet(self);
            _cachedResult = found;

            return found;
        }

        public TComponent GetOrAdd(Component self)
        {
            if (_cachedResult != null)
            {
                return _cachedResult;
            }

            var found = GetOrNull(self);

            if (found == null)
            {
                _cachedResult = self.gameObject.AddComponent<TComponent>();
                found = _cachedResult;
            }

            return found;
        }

        private TComponent? InternalGet(Component self)
        {
            if (_searchMode == ComponentSearchMode.Self)
            {
                return self.GetComponent<TComponent>();
            }

            if (_searchMode == ComponentSearchMode.InParent)
            {
                return self.GetComponentInParent<TComponent>(true);
            }

            if (_searchMode == ComponentSearchMode.InFirstFoundChild)
            {
                return self.GetComponentInChildren<TComponent>(true);
            }

            return null;
        }

        public bool Exists(Component self)
        {
            return GetOrNull(self) != null;
        }
    }
}
