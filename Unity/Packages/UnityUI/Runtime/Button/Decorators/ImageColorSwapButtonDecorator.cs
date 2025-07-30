using System;
using ExTween;
using ExTween.Unity;
using JetBrains.Annotations;
using SecretPlan.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageColorSwapButtonDecorator : ButtonDecoratorPerStateWithTween<Color>
    {
        private readonly CachedComponent<Image> _image = new();

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
        [NaughtyAttributes.Button]
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
