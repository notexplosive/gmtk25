using UnityEngine;

namespace SecretPlan.UI
{
    public abstract class ButtonDecoratorPerState<TData> : ButtonDecorator
    {
        [SerializeField]
        private ButtonDecoratorStates<TData> _states = new();

        protected ButtonDecoratorStates<TData> DataPerState => _states;
    }
}
