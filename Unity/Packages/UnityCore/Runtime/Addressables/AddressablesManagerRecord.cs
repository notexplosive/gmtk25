using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SecretPlan.Core
{
    internal class AddressablesManagerRecord<T> : IAddressableManagerRecord
    {
        private AsyncOperationHandle<T> _handle;
        private int _referenceCount;

        public AddressablesManagerRecord(string address)
        {
            _handle = Addressables.LoadAssetAsync<T>(address);
            IncrementCount();
        }

        public void IncrementCount()
        {
            _referenceCount++;
        }

        public void DecrementCount()
        {
            _referenceCount--;
        }

        public bool IsEmpty()
        {
            return _referenceCount <= 0;
        }

        public bool IsReady()
        {
            return _handle.IsDone && _handle.Status == AsyncOperationStatus.Succeeded;
        }

        public void Release()
        {
            Addressables.Release(_handle);
        }

        public void ForceLoadNow()
        {
            _handle.WaitForCompletion();
        }

        public T Result()
        {
            return _handle.Result;
        }
    }
}