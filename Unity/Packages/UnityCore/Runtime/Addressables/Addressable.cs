using UnityEngine;

namespace SecretPlan.Core
{
    public class Addressable<T> : IAddressable where T : Object
    {
        private readonly string _address;

        public Addressable(string address)
        {
            _address = address;
        }

        public PendingAddressable<T> LoadAsync(Component owner)
        {
            var result = AddressablesManager.LoadAsync<T>(_address);
            owner.GainOwnershipOfAddressable(this);
            return result;
        }

        public void Release()
        {
            AddressablesManager.Release(_address);
        }
    }
}