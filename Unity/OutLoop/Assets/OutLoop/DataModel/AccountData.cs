using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace OutLoop.Data
{
    [Serializable]
    [YamlSerializable]
    public class AccountData
    {
        [YamlMember(Alias = "user_name")]
        public string UserName { get; set; } = "";

        [YamlMember(Alias = "display_name")]
        public string DisplayName { get; set; } = "";

        [YamlMember(Alias = "is_fake_profile")]
        public bool IsFakeProfile { get; set; } = false;

        [YamlMember(Alias = "profile_picture")]
        public string? ProfilePicture { get; set; } = null;

        [YamlMember(Alias = "bio")]
        public string? Bio { get; set; } = null;

        [YamlMember(Alias = "follower_count")]
        public int FollowerCountMagnitude { get; set; } = 1;

        [YamlMember(Alias = "known_followers")]
        public List<string> KnownFollowers { get; set; } = new();

        public bool IsLikelyFakeProfile()
        {
            if (IsFakeProfile)
            {
                return true;
            }

            if (ProfilePicture == null)
            {
                return true;
            }

            return false;
        }
    }
}