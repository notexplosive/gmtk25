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
    public class TextMeshWithHyperlinks : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private LoopDataRelay? _relay;

        private readonly CachedComponent<TMP_Text> _textMesh = new();
        private string _rawText = string.Empty;

        private void OnEnable()
        {
            if (_relay != null)
            {
                _relay.State().WordAddedToBank += OnWordAdded;
                ReRunFormatter();
            }
        }

        private void OnDisable()
        {
            if (_relay != null)
            {
                _relay.State().WordAddedToBank -= OnWordAdded;
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
                var linkTextNoPrefix = linkText.Remove(0, 1);
                if (linkText.StartsWith("@"))
                {
                    if (_relay != null)
                    {
                        var state = _relay.State();
                        var account = state.AllAccounts().FirstOrDefault(a => a.UserName == linkTextNoPrefix) ??
                                      new Account();
                        state.RequestProfileModal(account);
                        // state.AddToNameBank(account);
                    }
                }
                else if (linkText.StartsWith("#"))
                {
                    if (_relay != null)
                    {
                        var state = _relay.State();
                        state.AddToWordBank(linkText);
                    }
                }

                ReRunFormatter();
            }
            else
            {
                if (_relay != null)
                {
                    var parentController = GetComponentInParent<PostBaseController>();
                    if (parentController != null)
                    {
                        var post = _relay.State().GetTopLevelOfPost(parentController.CachedPost);
                        if (post != null)
                        {
                            parentController.OpenComments(post);
                        }
                    }
                }
            }
        }

        private void OnWordAdded(AnswerType answerType, string word)
        {
            ReRunFormatter();
        }

        public void SetText(string rawText)
        {
            _rawText = rawText;
            ReRunFormatter();
        }

        private void ReRunFormatter()
        {
            if (_relay != null)
            {
                _textMesh.Get(this).text = OutloopHelpers.FormatWithHyperlinks(_rawText, _relay.State());
            }
        }
    }
}