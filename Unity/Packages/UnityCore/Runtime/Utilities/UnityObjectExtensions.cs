using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SecretPlan.Core
{
    public static class UnityObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var found = gameObject.GetComponent<T>();
            return found != null ? found : gameObject.AddComponent<T>();
        }
        
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return GetOrAddComponent<T>(component.gameObject);
        }

        /// <summary>
        /// Runs Unity Lifetime Check, returns false if the object is null or the object is destroyed 
        /// </summary>
        public static bool IsNotNull([NotNullWhen(true)] this Object? unityObject)
        {
            return unityObject;
        }
        
        public static bool IsNull(this Object? unityObject)
        {
            return !unityObject;
        }

        public static void GainOwnershipOfAddressable(this Component component, IAddressable addressable)
        {
            component.GetOrAddComponent<AddressableOwner>().GainOwnership(addressable);
        }
    }
}
