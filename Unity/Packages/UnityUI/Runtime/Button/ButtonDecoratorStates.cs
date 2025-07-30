using System;
using UnityEngine;

namespace SecretPlan.UI
{
    [Serializable]
    public class ButtonDecoratorStates<TData>
    {
        [SerializeField]
        protected TData? _normal;

        [SerializeField]
        protected TData? _navigatedTo;

        [SerializeField]
        protected TData? _pressed;

        [SerializeField]
        protected TData? _disabled;

        public TData? GetDataFromState(NavigationState state)
        {
            switch (state)
            {
                case NavigationState.Normal:
                    return _normal;
                case NavigationState.Pressed:
                    return _pressed;
                case NavigationState.NavigatedTo:
                    return _navigatedTo;
                case NavigationState.Disabled:
                    return _disabled;
            }

            Debug.LogWarning($"Unknown NavigationState: {state}");
            return default!;
        }

        public void SetDataForState(NavigationState state, TData data)
        {
            switch (state)
            {
                case NavigationState.Normal:
                    _normal = data;
                    break;
                case NavigationState.Pressed:
                    _pressed = data;
                    break;
                case NavigationState.NavigatedTo:
                    _navigatedTo = data;
                    break;
                case NavigationState.Disabled:
                    _disabled = data;
                    break;
                default:
                    Debug.LogWarning($"Unknown NavigationState: {state}");
                    break;
            }
        }
    }
}
