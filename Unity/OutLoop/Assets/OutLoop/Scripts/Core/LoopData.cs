using System.Collections.Generic;
using System.Linq;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class LoopData
    {
        private readonly List<Account> _allAccounts = new();
        private List<Puzzle> _allPuzzles = new();
        private readonly List<TopLevelPost> _allTopLevelPosts = new();

        public LoopData(List<AccountData> accountDataList, List<PuzzleData> puzzleDataList,
            List<TopLevelPostData> topLevelPostList)
        {
            var accountTable = BuildAccounts(accountDataList);

            foreach (var topLevelPostData in topLevelPostList)
            {
                var topLevelPost = new TopLevelPost(topLevelPostData, accountTable);
                _allTopLevelPosts.Add(topLevelPost);
            }

            var postsById = new Dictionary<string, Post>();
            foreach (var post in _allTopLevelPosts.SelectMany(a => a.AllChildPosts()))
            {
                if (post.Id != null)
                {
                    postsById.Add(post.Id, post);
                }
            }

            foreach (var post in _allTopLevelPosts.SelectMany(a => a.AllChildPosts()))
            {
                if (post.OriginalData.LinkedPostId != null)
                {
                    var linkedPost = postsById[post.OriginalData.LinkedPostId];
                    post.LinkedPost = linkedPost;
                }
            }

            foreach (var puzzleData in puzzleDataList)
            {
                var puzzle = new Puzzle(puzzleData, accountTable);
                foreach (var postId in puzzleData.TriggerPostIds)
                {
                    puzzle.TriggerPosts.Add(postsById[postId]);
                }

                _allPuzzles.Add(puzzle);
            }
        }

        public Dictionary<string, Account> BuildAccounts(List<AccountData> accountDataList)
        {
            var accountsByName = new Dictionary<string, Account>();

            foreach (var data in accountDataList)
            {
                var account = new Account(data);
                _allAccounts.Add(account);
                accountsByName.Add(account.UserName, account);
            }

            foreach (var account in _allAccounts)
            {
                foreach (var knownFollowerName in account.OriginalData.KnownFollowers)
                {
                    account.KnownFollowers.Add(accountsByName[knownFollowerName]);
                }
            }

            return accountsByName;
        }
    }
}