using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace SecretPlan.Core.Editor
{
    public static class AddressableUtilities
    {
        public static string? GetAddress(Object? asset)
        {
            if (asset.IsNull())
            {
                return null;
            }
            
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings.IsNull())
            {
                return null;
            }

            var entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));
            return entry?.address;
        }
    }
}