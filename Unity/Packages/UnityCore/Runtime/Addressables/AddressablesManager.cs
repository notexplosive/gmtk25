using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SecretPlan.Core
{
    public static class AddressablesManager
    {
        private static readonly Dictionary<string, IAddressableManagerRecord> _addressToRecord = new();

        public static PendingAddressable<T> LoadAsync<T>(string address) where T : Object
        {
            if (_addressToRecord.TryGetValue(address, out var record))
            {
                record.IncrementCount();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    _addressToRecord[address] = new EmptyAddressableRecord();
                }
                else
                {
                    _addressToRecord[address] = new AddressablesManagerRecord<T>(address);
                }
            }

            return new PendingAddressable<T>(_addressToRecord[address]);
        }

        public static void Release(string address)
        {
            if (!_addressToRecord.TryGetValue(address, out var record))
            {
                return;
            }

            record.DecrementCount();

            if (!record.IsEmpty())
            {
                return;
            }

            record.Release();
            _addressToRecord.Remove(address);

            if (_addressToRecord.Count == 0)
            {
                Debug.Log("!! Zero Addressables Referenced - Clean Slate !!");
            }
        }
    }
}