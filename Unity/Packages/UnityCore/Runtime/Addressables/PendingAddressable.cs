using System.Collections;
using UnityEngine;

namespace SecretPlan.Core
{
    public readonly struct PendingAddressable<T> where T : Object
    {
        private readonly IAddressableManagerRecord _record;

        internal PendingAddressable(IAddressableManagerRecord record)
        {
            _record = record;
        }

        public IEnumerator WaitUntilReady()
        {
            while (!_record.IsReady())
            {
                yield return null;
            }
        }

        public bool IsReady()
        {
            return _record.IsReady();
        }

        public T? Asset()
        {
            if (!IsReady())
            {
                Debug.LogError($"Attempted to get Asset from {nameof(PendingAddressable<T>)} before it was ready");
                return null;
            }
            
            var casted = _record as AddressablesManagerRecord<T>;
            return casted?.Result();
        }
    }
}