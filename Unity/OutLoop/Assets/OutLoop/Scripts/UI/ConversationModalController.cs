using OutLoop.Core;
using SecretPlan.Core;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class ConversationModalController : ModalController
    {
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private Image? _profilePicture;

        [SerializeField]
        private TextMeshWithHyperlinks? _usernameTextMesh;

        [SerializeField]
        private RectTransform? _contentRoot;

        [SerializeField]
        private DirectMessageController? _directMessageFromSenderPrefab;

        [SerializeField]
        private DirectMessageController? _directMessageFromMePrefab;

        [SerializeField]
        private ScrollRect? _scrollRect;

        public Account? Sender { get; private set; }


        public void PopulateWithProfile(Account account)
        {
            Sender = account;

            if (_contentRoot == null)
            {
                return;
            }

            if (_relay == null)
            {
                return;
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = account.ProfilePicture.ForceLoadNow(this);
            }

            if (_usernameTextMesh != null)
            {
                _usernameTextMesh.SetText(account.UserNameWithAt);
            }

            for (var i = 0; i < _contentRoot.childCount; i++)
            {
                Destroy(_contentRoot.GetChild(i).gameObject);
            }

            foreach (var message in _relay.State().GetDmConversationWith(account))
            {
                if (_directMessageFromSenderPrefab != null)
                {
                    var instance = SpawnUtility.Spawn(_directMessageFromSenderPrefab,
                        new InstantiateParameters { parent = _contentRoot });
                    instance.Populate(message);
                }
            }
            
            // scroll to bottom
            if (_scrollRect != null)
            {
                _scrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }
}