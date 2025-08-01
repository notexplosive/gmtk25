using OutLoop.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class ConversationController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private LoopDataRelay? _relay;
        
        [Header("Internal")]
        [SerializeField]
        private TMP_Text? _latestMessageTextMesh;
        
        [SerializeField]
        private TMP_Text? _senderTextMesh;
        
        [SerializeField]
        private Image? _profilePicture;

        [SerializeField]
        private GameObject? _unreadBadge;

        public void Populate(DirectMessage message)
        {
            if (_latestMessageTextMesh != null)
            {
                _latestMessageTextMesh.text = message.Message;
            }

            if (_senderTextMesh != null)
            {
                _senderTextMesh.text = message.Sender.DisplayNameAndUsernameStyled;
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = message.Sender.ProfilePicture.ForceLoadNow(this);
            }

            if (_unreadBadge != null && _relay != null)
            {
                _unreadBadge.SetActive(_relay.State().IsMessageRead(message));
            }
        }
    }
}