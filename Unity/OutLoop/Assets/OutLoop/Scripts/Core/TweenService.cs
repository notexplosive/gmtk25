using System.Collections.Generic;
using ExTween;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.Core
{
    public class TweenService : GlobalService<TweenService>
    {
        private readonly Dictionary<string, SequenceTween> _tweenByChannelName = new();

        public override void OnUpdate()
        {
            foreach (var tween in _tweenByChannelName.Values)
            {
                tween.Update(Time.deltaTime);

                if (tween.IsDone())
                {
                    tween.Clear();
                }
            }
        }

        public void ClearChannelAndSet(string channelName, ITween tweenToAdd)
        {
            var tween = _tweenByChannelName.GetValueOrDefault(channelName);

            if (tween == null)
            {
                tween = new SequenceTween();
                _tweenByChannelName.Add(channelName, tween);
            }
            
            tween.Clear();
            tween.Add(tweenToAdd);
        }
    }
}