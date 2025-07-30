using System;

namespace SecretPlan.UI
{
    public class PopupState
    {
        public event Action<Popup>? PopupRequested;
        
        public void RequestPopup(Popup prefab)
        {
            PopupRequested?.Invoke(prefab);
        }
    }
}
