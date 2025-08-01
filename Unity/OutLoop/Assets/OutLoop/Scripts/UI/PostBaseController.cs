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

        public void Populate(PostData post, bool isPartOfThread)
        {
            if (_threadIndicator != null)
            {
                _threadIndicator.SetActive(isPartOfThread);
            }

            if (_profilePicture != null)
            {
                
            }

            if (_displayNameAndUserName != null)
            {
                // _displayNameAndUserName.text = ;
            }
        }
    }
}