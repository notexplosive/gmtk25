using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace SecretPlan.Core
{
    public static class SceneTransition
    {
        private static readonly List<AsyncOperationHandle<SceneInstance>> _loadedScenes = new();
        private static readonly FloatProvider _loadingPercentProvider = new();
        private static bool _loadRequested;

        public static float PercentLoaded => _loadingPercentProvider.Get();

        public static void LoadScene(string targetSceneAddress)
        {
            CoroutineSpawner.Run("Load Scene", false, LoadSceneRoutine(targetSceneAddress));
        }

        private static IEnumerator LoadSceneRoutine(string targetSceneAddress)
        {
            if (_loadRequested)
            {
                yield break;
            }
            
            _loadRequested = true;
            _loadingPercentProvider.SetFlatValue(0);
            var loadingScene = Addressables.LoadSceneAsync(MagicAddress.LoadingScreen, LoadSceneMode.Additive);
            yield return AsyncHandleUtilities.WaitUntilComplete(loadingScene);
            var currentActiveScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(loadingScene.Result.Scene);

            yield return UnloadCurrentScenes(currentActiveScene);

            var newMainSceneHandle = Addressables.LoadSceneAsync(targetSceneAddress, LoadSceneMode.Additive);
            _loadedScenes.Add(newMainSceneHandle);
            _loadingPercentProvider.SetProvider(() => loadingScene.PercentComplete);

            yield return AsyncHandleUtilities.WaitUntilComplete(newMainSceneHandle);
            SceneManager.SetActiveScene(newMainSceneHandle.Result.Scene);
            _loadingPercentProvider.SetFlatValue(1);

            Addressables.UnloadSceneAsync(loadingScene);
            _loadRequested = false;
        }

        private static IEnumerator UnloadCurrentScenes(Scene currentActiveScene)
        {
            if (_loadedScenes.Count == 0)
            {
                yield return SceneManager.UnloadSceneAsync(currentActiveScene);
            }

            yield return AsyncHandleUtilities.WaitUntilCompleteMany(_loadedScenes
                .Select(scene => Addressables.UnloadSceneAsync(scene)).ToList());
        }
    }
}