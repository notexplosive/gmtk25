using ExTween;
using ExTween.Unity;
using OutLoop.Core;
using OutLoop.Data;
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

        private readonly SequenceTween _tween = new();

        private void Awake()
        {
            if (_highlightIndicator != null)
            {
                _highlightIndicator.gameObject.SetActive(false);
            }

            if (_titleText != null)
            {
                _titleText.text = GetTitleForBankType();
            }

            if (_relay != null)
            {
                _relay.State().WordAddedToBank += AppendToBank;

                if (_relay.State().GetWordsFromBank(_bankType).Count == 0)
                {
                    gameObject.SetActive(false);
                }

                _relay.State().PendingBlankSet += OnPendingBlankSet;
            }
        }

        private void Update()
        {
            _tween.Update(Time.deltaTime);
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

        private void AppendToBank(AnswerType answerType, string word)
        {
            if (_titleText != null)
            {
                _titleText.text = GetTitleForBankType();
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
            _bankText.Add(_bankType, word);
        }

        private string GetTitleForBankType()
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