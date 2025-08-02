using System.Collections.Generic;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.Core
{
    public class Account
    {
        private readonly bool _isUnavailableAccount;

        public Account()
        {
            _isUnavailableAccount = true;
            UserName = "";
            DisplayName = "";
            Bio = "";
            ProfilePicture = new Addressable<Sprite>("ProfilePictures/pfp_grey.png");
            OriginalData = new AccountData();
        }

        public Account(AccountData data)
        {
            UserName = data.UserName;
            DisplayName = string.IsNullOrEmpty(DisplayName) ? data.UserName : data.DisplayName;
            if (data.IsFakeProfile)
            {
                Bio = "<style=System>This profile cannot be loaded at this time.</style>";
            }

            Bio = data.Bio ?? string.Empty;

            if (data.IsLikelyFakeProfile())
            {
                ProfilePicture = new Addressable<Sprite>("ProfilePictures/pfp_default.png");
            }
            else
            {
                ProfilePicture = new Addressable<Sprite>($"ProfilePictures/{data.ProfilePicture}.png");
            }

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

        public string DisplayNameAndUsernameStyled
        {
            get
            {
                if (_isUnavailableAccount)
                {
                    return "<style=System>(Account Corrupted)</style>";
                }

                return $"{DisplayName} <style=Username>{UserNameWithAt}</style>";
            }
        }
    }
}