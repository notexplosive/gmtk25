using UnityEngine;

namespace SecretPlan.Core
{
    public static class SpawnUtility
    {
        public static T Spawn<T>(T componentPrefab, InstantiateParameters instantiateParameters) where T : Component
        {
            return Object.Instantiate(componentPrefab, instantiateParameters);
        }

        public static GameObject Spawn(GameObject gameObject, InstantiateParameters instantiateParameters)
        {
            return Object.Instantiate(gameObject, instantiateParameters);
        }
    }
}
