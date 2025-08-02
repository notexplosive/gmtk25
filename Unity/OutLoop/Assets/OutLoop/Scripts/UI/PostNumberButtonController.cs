using System;
using OutLoop.Core;
using SecretPlan.Core;
using SecretPlan.UI;
using UnityEngine;

namespace OutLoop.UI
{
    [RequireComponent(typeof(SecretButton))]
    public class PostNumberButtonController : MonoBehaviour
    {
        [SerializeField]
        private ButtonSkin? _toggledOnSkin;

        private readonly CachedComponent<SecretButton> _button = new();
        private bool _isOn;

        public void Setup(IPost post)
        {
            _button.Get(this).Clicked += DoToggle;
        }

        public void SetToggleState(bool state)
        {
            _isOn = state;
            _button.Get(this).Navigable.Skin = _isOn ? _toggledOnSkin : null;

            Toggled?.Invoke(_isOn);
        }

        private void DoToggle()
        {
            SetToggleState(!_isOn);
        }

        public event Action<bool>? Toggled;
    }
}