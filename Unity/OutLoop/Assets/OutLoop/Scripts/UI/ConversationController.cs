using System;
using OutLoop.Core;
using SecretPlan.UI;
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
        private Image? _unreadBadge;

        [SerializeField]
        private SecretButton? _button;

        private bool _isSolved;


        public void Populate(DirectMessage message)
        {
            if (_latestMessageTextMesh != null)
            {
                _latestMessageTextMesh.text = message.Message;
            }

            if (_senderTextMesh != null)
            {
                _senderTextMesh.text = "<style=Username>"+message.Sender.DisplayName+"</style>";
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = message.Sender.ProfilePicture.ForceLoadNow(this);
            }

            if (_relay == null)
            {
                return;
            }

            if (_button != null)
            {
                _button.Clicked += () => _relay.State().RequestConversationModal(message.Sender);
            }
            
            if (_unreadBadge != null && _relay != null)
            {
                _relay.State().MessageMarkedAsRead += _ =>
                {
                    if (_isSolved)
                    {
                        return;
                    }
                    
                    _unreadBadge.gameObject.SetActive(!_relay.State().IsMessageRead(message));
                };
                
                _relay.State().SolvedPuzzle += puzzle =>
                {
                    if (puzzle.Sender == message.Sender)
                    {
                        _unreadBadge.gameObject.SetActive(true);
                        _unreadBadge.color = Color.green;
                        if (ColorUtility.TryParseHtmlString("#9CD7A2", out var color))
                        {
                            _unreadBadge.color = color;
                        }

                        _isSolved = true;
                    }
                };
            }
        }
    }
}