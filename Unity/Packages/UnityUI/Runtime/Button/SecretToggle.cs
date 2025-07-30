using NaughtyAttributes;
using SecretPlan.Core;
using UnityEngine;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(SecretButton))]
    [RequireComponent(typeof(Navigable))]
    public class SecretToggle : MonoBehaviour
    {
        [SerializeField]
        private ButtonSkin? _checkedSkin;

        [SerializeField]
        private ButtonSkin? _uncheckedSkin;

        private readonly CachedComponent<Navigable> _navigable = new();
        private readonly CachedComponent<SecretButton> _secretButton = new();
        public WrappedValue<bool> CheckedState { get; } = new(false);

        [ShowNativeProperty]
        public bool IsChecked => CheckedState.Value;

        private void Awake()
        {
            CheckedState.ValueChanged += OnValueChanged;
        }

        private void OnEnable()
        {
            _secretButton.Get(this).Clicked += OnClick;
        }

        private void OnDisable()
        {
            _secretButton.Get(this).Clicked -= OnClick;
        }

        private void OnClick()
        {
            Toggle();
        }

        private void Toggle()
        {
            CheckedState.Value = !CheckedState.Value;
        }

        private void OnValueChanged(bool value)
        {
            _navigable.Get(this).Skin = GetSkinForCheckedState(value);
        }

        private ButtonSkin? GetSkinForCheckedState(bool value)
        {
            return value ? _checkedSkin : _uncheckedSkin;
        }
    }
}
