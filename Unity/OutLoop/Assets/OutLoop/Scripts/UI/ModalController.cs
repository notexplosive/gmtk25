using System;
using SecretPlan.UI;
using UnityEngine;

namespace OutLoop.UI
{
    public abstract class ModalController : MonoBehaviour
    {
        [SerializeField]
        private SecretButton? _closeButton;
        
        protected AppPage? OwningPage { get; private set; }

        protected void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.Clicked += Close;
            }
        }

        private void Close()
        {
            if (OwningPage != null)
            {
                OwningPage.CloseTopModal();
            }
        }

        public void SetOwningPage(AppPage owningPage)
        {
            OwningPage = owningPage;
        }
    }
}