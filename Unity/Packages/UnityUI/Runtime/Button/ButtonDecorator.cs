using UnityEngine;

namespace SecretPlan.UI
{
    public abstract class ButtonDecorator : MonoBehaviour
    {
        public abstract void OnState(NavigationState previousState, NavigationState newState, bool isInstant,
            ButtonSkin? skin);
    }
}
