using OutLoop.Core;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class CommentsModalController : ModalController
    {
        [SerializeField]
        private Transform? _contentRoot;

        [SerializeField]
        private LoopDataRelay? _loopDataRelay;

        [SerializeField]
        private PostBaseController? _postPrefab;

        public void PopulateForPost(TopLevelPost post)
        {
            if (_contentRoot == null)
            {
                return;
            }

            if (_loopDataRelay == null)
            {
                return;
            }

            for (var i = 0; i < _contentRoot.childCount; i++)
            {
                Destroy(_contentRoot.GetChild(i).gameObject);
            }

            var hasThread = post.ThreadComments.Count > 0;
            
            AddPost(post.RootPost, hasThread);

            for (var index = 0; index < post.ThreadComments.Count; index++)
            {
                var threadComment = post.ThreadComments[index];
                var isLastThreadComment = index == post.ThreadComments.Count - 1;
                
                AddPost(threadComment, !isLastThreadComment);
            }

            foreach (var normalComment in post.NormalComments)
            {
                AddPost(normalComment, false);
            }
        }

        private void AddPost(IPost topLevelPost, bool isPartOfThread)
        {
            if (_postPrefab == null)
            {
                return;
            }

            var spawned = SpawnUtility.Spawn(_postPrefab, new InstantiateParameters { parent = _contentRoot });
            spawned.Populate(topLevelPost, isPartOfThread);
        }
    }
}