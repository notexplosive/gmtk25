using System;
using System.Collections.Generic;
using ExTween;
using ExTween.Unity;
using OutLoop.Core;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class AppPage : MonoBehaviour
    {
        [SerializeField]
        private PageType _pageType;

        private readonly Ease.Delegate _easeFunction = Ease.QuadFastSlow;

        private readonly float _pageOverSpeed = 0.15f;
        private readonly CachedComponent<RectTransform> _rectTransform = new();

        private readonly Stack<ModalController> _openModals = new();

        public PageType PageType => _pageType;

        public void FlyOut(int direction)
        {
            TweenService.Instance.ClearChannelAndSet("AppPagingOut", new SequenceTween()
                .Add(transform.GetTweenableLocalPositionX()
                    .TweenTo(OutsideXPosition() * direction * -1, _pageOverSpeed, _easeFunction))
                .Add(new CallbackTween(Hide))
            );
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private float OutsideXPosition()
        {
            return _rectTransform.Get(this).rect.width;
        }

        public void FlyIn(int direction)
        {
            
            TweenService.Instance.ClearChannelAndSet("AppPagingIn", new SequenceTween()
                .Add(new CallbackTween(Show))
                .Add(transform.GetTweenableLocalPositionX().CallbackSetTo(OutsideXPosition() * direction))
                .Add(transform.GetTweenableLocalPositionY().CallbackSetTo(0))
                .Add(transform.GetTweenableLocalPositionX()
                    .TweenTo(0, _pageOverSpeed, _easeFunction))
            );
        }

        private void Show()
        {
            gameObject.SetActive(true);
            Selected?.Invoke();
        }

        public event Action? Selected;

        public T? OpenModal<T>(T? prefab) where T : ModalController
        {
            if (prefab == null)
            {
                return null;
            }
            
            var instance = SpawnUtility.Spawn(prefab, new InstantiateParameters { parent = transform });
            instance.SetOwningPage(this);
            _openModals.Push(instance);
            return instance;
        }

        public void CloseTopModal()
        {
            if (_openModals.Count == 0)
            {
                return;
            }
            
            var topModal = _openModals.Pop();
            Destroy(topModal.gameObject);
        }

        public ModalController? GetTopModal()
        {
            if (_openModals.Count > 0)
            {
                var topModal = _openModals.Peek();
                return topModal;
            }

            return null;
        }
    }
}