using System.Collections.Generic;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.Core
{
    public class Account
    {
        public Account(AccountData data)
        {
            UserName = data.UserName;
            DisplayName = data.DisplayName;
            if (data.IsFakeProfile)
            {
                Bio = "<style=System>This profile cannot be loaded at this time.</style>";
            }

            Bio = data.Bio ?? string.Empty;
            ProfilePicture = new Addressable<Sprite>("pfp_tronix");
            var followerCount = Constants.CalculateFollowers(data.FollowerCountMagnitude);
            FollowerCount = followerCount;
            OriginalData = data;
        }

        public string UserName { get; }
        public string DisplayName { get; }
        public string Bio { get; }
        public int FollowerCount { get; }
        public List<Account> KnownFollowers { get; } = new();
        public Addressable<Sprite> ProfilePicture { get; }
        public AccountData OriginalData { get; }

        public string UserNameWithAt => $"@{UserName}";
    }
}