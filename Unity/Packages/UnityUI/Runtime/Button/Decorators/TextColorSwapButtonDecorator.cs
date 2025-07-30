using System;
using ExTween;
using ExTween.Unity;
using JetBrains.Annotations;
using NaughtyAttributes;
using SecretPlan.Core;
using TMPro;
using UnityEngine;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextColorSwapButtonDecorator : ButtonDecoratorPerStateWithTween<Color>
    {
        private readonly CachedComponent<TMP_Text> _image = new();

        protected override Color GetCurrentValue()
        {
            return _image.Get(this).color;
        }

        protected override void SetCurrentValue(Color value)
        {
            _image.Get(this).color = value;
        }

        protected override Color GetTargetValue(Color defaultTargetValue)
        {
            return defaultTargetValue;
        }

        protected override Tweenable<Color> MakeTweenable(Tweenable<Color>.Getter get, Tweenable<Color>.Setter set)
        {
            return new TweenableColor(get, set);
        }

        [UsedImplicitly]
        [Button]
        public void SetAllAlphasTo1()
        {
            foreach (var state in Enum.GetValues(typeof(NavigationState)))
            {
                var realState = (NavigationState) state;
                var existingState = DataPerState.GetDataFromState(realState);
                var newColor = existingState.Value;
                newColor.a = 1f;
                existingState.Value = newColor;
                DataPerState.SetDataForState(realState, existingState);
            }
        }
    }
}
