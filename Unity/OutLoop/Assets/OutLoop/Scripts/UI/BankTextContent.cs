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
        [SerializeField]
        private LoopDataRelay? _relay;

        private readonly string _rawText = string.Empty;

        private readonly CachedComponent<TMP_Text> _textMesh = new();

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
                    _relay.State().OnSelectedBankWord(linkText);
                }
            }
        }

        public void Add(AnswerType bankType, string word)
        {
            if (_relay == null)
            {
                return;
            }

            var words = _relay.State().GetWordsFromBank(bankType);
            _textMesh.Get(this).text = string.Join("  ", words.Select(text => $"<link={text}>" + text + "</link>"));
        }
    }
}