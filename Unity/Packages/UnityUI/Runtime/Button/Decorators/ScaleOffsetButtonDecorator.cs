using ExTween;
using ExTween.Unity;
using UnityEngine;

namespace SecretPlan.UI
{
    public class ScaleOffsetButtonDecorator : ButtonDecoratorPerStateWithTween<Vector2>
    {
        protected override Vector2 GetCurrentValue()
        {
            return transform.localScale;
        }

        protected override void SetCurrentValue(Vector2 value)
        {
            transform.localScale = value;
        }

        protected override Vector2 GetTargetValue(Vector2 defaultTargetValue)
        {
            return defaultTargetValue;
        }

        protected override Tweenable<Vector2> MakeTweenable(Tweenable<Vector2>.Getter get, Tweenable<Vector2>.Setter set)
        {
            return new TweenableVector2(get, set);
        }
    }
}
