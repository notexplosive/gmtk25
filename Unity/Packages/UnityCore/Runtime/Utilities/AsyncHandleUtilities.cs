using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SecretPlan.Core
{
    public static class AsyncHandleUtilities
    {
        public static IEnumerator WaitUntilComplete<T>(AsyncOperationHandle<T> handle)
        {
            while (!handle.IsDone)
            {
                yield return null;
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("WaitUntilComplete failed");
            }
        }

        public static IEnumerator WaitUntilCompleteMany<T>(List<AsyncOperationHandle<T>> handlesToAwait)
        {
            var isDone = false;
            while (!isDone)
            {
                isDone = true;
                if (handlesToAwait.Any(handle => !handle.IsDone))
                {
                    isDone = false;
                }

                if (!isDone)
                {
                    yield return null;
                }
            }
        }
    }
}