using System.Linq;
using ExTween;
using ExTween.Unity;
using OutLoop.Core;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    public class NewMessageBadge : MonoBehaviour
    {
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private GameObject? _root;

        [SerializeField]
        private TMP_Text? _number;

        private readonly SequenceTween _tween = new();

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

        private void DoBumpAnimation()
        {
            _tween.Clear();

            if (_root == null)
            {
                return;
            }

            _tween.Add(_root.transform.GetTweenableLocalPositionY().TweenTo(20, 0.15f, Ease.QuadFastSlow));
            _tween.Add(_root.transform.GetTweenableLocalPositionY().TweenTo(0, 0.15f, Ease.QuadSlowFast));
            _tween.Add(_root.transform.GetTweenableLocalPositionY().TweenTo(10, 0.1f, Ease.QuadFastSlow));
            _tween.Add(_root.transform.GetTweenableLocalPositionY().TweenTo(0, 0.1f, Ease.QuadSlowFast));
            _tween.Add(_root.transform.GetTweenableLocalPositionY().TweenTo(5, 0.05f, Ease.QuadFastSlow));
            _tween.Add(_root.transform.GetTweenableLocalPositionY().TweenTo(0, 0.05f, Ease.QuadSlowFast));
        }

        private void OnReceive(DirectMessage obj)
        {
            UpdateState();
            DoBumpAnimation();
        }

        private void UpdateState()
        {
            if (_relay != null)
            {
                var loopData = _relay.State();
                var unreadMessageCount = loopData.AllMessages().Count(a => !loopData.IsMessageRead(a));
                if (unreadMessageCount > 0)
                {
                    if (_number != null)
                    {
                        _number.text = unreadMessageCount.ToString();
                    }

                    if (_root != null)
                    {
                        _root.SetActive(true);
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        _root.SetActive(false);
                    }
                }
            }
        }
    }
}