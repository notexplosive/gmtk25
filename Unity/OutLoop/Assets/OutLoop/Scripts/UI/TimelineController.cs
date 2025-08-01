using System.Linq;
using OutLoop.Core;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class TimelineController : MonoBehaviour
    {
        [SerializeField]
        private LoopDataRelay? _loopDataRelay;

        [SerializeField]
        private Transform? _contentRoot;

        [SerializeField]
        private PostBaseController? _postPrefab;

        public void Awake()
        {
            if (_contentRoot == null)
            {
                return;
            }

            if (_loopDataRelay == null)
            {
                return;
            }

            if (_postPrefab == null)
            {
                return;
            }

            for (var i = 0; i < _contentRoot.childCount; i++)
            {
                Destroy(_contentRoot.GetChild(0).gameObject);
            }
            
            var spawned = SpawnUtility.Spawn(_postPrefab, new InstantiateParameters { parent = _contentRoot });
            var loopData = _loopDataRelay.State();
            var topLevelPost = loopData.AllTopLevelPosts.First();
            spawned.Populate(topLevelPost.RootPost, false, topLevelPost.CommentCount());
        }
    }
}