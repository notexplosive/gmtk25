using OutLoop.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class RibbonButton : MonoBehaviour
    {
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private PageType _pageType;

        [SerializeField]
        private GameObject? _activeIndicator;

        private void Awake()
        {
            if (_relay != null)
            {
                _relay.State().PageUpdated += OnPageUpdate;
                OnPageUpdate(_relay.State().CurrentPage);
            }
            
        }

        private void OnPageUpdate(PageType pageType)
        {
            if (_activeIndicator != null)
            {
                _activeIndicator.SetActive(pageType == _pageType);
            }
        }
    }
}