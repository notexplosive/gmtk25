using ExTween;
using ExTween.Unity;
using OutLoop.Core;
using OutLoop.Data;
using SecretPlan.UI;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    public class BankController : MonoBehaviour
    {
        [SerializeField]
        private AnswerType _bankType;

        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private TextMeshProUGUI? _titleText;

        [SerializeField]
        private BankTextContent? _bankText;

        [SerializeField]
        private RectTransform? _highlightIndicator;

        [SerializeField]
        private GameObject? _pageNavigationRoot;

        [SerializeField]
        private SecretButton? _previousButton;

        [SerializeField]
        private SecretButton? _nextButton;

        [SerializeField]
        private RectTransform? _animationRoot;
        
        private readonly SequenceTween _tween = new();

        private void Awake()
        {
            if (_bankText != null)
            {
                _bankText.Setup(_bankType);
            }

            if (_highlightIndicator != null)
            {
                _highlightIndicator.gameObject.SetActive(false);
            }

            if (_titleText != null)
            {
                _titleText.text = GetTitleForBankTypeIncludingCount();
            }

            if (_relay != null)
            {
                _relay.State().WordAddedToBank += OnAddedToBank;

                if (_relay.State().GetWordsFromBank(_bankType).Count == 0)
                {
                    gameObject.SetActive(false);
                }

                _relay.State().PendingBlankSet += OnPendingBlankSet;
                _relay.State().BlankFilled += OnBlankFilled;

                if (_previousButton != null)
                {
                    _previousButton.Clicked += () => PageOver(-1);
                }

                if (_nextButton != null)
                {
                    _nextButton.Clicked += () => PageOver(1);
                }
            }

            if (_pageNavigationRoot != null)
            {
                _pageNavigationRoot.SetActive(false);
            }
        }

        private void OnBlankFilled(Puzzle obj, AnswerType answerType)
        {
            if (answerType != _bankType)
            {
                return;
            }

            var smallBeat = 0.07f;
            _tween.Clear();
            _tween
                .Add(new SequenceTween()
                    .Add(transform.GetTweenableLocalScale()
                        .TweenTo(new Vector3(0.7f, 0.7f, 1f), smallBeat, Ease.QuadFastSlow))
                    .Add(transform.GetTweenableLocalScale()
                        .TweenTo(new Vector3(1.3f, 1.3f, 1f), smallBeat, Ease.QuadFastSlow))
                    .Add(transform.GetTweenableLocalScale()
                        .TweenTo(new Vector3(0.9f, 0.9f, 1f), smallBeat, Ease.QuadFastSlow))
                    .Add(transform.GetTweenableLocalScale()
                        .TweenTo(new Vector3(1.1f, 1.1f, 1f), smallBeat, Ease.QuadFastSlow))
                    .Add(transform.GetTweenableLocalScale()
                        .TweenTo(new Vector3(1f, 1f, 1f), smallBeat * 2, Ease.QuadFastSlow))
                );
        }

        private void Update()
        {
            _tween.Update(Time.deltaTime);
        }

        private void PageOver(int offset)
        {
            if (_relay == null)
            {
                return;
            }

            if (_bankText == null)
            {
                return;
            }

            var state = _relay.State();

            var desiredPageNumber = _bankText.PageNumber + offset;
            if (desiredPageNumber < 0)
            {
                return;
            }
            
            var startingIndexOnDesiredPage = BankTextContent.PageSize * desiredPageNumber;

            if (startingIndexOnDesiredPage >= state.FoundBankCount(_bankType))
            {
                return;
            }
            
            _bankText.PageNumber = desiredPageNumber;
        }

        private void OnPendingBlankSet(PuzzleBlank? puzzleBlank)
        {
            if (_highlightIndicator == null)
            {
                return;
            }

            if (puzzleBlank == null)
            {
                _highlightIndicator.gameObject.SetActive(false);
                return;
            }

            if (puzzleBlank.AnswerType == _bankType)
            {
                BumpHighlightIndicator();
            }
            else
            {
                _highlightIndicator.gameObject.SetActive(false);
            }
        }

        private void BumpHighlightIndicator()
        {
            if (_highlightIndicator == null)
            {
                return;
            }

            _highlightIndicator.gameObject.SetActive(true);

            var scale = _highlightIndicator.transform.GetTweenableLocalScale();
            _tween
                .Add(scale.CallbackSetTo(new Vector3(1f, 1f, 1f)))
                .Add(scale.TweenTo(new Vector3(1.2f, 1.2f, 1.2f), 0.15f, Ease.QuadFastSlow))
                .Add(scale.TweenTo(new Vector3(1f, 1f, 1f), 0.15f, Ease.QuadSlowFast))
                .Add(scale.TweenTo(new Vector3(1.1f, 1.1f, 1.1f), 0.10f, Ease.QuadFastSlow))
                .Add(scale.TweenTo(new Vector3(1f, 1f, 1f), 0.10f, Ease.QuadSlowFast))
                ;
        }

        private void OnAddedToBank(AnswerType answerType, string word)
        {
            if (_titleText != null)
            {
                // update count
                _titleText.text = GetTitleForBankTypeIncludingCount();
            }

            if (answerType != _bankType)
            {
                return;
            }

            if (_bankText == null)
            {
                return;
            }

            gameObject.SetActive(true);

            _bankText.UpdateText();
            
            if (_relay != null)
            {
                if (_relay.State().FoundBankCount(_bankType) > BankTextContent.PageSize)
                {
                    if (_pageNavigationRoot != null)
                    {
                        _pageNavigationRoot.SetActive(true);
                    }
                }
            }
            
            _tween.Clear();
            var smallBeat = 0.05f;
            var medBeat = 0.12f;
            _tween
                .Add(
                    new MultiplexTween()
                        .Add(new SequenceTween()
                            .Add(transform.GetTweenableLocalScale().TweenTo(new Vector3(1.3f, 1.3f, 1f), smallBeat, Ease.QuadFastSlow))
                            .Add(transform.GetTweenableLocalScale().TweenTo(new Vector3(0.7f, 0.7f, 1f), smallBeat, Ease.QuadFastSlow))
                            .Add(transform.GetTweenableLocalScale().TweenTo(new Vector3(1.2f, 1.2f, 1f), smallBeat, Ease.QuadFastSlow))
                            .Add(transform.GetTweenableLocalScale().TweenTo(new Vector3(0.9f, 0.9f, 1f), smallBeat, Ease.QuadFastSlow))
                            .Add(transform.GetTweenableLocalScale().TweenTo(new Vector3(1.1f, 1.1f, 1f), smallBeat, Ease.QuadFastSlow))
                            .Add(transform.GetTweenableLocalScale().TweenTo(new Vector3(1f, 1f, 1f), smallBeat*2, Ease.QuadFastSlow))
                        )
                        .Add(
                            new SequenceTween()
                                .Add(transform.GetTweenableRotation().TweenTo(Quaternion.Euler(0,0,-15), medBeat, Ease.Linear))
                                .Add(transform.GetTweenableRotation().TweenTo(Quaternion.Euler(0,0,15), medBeat, Ease.Linear))
                                .Add(transform.GetTweenableRotation().TweenTo(Quaternion.Euler(0,0,-5), medBeat, Ease.Linear))
                                .Add(transform.GetTweenableRotation().TweenTo(Quaternion.Euler(0,0,5), medBeat, Ease.Linear))
                                .Add(transform.GetTweenableRotation().TweenTo(Quaternion.Euler(0,0,0), medBeat, Ease.Linear))
                            )
                    )
                
                ;
        }

        private string GetTitleForBankTypeIncludingCount()
        {
            if (_relay == null)
            {
                return "???";
            }

            if (_bankType == AnswerType.Hashtag)
            {
                return
                    $"#tags ({_relay.State().FoundBankCount(AnswerType.Hashtag)}/{_relay.State().TotalBankCount(AnswerType.Hashtag)})";
            }

            if (_bankType == AnswerType.Username)
            {
                return
                    $"@users ({_relay.State().FoundBankCount(AnswerType.Username)}/{_relay.State().TotalBankCount(AnswerType.Username)})";
            }

            return "???";
        }
    }
}