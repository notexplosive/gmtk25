using System;
using System.Collections.Generic;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.Core
{
    public class Puzzle 
    {
        
    }
    
    public class TopLevelPost
    {
    
    }
    
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
            var baseFollowerCount = (int)MathF.Pow(10, data.FollowerCountMagnitude);
            var extraFollowerCount = DirtyRandom.Instance.NextPositiveInt() %
                                     (int)MathF.Pow(10, data.FollowerCountMagnitude - 1);
            if (data.FollowerCountMagnitude <= 2)
            {
                extraFollowerCount = DirtyRandom.Instance.NextPositiveInt() % 10;
            }

            FollowerCount = baseFollowerCount + extraFollowerCount;
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