using System;
using SecretPlan.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(Navigable))]
    public class SecretButton : MonoBehaviour, IPointerClickHandler
    {
        public event Action? Clicked;

        private CachedComponent<Navigable> _navigable = new();
        
        private void Awake()
        {
            _navigable.Get(this).Submitted += OnSubmit;
        }

        private void OnSubmit()
        {
            InvokeClickedOn();
        }

        private void InvokeClickedOn()
        {
            if (_navigable.Get(this).IsInteractive)
            {
                Clicked?.Invoke();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.eligibleForClick)
            {
                InvokeClickedOn();
            }
        }
    }
}
