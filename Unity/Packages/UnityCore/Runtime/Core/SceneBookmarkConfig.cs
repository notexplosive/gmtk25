using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SecretPlan.Core
{
    [CreateAssetMenu(menuName = "SecretPlan/SceneBookmarkConfig", fileName = "SceneBookmarkConfig")]
    public class SceneBookmarkConfig : ScriptableObject
    {
        #if UNITY_EDITOR
        [SerializeField]
        private List<SceneAsset> _scenes = new();
        
        public List<SceneAsset> Scenes => _scenes;
        #endif
    }
}