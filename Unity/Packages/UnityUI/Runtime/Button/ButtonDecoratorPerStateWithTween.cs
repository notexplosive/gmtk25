using ExTween;
using UnityEngine;

namespace SecretPlan.UI
{
    public abstract class ButtonDecoratorPerStateWithTween<TData> : ButtonDecoratorPerState<StateWithDuration<TData>>
        where TData : unmanaged
    {
        private readonly SequenceTween _tween = new();

        protected virtual void Update()
        {
            if (_tween.IsDone())
            {
                _tween.Clear();
            }
            
            _tween.Update(Time.deltaTime);
        }

        protected abstract TData GetCurrentValue();
        protected abstract void SetCurrentValue(TData value);

        public override void OnState(NavigationState previousState, NavigationState newState, bool isInstant,
            ButtonSkin? skin)
        {
            var duration = DataPerState.GetDataFromState(newState).Duration;
            var targetValue = GetTargetValue(DataPerState.GetDataFromState(newState).Value);
            
            if (newState == NavigationState.Pressed)
            {
                _tween.Clear();
            }

            else if (previousState == NavigationState.Normal && newState == NavigationState.NavigatedTo)
            {
                _tween.Clear();
            }
            
            else if (previousState == NavigationState.NavigatedTo && newState == NavigationState.Normal)
            {
                _tween.Clear();
            }

            else if (newState == NavigationState.Disabled || previousState == NavigationState.Disabled)
            {
                _tween.Clear();
            }

            if (isInstant)
            {
                SetCurrentValue(targetValue);
            }
            else
            {
                var tweenable = MakeTweenable(GetCurrentValue, SetCurrentValue);
                _tween.Add(tweenable.TweenTo(targetValue, duration, Ease.Linear));
            }
        }

        protected abstract TData GetTargetValue(TData defaultTargetValue);


        /// <summary>
        ///     Inheritors are expected to call `new TweenableXyz(get,set)`. If the Get() and Set() methods are populated as
        ///     expected, this should just work.
        /// </summary>
        protected abstract Tweenable<TData> MakeTweenable(Tweenable<TData>.Getter get, Tweenable<TData>.Setter set);
    }
}
