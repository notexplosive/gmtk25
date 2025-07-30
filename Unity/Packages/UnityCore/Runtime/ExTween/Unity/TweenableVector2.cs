using UnityEngine;

namespace ExTween.Unity
{
    public class TweenableVector2 : Tweenable<Vector2>
    {
        public TweenableVector2(Vector2 initializedValue) : base(initializedValue)
        {
        }

        public TweenableVector2(Getter getter, Setter setter) : base(getter, setter)
        {
        }

        public TweenableVector2() : this(Vector2.zero)
        {
        }

        public override Vector2 Lerp(Vector2 startingValue, Vector2 targetValue, float percent)
        {
            return Vector2.LerpUnclamped(startingValue, targetValue, percent);
        }
    }
}