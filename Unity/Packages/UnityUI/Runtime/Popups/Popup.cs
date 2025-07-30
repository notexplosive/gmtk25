using System;
using SecretPlan.Core;
using UnityEngine;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(NavigationLayer))]
    public class Popup : MonoBehaviour
    {
        [SerializeField]
        private GameObject? _scrim;

        private readonly CachedComponent<CanvasGroup> _canvasGroup = new();
        private readonly CachedComponent<NavigationLayer> _navigationLayer = new();

        public GameObject? ScrimPrefab => _scrim;

        public event Action? RequestClose;

        public void Close()
        {
            RequestClose?.Invoke();
        }

        public void SetInteractive(bool state)
        {
            _canvasGroup.Get(this).interactable = state;
        }
    }
}
