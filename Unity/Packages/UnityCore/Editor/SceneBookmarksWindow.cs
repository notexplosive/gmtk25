using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SecretPlan.Core.Editor
{
    public class SceneBookmarksWindow : EditorWindow
    {
        private SceneAsset? _requestedScene;

        private void OnGUI()
        {
            HandleRequests();

            var bookmarkConfigName = AssetDatabase.FindAssets($"t: {nameof(SceneBookmarkConfig)}").FirstOrDefault();
            if (bookmarkConfigName == null)
            {
                return;
            }

            var sceneBookmarkConfig =
                AssetDatabase.LoadAssetAtPath<SceneBookmarkConfig>(AssetDatabase.GUIDToAssetPath(bookmarkConfigName));

            if (sceneBookmarkConfig == null)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();

            foreach (var sceneAsset in sceneBookmarkConfig.Scenes)
            {
                if (GUILayout.Button(sceneAsset.name))
                {
                    _requestedScene = sceneAsset;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void HandleRequests()
        {
            if (_requestedScene.IsNotNull())
            {
                if (!Application.isPlaying)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_requestedScene));
                }
                else
                {
                    var address = AddressableUtilities.GetAddress(_requestedScene);
                    if (address != null)
                    {
                        SceneTransition.LoadScene(address);
                    }
                }
            }
            
            _requestedScene = null;
        }

        [MenuItem("SecretPlan/Scene Bookmarks")]
        public static void ShowWindow()
        {
            GetWindow<SceneBookmarksWindow>("Scene Bookmarks");
        }
    }
}