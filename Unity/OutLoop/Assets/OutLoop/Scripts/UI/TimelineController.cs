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
                Destroy(_contentRoot.GetChild(i).gameObject);
            }

            var loopData = _loopDataRelay.State();
            foreach (var topLevelPost in loopData.TimelinePosts)
            {
                var spawned = SpawnUtility.Spawn(_postPrefab, new InstantiateParameters { parent = _contentRoot });
                spawned.Populate(topLevelPost.RootPost, false, topLevelPost.CommentCount());
            }
        }
    }
}