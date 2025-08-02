using OutLoop.Core;
using OutLoop.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class PostBaseController : MonoBehaviour
    {
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
        private PostBaseController? _linkedPost;

        [SerializeField]
        private PostButtonRowController? _buttons;

        [SerializeField]
        private GameObject? _threadIndicator;

        public void Populate(IPost post, bool isPartOfThread)
        {
            var topLevelPost = post as TopLevelPost;
            
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
                _bodyText.text = post.RootPost.Text;
            }

            if (_media != null)
            {
                _media.sprite = post.RootPost.AttachedImage.ForceLoadNow(this);
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
    }
}