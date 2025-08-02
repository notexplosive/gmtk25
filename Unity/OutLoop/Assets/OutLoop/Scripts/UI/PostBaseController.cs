using OutLoop.Core;
using OutLoop.Data;
using SecretPlan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class PostBaseController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private LoopDataRelay? _relay;
        
        [Header("Base Post")]
        [SerializeField]
        private Image? _profilePicture;

        [SerializeField]
        private TMP_Text? _displayNameAndUserName;

        [SerializeField]
        private TMP_Text? _bodyText;

        [SerializeField]
        private Image? _media;

        [SerializeField]
        private TMP_Text? _contextHeader;

        [SerializeField]
        private SecretButton? _profileButton;
        
        [SerializeField]
        private SecretButton? _textButton;
        
        [Header("Normal Post")]
        
        [SerializeField]
        private PostBaseController? _linkedPost;
        
        [SerializeField]
        private PostButtonRowController? _buttons;

        [SerializeField]
        private GameObject? _threadIndicator;

        public IPost? CachedPost { get; private set; }

        public void Populate(IPost post, bool isPartOfThread)
        {
            CachedPost = post;
            if (post is TopLevelPost topLevelPost)
            {
                if (_textButton != null)
                {
                    _textButton.Clicked += () => { OpenComments(topLevelPost); };
                }
            }

            if (_profileButton != null)
            {
                _profileButton.Clicked += () =>
                {
                    if (_relay != null)
                    {
                        _relay.State().RequestProfileModal(post.RootPost.Author);
                    }
                };
            }

            if (_threadIndicator != null)
            {
                _threadIndicator.SetActive(isPartOfThread);
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = post.RootPost.Author.ProfilePicture.ForceLoadNow(this);
            }

            if (_displayNameAndUserName != null)
            {
                _displayNameAndUserName.text = post.RootPost.Author.DisplayNameAndUsernameStyled;
            }

            if (_bodyText != null)
            {
                if (_relay != null)
                {
                    _bodyText.text = post.RootPost.FormattedText(_relay.State());
                }
            }

            if (_media != null)
            {
                if (post.RootPost.AttachedImage != null)
                {
                    _media.sprite = post.RootPost.AttachedImage.ForceLoadNow(this);
                    _media.gameObject.SetActive(true);
                }
                else
                {
                    _media.gameObject.SetActive(false);
                }
            }

            if (_contextHeader != null)
            {
                // not sure if we're even using this anymore
            }

            if (_linkedPost != null)
            {
                if (post.RootPost.LinkedPost != null)
                {
                    _linkedPost.Populate(post.RootPost.LinkedPost, false);
                }
            }

            if (_buttons != null)
            {
                _buttons.Setup(post);
            }
        }

        public void OpenComments(TopLevelPost post)
        {
            if (_relay != null)
            {
                _relay.State().RequestCommentsModal(post);
            }
        }
    }
}