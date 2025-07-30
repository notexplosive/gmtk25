using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SecretPlan.Core
{
    [CreateAssetMenu(menuName = "SecretPlan/SceneBookmarkConfig", fileName = "SceneBookmarkConfig")]
    public class SceneBookmarkConfig : ScriptableObject
    {
        [SerializeField]
        private List<SceneAsset> _scenes = new();
        
        public List<SceneAsset> Scenes => _scenes;
    }
}