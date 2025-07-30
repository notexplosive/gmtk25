using System.Collections;
using UnityEngine;

namespace SecretPlan.Core
{
    public static class CoroutineSpawner
    {
        public static void Run(string name, bool isLocal, IEnumerator routine)
        {
            var runner = new GameObject($"CoroutineRunner_{name}");
            if (!isLocal)
            {
                Object.DontDestroyOnLoad(runner);
            }

            runner.AddComponent<EphemeralCoroutineRunner>().RunCoroutineAndDestroy(routine);
        }
    }
}