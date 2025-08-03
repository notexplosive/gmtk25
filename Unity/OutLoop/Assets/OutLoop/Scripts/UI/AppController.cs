using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OutLoop.Core;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;

namespace OutLoop.UI
{
    public class AppController : MonoBehaviour
    {
        [SerializeField]
        private AudioClip? _receiveDmSound;

        [SerializeField]
        private AudioClip? _addClueToBankSound;

        [SerializeField]
        private AudioClip? _openModalSound;

        [SerializeField]
        private AudioClip? _swipeOver;

        [SerializeField]
        private AudioClip? _blankFilledSound;
        
        [SerializeField]
        private AudioClip? _blankSelectedSound;
        
        [SerializeField]
        private AppButtonRow? _appButtonRow;

        [SerializeField]
        private List<AppPage> _pages = new();

        [SerializeField]
        private LoopDataRelay? _relay;


        [SerializeField]
        private CommentsModalController? _commentsPrefab;

        [SerializeField]
        private ProfileModalController? _profilePrefab;

        [SerializeField]
        private ConversationModalController? _conversationPrefab;

        private AppPage? _currentPage;
        private bool _hasTriggeredEnding;

        private void Awake()
        {
            if (_appButtonRow == null)
            {
                Debug.Log("Missing button row");
                return;
            }

            if (_pages.Count < 4)
            {
                Debug.Log($"Not enough pages, only found {_pages.Count}");
                return;
            }

            _currentPage = _pages.First();

            foreach (var page in _pages)
            {
                if (page != _currentPage)
                {
                    page.gameObject.SetActive(false);
                }
            }

            if (_appButtonRow.HomeButton != null)
            {
                _appButtonRow.HomeButton.Clicked += CreatePageToEvent(_pages[0]);
            }

            if (_appButtonRow.SearchButton != null)
            {
                _appButtonRow.SearchButton.Clicked += CreatePageToEvent(_pages[1]);
            }

            if (_appButtonRow.MessagesButton != null)
            {
                _appButtonRow.MessagesButton.Clicked += CreatePageToEvent(_pages[2]);
            }

            if (_appButtonRow.FavoritesButton != null)
            {
                _appButtonRow.FavoritesButton.Clicked += CreatePageToEvent(_pages[3]);
            }

            if (_relay == null)
            {
                return;
            }

            var loopData = _relay.State();

            loopData.SolvedPuzzle += _ =>
            {
                if (_hasTriggeredEnding)
                {
                    return;
                }
                
                if (loopData.AllPuzzles.All(a => a.IsSolved()))
                {
                    _hasTriggeredEnding = true;
                    StartCoroutine(Ending(loopData));
                }
            };

            loopData.WordAddedToBank += (_, _) => { SoundService.Instance.PlaySound(_addClueToBankSound); };

            loopData.MessageReceived += (_) =>
            {
                SoundService.Instance.PlaySound(_receiveDmSound);
            };
            
            loopData.BlankFilled += (clue) =>
            {
                SoundService.Instance.PlaySound(_blankFilledSound);
            };

            loopData.PendingBlankSet += (blank) =>
            {
                if (blank == null)
                {
                    return;
                }

                SoundService.Instance.PlaySound(_blankSelectedSound);
            };
            
            loopData.PuzzleTriggered += puzzle =>
            {
                foreach (var message in puzzle.QuestionMessages)
                {
                    loopData.ReceiveMessage(new DirectMessage(puzzle.Sender, message));
                }
                
                loopData.StartPuzzle(puzzle);
            };

            loopData.CommentsModalRequested += post =>
            {
                if (_currentPage.GetTopModal() is CommentsModalController existingCommentsModal)
                {
                    if (existingCommentsModal.CachedPost == post)
                    {
                        // don't open comments for a modal we're already in
                        return;
                    }
                }

                var commentsModal = _currentPage.OpenModal(_commentsPrefab);
                SoundService.Instance.PlaySound(_openModalSound);

                if (commentsModal != null)
                {
                    commentsModal.PopulateForPost(post);
                }
            };

            loopData.ProfileModalRequested += account =>
            {
                if (_currentPage.GetTopModal() is ProfileModalController existingProfileModal)
                {
                    if (existingProfileModal.CachedAccount == account)
                    {
                        // don't open the profile we're currently looking at
                        return;
                    }
                }

                var profileModal = _currentPage.OpenModal(_profilePrefab);
                SoundService.Instance.PlaySound(_openModalSound);
                if (profileModal != null)
                {
                    profileModal.PopulateWithProfile(account);
                }
            };

            loopData.ConversationModalRequested += account =>
            {
                if (_currentPage.GetTopModal() is ConversationModalController existingModal)
                {
                    if (existingModal.Sender == account)
                    {
                        // don't open the profile we're currently looking at
                        return;
                    }
                }

                var profileModal = _currentPage.OpenModal(_conversationPrefab);
                SoundService.Instance.PlaySound(_openModalSound);
                if (profileModal != null)
                {
                    profileModal.PopulateWithProfile(account);
                }
            };
        }

        private IEnumerator Ending(LoopData loopData)
        {
            var account = new Account(new AccountData()
            {
                Bio = "Programmer",
                DisplayName = "Potatoes Are Not Explosive",
                FollowerCountMagnitude = 3,
                UserName = "notexplosive",
                ProfilePicture = "pfp_notexplosive"
            });
                
            loopData.ReceiveMessage(new DirectMessage(account, "Thanks for playing!"));
            yield return new WaitForSeconds(0.5f);
            loopData.ReceiveMessage(new DirectMessage(account, "This game was made in 4 days for GMTK Jam 2025"));
            yield return new WaitForSeconds(0.5f);
            loopData.ReceiveMessage(new DirectMessage(account, "Game Design by NotExplosive and Tesseralis"));
            yield return new WaitForSeconds(0.5f);
            loopData.ReceiveMessage(new DirectMessage(account, "Art by SonderingEmily KaeOnline, Tesseralis"));
            yield return new WaitForSeconds(0.5f);
            loopData.ReceiveMessage(new DirectMessage(account, "Additional profile picture art by NotExplosive and isoymetric"));
            yield return new WaitForSeconds(0.5f);
            loopData.ReceiveMessage(new DirectMessage(account, "Music and Sound Design by Quarkimo"));
            yield return new WaitForSeconds(0.5f);
            loopData.ReceiveMessage(new DirectMessage(account, "Programming by NotExplosive"));
            yield return new WaitForSeconds(1f);
            loopData.ReceiveMessage(new DirectMessage(account,
                "If you're enjoying your time here, feel free to keep exploring. There's lots more to see!"));
        }

        private Action CreatePageToEvent(AppPage page)
        {
            return () => { PageTo(page); };
        }

        private void PageTo(AppPage page)
        {
            if (_currentPage == page)
            {
                _currentPage.CloseAllModals();
                return;
            }

            if (_currentPage == null)
            {
                _currentPage = page;
                page.FlyIn(1);
                return;
            }

            var targetPageIndex = _pages.IndexOf(page);
            var currentPageIndex = _pages.IndexOf(_currentPage);
            var direction = Math.Sign(targetPageIndex - currentPageIndex);

            SoundService.Instance.PlaySound(_swipeOver);

            _currentPage.FlyOut(direction);
            page.FlyIn(direction);
            _currentPage = page;

            if (_relay != null)
            {
                _relay.State().CurrentPage = page.PageType;
            }
        }
    }
}