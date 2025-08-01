using System.Linq;
using ExTween;
using OutLoop.Core;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class UnreadMessagesFlasher : MonoBehaviour
    {
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private Image? _flasher;

        private readonly SequenceTween _tween = new();

        public TweenableFloat TweenableOpacity
        {
            get
            {
                if (_flasher == null)
                {
                    return new TweenableFloat();
                }

                return new TweenableFloat(() => _flasher.color.a, a => _flasher.color = _flasher.color with { a = a });
            }
        }

        private void Awake()
        {
            if (_relay == null)
            {
                return;
            }

            var loopData = _relay.State();

            loopData.MessageReceived += OnReceive;
            loopData.MessageMarkedAsRead += OnRead;

            UpdateState();
        }

        private void Update()
        {
            _tween.Update(Time.deltaTime);
        }

        private void OnRead(DirectMessage obj)
        {
            UpdateState();
        }

        private void OnReceive(DirectMessage obj)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (_relay == null)
            {
                return;
            }

            var loopData = _relay.State();
            var unreadMessageCount = loopData.AllMessages().Count(a => !loopData.IsMessageRead(a));
            if (unreadMessageCount > 0)
            {
                _tween.Clear();
                _tween.Add(TweenableOpacity.TweenTo(1, 0.5f, Ease.CubicFastSlow));
                _tween.Add(TweenableOpacity.TweenTo(0, 0.5f, Ease.CubicFastSlow));
                _tween.SetLooping(true);
            }
            else
            {
                _tween.Clear();
                _tween.Add(TweenableOpacity.TweenTo(0, 0.25f, Ease.CubicFastSlow));
            }
        }
    }
}