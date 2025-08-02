using System.Collections.Generic;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class TopLevelPost : IPost
    {
        public TopLevelPost(TopLevelPostData data, Dictionary<string, Account> accountTable)
        {
            OriginalData = data;
            RootPost = new Post(data.OriginalPost, accountTable);

            foreach (var threadComment in data.ThreadEntries)
            {
                ThreadComments.Add(new Post(threadComment, accountTable));
            }

            foreach (var threadComment in data.NormalComments)
            {
                NormalComments.Add(new Post(threadComment, accountTable));
            }
        }

        public List<Post> NormalComments { get; } = new();
        public List<Post> ThreadComments { get; } = new();
        public Post RootPost { get; }
        public TopLevelPostData OriginalData { get; }

        public IEnumerable<Post> AllChildPosts()
        {
            yield return RootPost;
            foreach (var post in NormalComments)
            {
                yield return post;
            }

            foreach (var post in ThreadComments)
            {
                yield return post;
            }
        }

        public int CommentCount()
        {
            return NormalComments.Count + ThreadComments.Count;
        }
    }
}