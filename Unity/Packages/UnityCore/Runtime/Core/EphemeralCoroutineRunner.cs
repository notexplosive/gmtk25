using System.Collections;
using UnityEngine;

namespace SecretPlan.Core
{
    public class EphemeralCoroutineRunner : MonoBehaviour
    {
        public void RunCoroutineAndDestroy(IEnumerator routine)
        {
            StartCoroutine(RoutineWrapper(routine));
        }

        private IEnumerator RoutineWrapper(IEnumerator routine)
        {
            yield return routine;
            Destroy(gameObject);
        }
    }
}