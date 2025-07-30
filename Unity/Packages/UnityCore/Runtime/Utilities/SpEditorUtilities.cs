using UnityEngine;

namespace SecretPlan.Core
{
    public static class SpEditorUtilities
    {
        public static bool IsRootOfPrefab(Transform transform)
        {
            return transform.parent == null || transform.parent.name == "Prefab Mode in Context" || transform.parent.name == "Canvas (Environment)";
        }
    }
}
