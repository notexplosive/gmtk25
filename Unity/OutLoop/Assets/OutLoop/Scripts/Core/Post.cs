using System.Collections.Generic;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class Post
    {
        public Post(PostData postData, Dictionary<string, Account> accountTable)
        {
            Id = postData.PostId;
            OriginalData = postData;
            Text = postData.Text;
            Author = accountTable[postData.AuthorUsername];
            Likes = Constants.CalculateReposts(postData.RepostsMagnitude ?? Author.OriginalData.FollowerCountMagnitude);
            Reposts = Constants.CalculateLikes(postData.LikesMagnitude ?? Author.OriginalData.FollowerCountMagnitude);
        }

        public int Reposts { get; }
        public int Likes { get; }
        public Account Author { get; }
        public string Text { get; }
        public PostData OriginalData { get; }
        public string? Id { get; }
        public Post? LinkedPost { get; set; }
    }
}