using System.Linq;
using OutLoop.Core;
using OutLoop.Data;
using SecretPlan.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutLoop.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class BankTextContent : MonoBehaviour, IPointerClickHandler
    {
        public const int PageSize = 10;

        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private TextMeshProUGUI? _pageNumberTextMesh;

        private readonly string _rawText = string.Empty;

        private readonly CachedComponent<TMP_Text> _textMesh = new();
        private AnswerType _bankType;
        private int _pageNumber;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                _pageNumber = value;
                if (_pageNumberTextMesh != null)
                {
                    _pageNumberTextMesh.text = (_pageNumber + 1).ToString();
                }

                UpdateText();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var textMeshPro = _textMesh.Get(this);
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, null);
            if (linkIndex != -1)
            {
                var linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
                var linkText = linkInfo.GetLinkID();

                if (_relay != null)
                {
                    _relay.State().OnSelectedBankWord(_bankType, linkText);
                }
            }
        }

        public void Setup(AnswerType bankType)
        {
            _bankType = bankType;
            UpdateText();
        }

        public void UpdateText()
        {
            if (_relay == null)
            {
                return;
            }

            var words = _relay.State().GetWordsFromBank(_bankType);
            words.Sort();
            if (_pageNumber > 0)
            {
                words.RemoveRange(0, PageSize * _pageNumber);
            }

            _textMesh.Get(this).text =
                string.Join("  ", words.Take(PageSize).Select(text => $"<link={text}>" + text + "</link>"));
            _textMesh.Get(this).ForceMeshUpdate();
        }
    }
}