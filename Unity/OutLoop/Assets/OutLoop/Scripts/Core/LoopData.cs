using System;
using System.Collections.Generic;
using System.Linq;
using OutLoop.Data;
using UnityEngine;

namespace OutLoop.Core
{
    public class LoopData
    {
        private readonly List<Account> _allAccounts = new();
        private readonly List<Puzzle> _allPuzzles = new();
        private readonly List<TopLevelPost> _allTopLevelPosts = new();
        private readonly List<IPost> _bookmarkedPosts = new();
        private readonly List<DirectMessage> _messages = new();
        private readonly Dictionary<IPost, TopLevelPost> _postToOwner = new();
        private readonly HashSet<DirectMessage> _readMessages = new();
        private readonly List<string> _seenHashtags = new();
        private readonly List<string> _seenNames = new();
        private readonly List<TopLevelPost> _timelinePosts = new();
        private PageType _currentPage;

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
                foreach (var postId in puzzleData.TriggerPostIds)
                {
                    puzzle.TriggerPosts.Add(postsById[postId]);
                }

                _allPuzzles.Add(puzzle);
            }

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
            }
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
                return _seenHashtags.Contains(keyword);
            }

            if (keyword.StartsWith("@"))
            {
                return _seenNames.Contains(keyword);
            }

            return false;
        }

        public void AddToWordBank(string text)
        {
            _seenHashtags.Add(text);
            WordAddedToBank?.Invoke(AnswerType.Hashtag, text);
        }

        public void AddToNameBank(Account account)
        {
            var text = account.UserNameWithAt;
            _seenNames.Add(text);
            WordAddedToBank?.Invoke(AnswerType.Username, text);
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

        public void OnSelectedBankWord(string text)
        {
            Debug.Log($"Clicked on text {text}");
        }

        public List<string> GetWordsFromBank(AnswerType bankType)
        {
            switch (bankType)
            {
                case AnswerType.Username:
                    return _seenNames.ToList();
                case AnswerType.Hashtag:
                    return _seenHashtags.ToList();
                default:
                    return new List<string>();
            }
        }
    }
}