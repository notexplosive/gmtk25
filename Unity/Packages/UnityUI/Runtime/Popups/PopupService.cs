using SecretPlan.Core;
using UnityEngine;

namespace SecretPlan.UI
{
    [CreateAssetMenu(menuName = "SecretPlan/UI/PopupRelay", fileName = "PopupRelay")]
    public class PopupRelay : ScriptableRelay<PopupState> 
    {
        protected override PopupState CreateState()
        {
            return new PopupState();
        }
    }
}
