using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class LoopData
    {
        private readonly Dictionary<string, bool> _accountSeenStatus = new();
        private readonly List<Account> _allAccounts = new();
        private readonly List<Puzzle> _allPuzzles = new();
        private readonly List<TopLevelPost> _allTopLevelPosts = new();
        private readonly List<IPost> _bookmarkedPosts = new();
        private readonly Dictionary<string, bool> _hashTagSeenStatus = new();

        private readonly List<DirectMessage> _messages = new();
        private readonly Dictionary<IPost, TopLevelPost> _postToOwner = new();
        private readonly HashSet<DirectMessage> _readMessages = new();
        private readonly List<TopLevelPost> _timelinePosts = new();
        private readonly List<Puzzle> _untriggeredPuzzles = new();
        private PageType _currentPage;
        private PuzzleBlank? _pendingBlank;

        public LoopData()
        {
        }

        public LoopData(List<AccountData> accountDataList, List<PuzzleData> puzzleDataList,
            List<TopLevelPostData> topLevelPostList, List<string> timelinePostIds)
        {
            var accountTable = BuildAccounts(accountDataList);

            var topLevelPostById = new Dictionary<string, TopLevelPost>();

            foreach (var topLevelPostData in topLevelPostList)
            {
                var topLevelPost = new TopLevelPost(topLevelPostData, accountTable);
                _allTopLevelPosts.Add(topLevelPost);
                if (topLevelPost.RootPost.Id != null)
                {
                    topLevelPostById[topLevelPost.RootPost.Id] = topLevelPost;
                }
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
                _allPuzzles.Add(puzzle);
            }

            _untriggeredPuzzles.AddRange(_allPuzzles);

            foreach (var postId in timelinePostIds)
            {
                _timelinePosts.Add(topLevelPostById[postId]);
            }

            foreach (var topLevelPost in _allTopLevelPosts)
            {
                foreach (var childPost in topLevelPost.AllChildPosts())
                {
                    _postToOwner[childPost] = topLevelPost;
                }

                foreach (var text in topLevelPost.AllChildPosts()
                             .Select(a => a.RawText)
                             .Concat(_allAccounts.Select(a => a.Bio))
                             .Concat(_allAccounts.Select(a => a.DisplayName)
                             ))
                {
                    var hashTagMatches = Regex.Matches(text, @"#\w+");
                    foreach (var match in hashTagMatches)
                    {
                        _hashTagSeenStatus[match.ToString()] = false;
                    }
                }
            }

            foreach (var account in _allAccounts)
            {
                _accountSeenStatus.Add(account.UserNameWithAt, false);
            }

            _accountSeenStatus.Add(new Account().UserNameWithAt, false);
        }

        public PageType CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                PageUpdated?.Invoke(_currentPage);
            }
        }

        public IEnumerable<TopLevelPost> AllTopLevelPosts => _allTopLevelPosts;
        public IEnumerable<TopLevelPost> AllTopLevelPostsSorted => _allTopLevelPosts; // todo: algo sort
        public IEnumerable<TopLevelPost> TimelinePosts => _timelinePosts;
        public IEnumerable<IPost> BookmarkedPosts => _bookmarkedPosts;

        public List<Puzzle> InProgressPuzzles { get; } = new();
        public IEnumerable<Puzzle> AllPuzzles => _allPuzzles;

        public event Action<IPost>? BookmarkAdded;
        public event Action<IPost>? BookmarkRemoved;

        public void ReceiveMessage(DirectMessage message)
        {
            _messages.Add(message);
            MessageReceived?.Invoke(message);
        }

        public event Action<DirectMessage>? MessageReceived;
        public event Action<PageType>? PageUpdated;

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

        public IEnumerable<DirectMessage> AllMessages()
        {
            return _messages;
        }

        public bool IsMessageRead(DirectMessage message)
        {
            return _readMessages.Contains(message);
        }

        public void MarkMessageAsRead(DirectMessage message)
        {
            _readMessages.Add(message);
            MessageMarkedAsRead?.Invoke(message);
        }

        public event Action<DirectMessage>? MessageMarkedAsRead;

        public IEnumerable<Account> AllAccounts()
        {
            return _allAccounts;
        }

        public void AddBookmark(IPost post)
        {
            _bookmarkedPosts.Add(post);
            BookmarkAdded?.Invoke(post);
        }

        public void RemoveBookmark(IPost post)
        {
            _bookmarkedPosts.Remove(post);
            BookmarkRemoved?.Invoke(post);
        }

        public bool HasBookmark(IPost post)
        {
            return _bookmarkedPosts.Contains(post);
        }

        public void RequestCommentsModal(TopLevelPost post)
        {
            CommentsModalRequested?.Invoke(post);
        }

        public void RequestConversationModal(Account account)
        {
            ConversationModalRequested?.Invoke(account);
        }

        public void RequestProfileModal(Account account)
        {
            ProfileModalRequested?.Invoke(account);
        }

        public event Action<TopLevelPost>? CommentsModalRequested;
        public event Action<Account>? ConversationModalRequested;
        public event Action<Account>? ProfileModalRequested;
        public event Action<AnswerType, string>? WordAddedToBank;

        public bool HasSeenKeyword(string keyword)
        {
            if (keyword.StartsWith("#"))
            {
                return _hashTagSeenStatus.GetValueOrDefault(keyword);
            }

            if (keyword.StartsWith("@"))
            {
                return _accountSeenStatus.GetValueOrDefault(keyword);
            }

            return false;
        }

        public void AddToWordBank(string text)
        {
            _hashTagSeenStatus[text] = true;
            OnAddedWord(AnswerType.Hashtag, text);
        }

        private void OnAddedWord(AnswerType type, string text)
        {
            var triggeredPuzzles = new HashSet<Puzzle>();
            foreach (var puzzle in _untriggeredPuzzles)
            {
                puzzle.AddWord(text);
                if (puzzle.CanTrigger())
                {
                    TriggerPuzzle(puzzle);
                    triggeredPuzzles.Add(puzzle);
                }
            }

            _untriggeredPuzzles.RemoveAll(a => triggeredPuzzles.Contains(a));

            WordAddedToBank?.Invoke(type, text);
        }

        private void TriggerPuzzle(Puzzle puzzle)
        {
            PuzzleTriggered?.Invoke(puzzle);
        }

        public event Action<Puzzle>? PuzzleTriggered;

        public void AddToNameBank(Account account)
        {
            var text = account.UserNameWithAt;
            _accountSeenStatus[text] = true;
            OnAddedWord(AnswerType.Username, text);
        }

        public TopLevelPost? GetTopLevelOfPost(IPost? post)
        {
            if (post == null)
            {
                return null;
            }

            if (post is TopLevelPost topLevelPost)
            {
                return topLevelPost;
            }

            return _postToOwner.GetValueOrDefault(post.RootPost);
        }

        public void OnSelectedBankWord(AnswerType bankType, string text)
        {
            if (_pendingBlank != null)
            {
                _pendingBlank.GivenAnswer = text;

                if (bankType != _pendingBlank.AnswerType)
                {
                    return;
                }

                if (_pendingBlank.ParentPuzzle.IsSolved())
                {
                    SolvedPuzzle?.Invoke(_pendingBlank.ParentPuzzle);
                }

                BlankFilled?.Invoke(_pendingBlank.ParentPuzzle);
            }
        }

        public event Action<Puzzle>? SolvedPuzzle;
        public event Action<Puzzle>? BlankFilled;

        public List<string> GetWordsFromBank(AnswerType bankType)
        {
            switch (bankType)
            {
                case AnswerType.Username:
                    return _accountSeenStatus.Where(a => a.Value).Select(a => a.Key).ToList();
                case AnswerType.Hashtag:
                    return _hashTagSeenStatus.Where(a => a.Value).Select(a => a.Key).ToList();
                default:
                    return new List<string>();
            }
        }

        public int FoundBankCount(AnswerType bankType)
        {
            switch (bankType)
            {
                case AnswerType.Username:
                    return _accountSeenStatus.Count(a => a.Value);
                case AnswerType.Hashtag:
                    return _hashTagSeenStatus.Count(a => a.Value);
                default:
                    return 0;
            }
        }

        public int TotalBankCount(AnswerType bankType)
        {
            switch (bankType)
            {
                case AnswerType.Username:
                    return _accountSeenStatus.Count();
                case AnswerType.Hashtag:
                    return _hashTagSeenStatus.Count();
                default:
                    return 0;
            }
        }

        public IEnumerable<string> AllBankWords()
        {
            foreach (var x in _hashTagSeenStatus.Keys.ToList())
            {
                yield return x;
            }
        }

        public void StartPuzzle(Puzzle puzzle)
        {
            PuzzleStarted?.Invoke(puzzle);
            InProgressPuzzles.Add(puzzle);
        }

        public event Action<Puzzle>? PuzzleStarted;

        public IEnumerable<DirectMessage> GetDmConversationWith(Account account)
        {
            return AllMessages().Where(a => a.Sender == account);
        }

        public void SetPendingBlank(PuzzleBlank puzzleBlank)
        {
            _pendingBlank = puzzleBlank;
            PendingBlankSet?.Invoke(puzzleBlank);
        }

        public event Action<PuzzleBlank?>? PendingBlankSet;

        public void ClearPendingBlank()
        {
            _pendingBlank = null;
            PendingBlankSet?.Invoke(null);
        }
    }
}