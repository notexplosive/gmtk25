using System.Collections.Generic;
using OutLoop.Core;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class BookmarksController : MonoBehaviour
    {
        [SerializeField]
        private Transform? _contentRoot;

        [SerializeField]
        private PostBaseController? _postPrefab;

        [SerializeField]
        private LoopDataRelay? _loopDataRelay;

        private Dictionary<IPost, PostBaseController> _postInstances = new();

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
            foreach (var post in loopData.BookmarkedPosts)
            {
                CreateBookmarkedPost(post);
            }

            loopData.BookmarkAdded += OnBookmarkAdded;
            loopData.BookmarkRemoved += OnBookmarkRemoved;
        }

        private PostBaseController? CreateBookmarkedPost(IPost post)
        {
            if (_postPrefab == null)
            {
                return null;
            }

            var spawned = SpawnUtility.Spawn(_postPrefab, new InstantiateParameters { parent = _contentRoot });
            spawned.Populate(post, false);
            return spawned;
        }


        private void OnBookmarkAdded(IPost post)
        {
            var spawned = CreateBookmarkedPost(post);
            if (spawned != null)
            {
                _postInstances[post] = spawned;
            }
        }

        private void OnBookmarkRemoved(IPost post)
        {
            if (_postInstances[post] != null)
            {
                Destroy(_postInstances[post].gameObject);
            }
        }
    }
}