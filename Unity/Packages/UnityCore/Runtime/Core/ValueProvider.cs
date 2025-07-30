using System;

namespace SecretPlan.Core
{
    public interface IValueProvider<out TData>
    {
        public TData Get();
    }

    public abstract class ValueProvider<TData> : IValueProvider<TData> where TData : new()
    {
        private Func<TData>? _providerFunction;

        protected ValueProvider(TData data)
        {
            SetFlatValue(data);
        }

        protected ValueProvider(Func<TData> providerFunction)
        {
            SetProvider(providerFunction);
        }

        public TData Get()
        {
            if (_providerFunction == null)
            {
                var flatValue = new TData();
                _providerFunction = () => flatValue;
            }

            return _providerFunction();
        }

        public void SetFlatValue(TData value)
        {
            _providerFunction = () => value;
        }

        public void SetProvider(Func<TData> providerFunction)
        {
            _providerFunction = providerFunction;
        }
    }
}
