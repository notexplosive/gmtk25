using System.Collections.Generic;
using OutLoop.Core;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class MessagesController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private ConversationController? _conversationPrefab;

        [Header("Internal")]
        [SerializeField]
        private Transform? _conversationRoot;

        private readonly Dictionary<Account, ConversationController> _accountsToConversationInstances = new();

        private void Awake()
        {
            if (_conversationRoot != null)
            {
                for (var i = 0; i < _conversationRoot.childCount; i++)
                {
                    Destroy(_conversationRoot.GetChild(i).gameObject);
                }
            }

            if (_relay != null)
            {
                _relay.State().MessageReceived += OnMessageReceived;

                foreach (var message in _relay.State().AllMessages())
                {
                    AddMessage(message);
                }
            }
        }

        private void AddMessage(DirectMessage message)
        {
            if (_conversationPrefab == null)
            {
                return;
            }

            var conversation = _accountsToConversationInstances.GetValueOrDefault(message.Sender);
            if (conversation == null)
            {
                conversation = SpawnUtility.Spawn(_conversationPrefab,
                    new InstantiateParameters { parent = _conversationRoot });
                _accountsToConversationInstances[message.Sender] = conversation;
            }

            conversation.Populate(message);
        }

        private void OnMessageReceived(DirectMessage message)
        {
            AddMessage(message);
        }
    }
}