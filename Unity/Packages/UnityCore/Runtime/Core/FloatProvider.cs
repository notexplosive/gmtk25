using System;

namespace SecretPlan.Core
{
    public class FloatProvider : ValueProvider<float>
    {
        public FloatProvider(float data) : base(data)
        {
        }

        public FloatProvider(Func<float> providerFunction) : base(providerFunction)
        {
        }

        public FloatProvider() : this(0)
        {
        }
    }
}