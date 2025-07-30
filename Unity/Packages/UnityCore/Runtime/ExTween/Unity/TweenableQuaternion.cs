using UnityEngine;

namespace ExTween.Unity
{
    public class TweenableQuaternion : Tweenable<Quaternion>
    {
        public TweenableQuaternion(Quaternion initializedValue) : base(initializedValue)
        {
        }

        public TweenableQuaternion(Getter getter, Setter setter) : base(getter, setter)
        {
        }

        public TweenableQuaternion() : this(Quaternion.identity)
        {
        }

        public override Quaternion Lerp(Quaternion startingValue, Quaternion targetValue, float percent)
        {
            return Quaternion.LerpUnclamped(startingValue, targetValue, percent);
        }
    }
}
