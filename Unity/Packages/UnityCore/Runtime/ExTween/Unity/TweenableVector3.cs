using UnityEngine;

namespace ExTween.Unity
{
    public class TweenableVector3 : Tweenable<Vector3>
    {
        public TweenableVector3(Vector3 initializedValue) : base(initializedValue)
        {
        }

        public TweenableVector3(Getter getter, Setter setter) : base(getter, setter)
        {
        }

        public TweenableVector3() : this(Vector3.zero)
        {
        }

        public override Vector3 Lerp(Vector3 startingValue, Vector3 targetValue, float percent)
        {
            return Vector3.LerpUnclamped(startingValue, targetValue, percent);
        }
    }
}
