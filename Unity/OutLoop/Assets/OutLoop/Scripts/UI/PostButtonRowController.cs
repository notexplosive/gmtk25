using SecretPlan.UI;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    public class PostButtonRowController : MonoBehaviour
    {
        [SerializeField]
        private SecretButton? _comment;
        
        [SerializeField]
        private SecretButton? _repost;
        
        [SerializeField]
        private SecretButton? _like;
        
        [SerializeField]
        private TMP_Text? _commentCount;
        
        [SerializeField]
        private TMP_Text? _repostCount;
        
        [SerializeField]
        private TMP_Text? _likeCount;

        public void DisplayStats(int likes, int reposts, int comments)
        {
            if (_likeCount != null)
            {
                _likeCount.text = likes.ToString();
            }
            
            if (_repostCount != null)
            {
                _repostCount.text = reposts.ToString();
            }
            
            if (_commentCount != null)
            {
                _commentCount.text = comments.ToString();
            }
        }
    }
}