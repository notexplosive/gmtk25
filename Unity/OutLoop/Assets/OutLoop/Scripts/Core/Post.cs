using System.Collections.Generic;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.Core
{
    public class Post : IPost
    {
        public Post(PostData postData, Dictionary<string, Account> accountTable)
        {
            Id = postData.PostId;
            OriginalData = postData;
            SearchableText = postData.Text;
            FormattedText = OutloopHelpers.FormatPost(postData.Text);
            Author = postData.AuthorUsername == null ? new Account() : accountTable[postData.AuthorUsername];
            Likes = OutloopHelpers.CalculateLikes(postData.LikesMagnitude ?? Author.OriginalData.FollowerCountMagnitude);
            Reposts = OutloopHelpers.CalculateReposts(postData.RepostsMagnitude ?? Author.OriginalData.FollowerCountMagnitude);
            AttachedImage = new Addressable<Sprite>(postData.ImagePath ?? string.Empty);
        }

        public string FormattedText { get; }
        public int Reposts { get; set; }
        public int Likes { get; set; }
        public string LikesFormatted => OutloopHelpers.FormatNumberAsString(Likes);
        public Account Author { get; }
        public string SearchableText { get; }
        public PostData OriginalData { get; }
        public string? Id { get; }
        public Addressable<Sprite> AttachedImage { get; }
        public Post? LinkedPost { get; set; }
        public Post RootPost => this;
    }
}