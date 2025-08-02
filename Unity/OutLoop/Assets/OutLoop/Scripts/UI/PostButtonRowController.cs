using OutLoop.Core;
using SecretPlan.UI;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    public class PostButtonRowController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private LoopDataRelay? _relay;

        [Header("Internal")]
        [SerializeField]
        private SecretButton? _comment;

        [SerializeField]
        private RepostButtonController? _repost;

        [SerializeField]
        private LikeButtonController? _like;

        [SerializeField]
        private TMP_Text? _commentCount;

        public void Setup(IPost post)
        {
            if (_like != null)
            {
                _like.Setup(post);
            }

            if (_repost != null)
            {
                _repost.Setup(post);
            }

            if (post is TopLevelPost topLevelPost)
            {
                if (_commentCount != null)
                {
                    _commentCount.text = topLevelPost.CommentCount().ToString();
                }

                if (_comment != null)
                {
                    _comment.Clicked += () =>
                    {
                        if (_relay != null)
                        {
                            _relay.State().RequestCommentsModal(topLevelPost);
                        }
                    };
                }
            }
            else
            {
                if (_comment != null)
                {
                    _comment.gameObject.SetActive(false);
                }
            }
        }
    }
}