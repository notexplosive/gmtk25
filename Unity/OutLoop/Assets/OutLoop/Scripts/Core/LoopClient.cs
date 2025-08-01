using System.Collections.Generic;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class LoopClient
    {
        private List<Account> _allAccounts = new();
        private List<Puzzle> _allPuzzles = new();
        private List<TopLevelPost> _allTopLevelPosts = new();

        public void BuildAccounts(List<AccountData> accountDataList)
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
        }

        public LoopClient(List<AccountData> accountDataList, List<PuzzleData> puzzleDataList, List<TopLevelPostData> topLevelPostList)
        {
            BuildAccounts(accountDataList);

            // foreach (var )
        }
    }
}