using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace SecretPlan.Core
{
    /// <summary>
    ///     Utility class to hold onto a reference to an object so it never gets de-allocated.
    /// </summary>
    public static class PersistentReference
    {
        /// <summary>
        ///     Hold onto references long-term so Unity (and Addressables) never drops them.
        /// </summary>
        [Preserve]
        private static HashSet<Object> allReferences = new();

        public static void AddReference(Object obj)
        {
            allReferences.Add(obj);
        }
    }
}