using ExTween;
using ExTween.Unity;
using NaughtyAttributes;
using SecretPlan.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class IntroSequence : MonoBehaviour
    {
        [SerializeField]
        private AudioClip? _startupSound;
        
        [SerializeField]
        private RectTransform? _root;

        [SerializeField]
        private TextMeshProUGUI? _splashScreenText;

        [SerializeField]
        private Image? _logoImage;

        [SerializeField]
        private CanvasGroup? _splashScreenCanvasGroup;

        [SerializeField]
        private Transform? _splashScreenRoot;

        [SerializeField]
        private AudioSource? _musicPlayer;

        private readonly SequenceTween _tween = new();

        private void Awake()
        {
            RunIntro();
        }

        private void Update()
        {
            _tween.Update(Time.deltaTime);
            if (_tween.IsDone())
            {
                _tween.Clear();
            }
        }

        [Button]
        public void RunIntro()
        {
            if (_root == null)
            {
                return;
            }

            if (_logoImage == null)
            {
                return;
            }

            if (_splashScreenCanvasGroup == null)
            {
                return;
            }

            if (_splashScreenText == null)
            {
                return;
            }

            if (_splashScreenRoot == null)
            {
                return;
            }

            if (_musicPlayer != null)
            {
                _musicPlayer.Stop();
            }

            _tween.Clear();
            var tweenableY = _root.GetTweenableLocalPositionY();
            var rootStartingPosition = tweenableY.Value;

            var imageOpacity =
                new TweenableFloat(() => _logoImage.color.a, a => _logoImage.color = new Color(1, 1, 1, a));
            var imageScale = _logoImage.transform.GetTweenableLocalScale();
            var textOpacity = new
                TweenableFloat(() => _splashScreenText.color.a,
                    a => _splashScreenText.color = _splashScreenText.color with { a = a });
            var textStartPosition = _splashScreenText.transform.localPosition.x;

            var textPositionX = _splashScreenText.transform.GetTweenableLocalPositionX();

            var splashScreenOpacity = new TweenableFloat(() => _splashScreenCanvasGroup.alpha,
                a => _splashScreenCanvasGroup.alpha = a);
            _splashScreenRoot.gameObject.SetActive(true);

            var beatLength = 1f;

            _tween
                .Add(tweenableY.CallbackSetTo(rootStartingPosition - _root.rect.height * 2))
                .Add(imageOpacity.CallbackSetTo(0))
                .Add(textOpacity.CallbackSetTo(0))
                .Add(tweenableY.TweenTo(rootStartingPosition, 0.5f, Ease.QuadFastSlow))
                .Add(imageScale.CallbackSetTo(new Vector3(2f, 2f, 1)))
                .Add(textPositionX.CallbackSetTo(textStartPosition + 800))
                .Add(new CallbackTween(() =>
                {
                    SoundService.Instance.PlaySound(_startupSound);
                }))
                .Add(new MultiplexTween()
                    .Add(new SequenceTween()
                        .Add(imageScale.TweenTo(new Vector3(0.8f, 0.8f, 1), beatLength / 2f, Ease.QuadFastSlow))
                        .Add(imageScale.TweenTo(new Vector3(1.1f, 1.1f, 1), beatLength / 2f, Ease.QuadSlowFast))
                        .Add(imageScale.TweenTo(new Vector3(1f, 1f, 1), beatLength / 2f, Ease.QuadFastSlow))
                    )
                    .Add(new SequenceTween()
                        .Add(new WaitSecondsTween(beatLength * 0.3f))
                        .Add(imageOpacity.TweenTo(1f, beatLength * 0.7f, Ease.Linear))
                    )
                )
                .Add(
                    new MultiplexTween()
                        .Add(textOpacity.TweenTo(1f, beatLength/2, Ease.Linear))
                        .Add(textPositionX.TweenTo(textStartPosition, beatLength, Ease.QuadFastSlow))
                )
                .Add(new WaitSecondsTween(beatLength / 2f))
                .Add(new CallbackTween(() =>
                {
                    if (_musicPlayer != null)
                    {
                        _musicPlayer.Play();
                    }
                }))
                .Add(new MultiplexTween()
                    .Add(splashScreenOpacity.TweenTo(0, beatLength / 2f, Ease.Linear))
                    .Add(imageScale.TweenTo(new Vector3(2, 2, 2), beatLength, Ease.QuadSlowFast))
                )
                .Add(new CallbackTween(() => { _splashScreenRoot.gameObject.SetActive(false); }))
                ;
        }
    }
}