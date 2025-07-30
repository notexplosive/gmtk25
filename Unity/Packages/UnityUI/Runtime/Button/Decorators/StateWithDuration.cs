using System;
using UnityEngine;

namespace SecretPlan.UI
{
    [Serializable]
    public struct StateWithDuration<TState> where TState : unmanaged
    {
        [SerializeField]
        private TState _state;

        [SerializeField]
        private float _duration;

        public TState Value
        {
            get => _state;
            set => _state = value;
        }

        public float Duration => _duration;
    }
}
