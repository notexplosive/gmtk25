using System;

namespace SecretPlan.Core
{
    public class BoolProvider : ValueProvider<bool>
    {
        public BoolProvider(bool data) : base(data)
        {
        }

        public BoolProvider(Func<bool> providerFunction) : base(providerFunction)
        {
        }
    }
}
