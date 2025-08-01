using System;
using ExTween;
using ExTween.Unity;
using OutLoop.Core;
using SecretPlan.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutLoop.UI
{
    public class AppPage : MonoBehaviour
    {
        private readonly CachedComponent<RectTransform> _rectTransform = new();

        private readonly float _pageOverSpeed = 0.15f;
        private readonly Ease.Delegate _easeFunction = Ease.QuadFastSlow;

        [SerializeField]
        private PageType _pageType;

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
    }
}