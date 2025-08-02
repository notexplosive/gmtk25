using System.Linq;
using OutLoop.Core;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            var tmpText = _textMesh.Get(this);
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, eventData.position, null);
            if (linkIndex != -1)
            {
                var linkInfo = tmpText.textInfo.linkInfo[linkIndex];
                var username = linkInfo.GetLinkID();
                if (username.StartsWith("@"))
                {
                    username = username.Remove(0, 1);
                    if (_relay != null)
                    {
                        var state = _relay.State();
                        var account = state.AllAccounts().FirstOrDefault(a => a.UserName == username) ?? new Account();
                        state.RequestProfileModal(account);
                    }
                }
            }
            else
            {
                if (_relay != null)
                {
                    var parentController = GetComponentInParent<PostBaseController>();
                    if (parentController != null)
                    {
                        if (parentController.CachedPost is TopLevelPost post)
                        {
                            parentController.OpenComments(post);
                        }
                    }
                }
            }
        }
    }
}