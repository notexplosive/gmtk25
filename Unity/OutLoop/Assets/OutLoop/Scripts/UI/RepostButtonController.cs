using OutLoop.Core;
using SecretPlan.Core;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    [RequireComponent(typeof(PostNumberButtonController))]
    public class RepostButtonController : MonoBehaviour
    {
        [SerializeField]
        private LoopDataRelay? _relay;
        
        [SerializeField]
        private TMP_Text? _counter;

        private readonly CachedComponent<PostNumberButtonController> _button = new();

        public void Setup(IPost post)
        {
            _button.Get(this).Setup(post);
            
            if (_relay == null)
            {
                return;
            }

            if (_relay.State().HasBookmark(post))
            {
                _button.Get(this).SetToggleState(true);
            }
            
            UpdateCounter(post);
            
            _button.Get(this).Toggled += isOn =>
            {
                if (isOn)
                {
                    post.RootPost.Reposts++;
                    UpdateCounter(post);
                    _relay.State().AddBookmark(post);
                }
                else
                {
                    post.RootPost.Reposts--;
                    UpdateCounter(post);
                    _relay.State().RemoveBookmark(post);
                }
            };
        }
        
        private void UpdateCounter(IPost post)
        {
            if (_counter != null)
            {
                _counter.text = post.RootPost.Reposts.ToString();
            }
        }
    }
}