using OutLoop.Core;
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

        public void Populate(Post post, bool isPartOfThread, int comments)
        {
            if (_threadIndicator != null)
            {
                _threadIndicator.SetActive(isPartOfThread);
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = post.Author.ProfilePicture.ForceLoadNow(this);
            }

            if (_displayNameAndUserName != null)
            {
                _displayNameAndUserName.text = post.Author.DisplayNameAndUsernameStyled;
            }

            if (_bodyText != null)
            {
                _bodyText.text = post.Text;
            }

            if (_media != null)
            {
                _media.sprite = post.AttachedImage.ForceLoadNow(this);
            }

            if (_contextHeader != null)
            {
                // not sure if we're even using this anymore
            }

            if (_linkedPost != null)
            {
                if (post.LinkedPost != null)
                {
                    _linkedPost.Populate(post.LinkedPost, false, 0);
                }
            }

            if (_buttons != null)
            {
                _buttons.DisplayStats(post.Likes, post.Reposts, comments);
            }
        }
    }
}