using System.Collections.Generic;
using UnityEngine;

namespace SecretPlan.Core
{
    public class AddressableOwner : MonoBehaviour
    {
        private readonly List<IAddressable> _ownedReferences = new();

        private void OnDestroy()
        {
            foreach (var reference in _ownedReferences)
            {
                reference.Release();
            }
        }

        public void GainOwnership(IAddressable addressable)
        {
            _ownedReferences.Add(addressable);
        }
    }
}