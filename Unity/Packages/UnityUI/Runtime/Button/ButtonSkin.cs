using SecretPlan.Core;
using UnityEngine;

namespace SecretPlan.UI
{
    [CreateAssetMenu(fileName = "ButtonSkin", menuName = "SecretPlan/UI/ButtonSkin")]
    public class ButtonSkin : ScriptableObject
    {
        private void OnEnable()
        {
            PersistentReference.AddReference(this);
        }
    }
}
