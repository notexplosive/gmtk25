using ExTween;
using ExTween.Unity;
using UnityEngine;

namespace SecretPlan.UI
{
    public class PositionOffsetButtonDecorator : ButtonDecoratorPerStateWithTween<Vector2>
    {
        private Vector2 _startingValue;

        private void Awake()
        {
            _startingValue = transform.localPosition;
        }

        protected override Vector2 GetCurrentValue()
        {
            return transform.localPosition;
        }

        protected override void SetCurrentValue(Vector2 value)
        {
            transform.localPosition = value;
        }

        protected override Vector2 GetTargetValue(Vector2 defaultTargetValue)
        {
            return defaultTargetValue + _startingValue;
        }

        protected override Tweenable<Vector2> MakeTweenable(Tweenable<Vector2>.Getter get,
            Tweenable<Vector2>.Setter set)
        {
            return new TweenableVector2(get, set);
        }
    }
}
