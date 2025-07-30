using System;
using SecretPlan.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(Image))]
    public class UIHitBox : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        private CachedComponent<Image> _image = new();

        private Navigable? _navigable;

        private void Awake()
        {
            var image = _image.Get(this);
            image.alphaHitTestMinimumThreshold = 0.5f;
            image.color = new Color(0, 0, 0, 0);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_navigable == null)
            {
                return;
            }

            _navigable.ReceivePointerDown(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_navigable == null)
            {
                return;
            }

            _navigable.ReceivePointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_navigable == null)
            {
                return;
            }

            _navigable.ReceivePointerExit();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_navigable == null)
            {
                return;
            }

            _navigable.ReceivePointerUp(eventData);
        }

        public void Initialize(Navigable navigable)
        {
            _navigable = navigable;
        }
    }
}
