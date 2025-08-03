using System;
using OutLoop.Core;
using SecretPlan.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutLoop.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PuzzlePhraseTextController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private RectTransform? _highlight;

        private readonly CachedComponent<TMP_Text> _textMesh = new();

        private Puzzle? _puzzle;

        private void OnDestroy()
        {
            if (_relay != null)
            {
                _relay.State().BlankFilled -= OnBlankFilled;
                _relay.State().PendingBlankSet -= OnPendingBlankSet;
                _relay.State().ClearPendingBlank();
            }
        }

        private void OnPendingBlankSet(PuzzleBlank? obj)
        {
            if (obj == null)
            {
                ClearHighlight();
            }
        }

        private void ClearHighlight()
        {
            if (_highlight != null)
            {
                _highlight.gameObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_puzzle == null)
            {
                return;
            }

            if (_relay == null)
            {
                return;
            }

            if (_puzzle.IsSolved())
            {
                return;
            }

            var textMeshPro = _textMesh.Get(this);
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, null);
            if (linkIndex != -1)
            {
                var linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];

                var linkText = linkInfo.GetLinkID();
                if (int.TryParse(linkText, out var result))
                {
                    _relay.State().SetPendingBlank(_puzzle.Blanks[result]);
                    UpdateText(result);
                }
            }
        }
        
        private void UpdateHighlight(TMP_LinkInfo linkInfo)
        {
            var textMeshPro = _textMesh.Get(this);

            foreach (var wordInfo in textMeshPro.textInfo.wordInfo)
            {
                if (linkInfo.linkTextfirstCharacterIndex < wordInfo.firstCharacterIndex)
                {
                    var firstCharacterOfWord = textMeshPro.textInfo.characterInfo[wordInfo.firstCharacterIndex - 1];
                    var lastCharacterOfWord = textMeshPro.textInfo.characterInfo[wordInfo.lastCharacterIndex];

                    var lineInfo = GetLineOfWord(wordInfo);

                    var topLeftOfWord = textMeshPro.transform.TransformPoint(firstCharacterOfWord.topLeft);
                    var bottomRightOfWord = textMeshPro.transform.TransformPoint(lastCharacterOfWord.bottomRight);
                    var bottom = bottomRightOfWord.y;
                    var top = topLeftOfWord.y;

                    if (lineInfo.HasValue)
                    {
                        for (var characterIndex = lineInfo.Value.firstCharacterIndex;
                             characterIndex < lineInfo.Value.lastCharacterIndex;
                             characterIndex++)
                        {
                            var characterInfo = textMeshPro.textInfo.characterInfo[characterIndex];
                            bottom = MathF.Min(bottom, textMeshPro.transform.TransformPoint(characterInfo.bottomRight).y);
                            top = MathF.Max(top, textMeshPro.transform.TransformPoint(characterInfo.topLeft).y);
                        }
                    }
                    
                    var width = bottomRightOfWord.x - topLeftOfWord.x;
                    var rect = new Rect(topLeftOfWord.x, bottom, width, top - bottom);

                    if (_highlight != null)
                    {
                        _highlight.gameObject.SetActive(true);
                        _highlight.position = rect.center;
                        _highlight.sizeDelta = new Vector2(rect.width, rect.height);
                    }

                    break;
                }
            }
        }

        private TMP_LineInfo? GetLineOfWord(TMP_WordInfo wordInfo)
        {
            var textMeshPro = _textMesh.Get(this);

            foreach (var line in textMeshPro.textInfo.lineInfo)
            {
                if (line.firstCharacterIndex <= wordInfo.firstCharacterIndex &&
                    line.lastCharacterIndex >= wordInfo.firstCharacterIndex)
                {
                    return line;
                }
            }

            return null;
        }

        public void SetupForPuzzle(Puzzle puzzle)
        {
            _puzzle = puzzle;
            UpdateText(null);

            if (_relay != null)
            {
                _relay.State().BlankFilled += OnBlankFilled;
                _relay.State().PendingBlankSet += OnPendingBlankSet;
            }
        }

        private void OnBlankFilled(Puzzle puzzle)
        {
            if (_puzzle != null)
            {
                UpdateText(null);
            }

            if (_relay != null)
            {
                _relay.State().ClearPendingBlank();
            }
        }

        private void UpdateText(int? selectedIndex)
        {
            if (_puzzle != null)
            {
                _textMesh.Get(this).text = _puzzle.UnsolvedTextFormatted(selectedIndex);
            }
        }
    }
}