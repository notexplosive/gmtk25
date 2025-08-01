using System.Text.RegularExpressions;
using OutLoop.Core;
using SecretPlan.Core;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    public class SearchController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField]
        private LoopDataRelay? _loopDataRelay;

        [SerializeField]
        private PostBaseController? _topLevelPostPrefab;
        
        [Header("Internal")]
        [SerializeField]
        private TMP_InputField? _inputField;

        [SerializeField]
        private Transform? _searchResultsRoot;
        
        private string _cachedSearchText = string.Empty;
        private string _currentSearchQuery = string.Empty;
        private float _delay;

        private void Awake()
        {
            if (_inputField != null)
            {
                _inputField.onValueChanged.AddListener(TextChanged);
            }
        }

        private void Update()
        {
            if (_delay <= 0 && _currentSearchQuery != _cachedSearchText)
            {
                _currentSearchQuery = _cachedSearchText;
                DoSearch();
            }

            _delay -= Time.deltaTime;
        }

        private void DoSearch()
        {
            if (_loopDataRelay == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_currentSearchQuery))
            {
                // no results for an empty post
                return;
            }

            int postLimit = 6;

            foreach (var post in _loopDataRelay.State().AllTopLevelPostsSorted)
            {
                var match = Regex.Match(post.RootPost.Text, @$"\b({_currentSearchQuery})\b", RegexOptions.IgnoreCase);
                if (match.Length > 0)
                {
                    SpawnPost(post);
                    postLimit--;
                }

                if (postLimit <= 0)
                {
                    break;
                }
            }
        }

        private void SpawnPost(TopLevelPost post)
        {
            if (_topLevelPostPrefab != null)
            {
                var instance = SpawnUtility.Spawn(_topLevelPostPrefab, new InstantiateParameters() { parent = _searchResultsRoot });
                instance.Populate(post.RootPost, false, post.CommentCount());
            }
        }

        private void TextChanged(string text)
        {
            if (_searchResultsRoot != null)
            {
                for (var i = 0; i < _searchResultsRoot.childCount; i++)
                {
                    Destroy(_searchResultsRoot.GetChild(i).gameObject);
                }
            }
            
            _cachedSearchText = text;
            _delay = 0.25f;
        }
    }
}