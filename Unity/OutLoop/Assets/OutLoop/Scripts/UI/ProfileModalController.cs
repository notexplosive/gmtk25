using OutLoop.Core;
using SecretPlan.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class ProfileModalController : ModalController
    {
        [SerializeField]
        private TextMeshWithHyperlinks? _nameTextMesh;
        
        [SerializeField]
        private TextMeshWithHyperlinks? _bioTextMesh;
        
        [SerializeField]
        private TMP_Text? _followerCountTextMesh;
        
        [SerializeField]
        private Image? _profilePicture;

        [SerializeField]
        private LoopDataRelay? _loopDataRelay;

        [SerializeField]
        private Transform? _contentRoot;

        [SerializeField]
        private PostBaseController? _postPrefab;
        
        public void PopulateWithProfile(Account account)
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
            foreach (var topLevelPost in loopData.AllTopLevelPostsSorted)
            {
                if (topLevelPost.RootPost.Author == account)
                {
                    var spawned = SpawnUtility.Spawn(_postPrefab, new InstantiateParameters { parent = _contentRoot });
                    spawned.Populate(topLevelPost, false);
                }
            }

            if (_nameTextMesh != null)
            {
                _nameTextMesh.SetText(account.DisplayNameAndUsernameStyled);
            }

            if (_bioTextMesh != null)
            {
                _bioTextMesh.SetText(account.Bio);
            }

            if (_followerCountTextMesh != null)
            {
                var followerCount = OutloopHelpers.FormatNumberAsString(account.FollowerCount);
                _followerCountTextMesh.text = $"<style=Username>{followerCount} Followers</style>";
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = account.ProfilePicture.ForceLoadNow(this);
            }
            
            
            _loopDataRelay.State().AddToNameBank(account);
        }
    }
}