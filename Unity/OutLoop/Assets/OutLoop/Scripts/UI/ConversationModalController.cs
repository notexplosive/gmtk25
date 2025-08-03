using System;
using System.Collections;
using System.Linq;
using OutLoop.Core;
using OutLoop.Data;
using SecretPlan.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class ConversationModalController : ModalController
    {
        [SerializeField]
        private AudioClip? _puzzleSolvedSound;
        
        [SerializeField]
        private AudioClip? _puzzleAlmostSolvedSound;
        
        [SerializeField]
        private AudioClip? _puzzleWrongSound;
        
        [SerializeField]
        private LoopDataRelay? _relay;

        [SerializeField]
        private Image? _profilePicture;

        [SerializeField]
        private TextMeshWithHyperlinks? _usernameTextMesh;

        [SerializeField]
        private RectTransform? _contentRoot;

        [SerializeField]
        private DirectMessageController? _directMessageFromSenderPrefab;

        [SerializeField]
        private DirectMessageController? _directMessageFromMePrefab;

        [SerializeField]
        private ScrollRect? _scrollRect;

        [SerializeField]
        private PuzzlePhraseTextController? _puzzlePhraseTextController;

        [SerializeField]
        private TextMeshProUGUI? _hintTextMesh;

        [SerializeField]
        private Transform? _hintArea;

        [SerializeField]
        private Transform? _puzzleRoot;

        public Account? Sender { get; private set; }

        private void OnEnable()
        {
            if (_hintArea != null)
            {
                _hintArea.gameObject.SetActive(false);
            }
            
            
        }

        private void OnDisable()
        {
            if (_relay == null)
            {
                return;
            }
            _relay.State().ClearPendingBlank();
        }

        private void OnDestroy()
        {
            if (_relay == null)
            {
                return;
            }

            _relay.State().PuzzleStarted -= OnPuzzleStarted;
            _relay.State().BlankFilled -= OnBlankFilled;
            _relay.State().MessageReceived -= OnMessageReceived;
        }


        public void PopulateWithProfile(Account account)
        {
            Sender = account;

            if (_contentRoot == null)
            {
                return;
            }

            if (_relay == null)
            {
                return;
            }

            if (_profilePicture != null)
            {
                _profilePicture.sprite = account.ProfilePicture.ForceLoadNow(this);
            }

            if (_usernameTextMesh != null)
            {
                _usernameTextMesh.SetText(account.UserNameWithAt);
            }

            for (var i = 0; i < _contentRoot.childCount; i++)
            {
                Destroy(_contentRoot.GetChild(i).gameObject);
            }

            foreach (var message in _relay.State().GetDmConversationWith(account))
            {
                if (_directMessageFromSenderPrefab != null)
                {
                    SpawnMessage(message);
                }
            }


            // scroll to bottom
            IEnumerator WaitAFrameAndThenScroll()
            {
                Canvas.ForceUpdateCanvases();
                yield return new WaitForEndOfFrame();
                Canvas.ForceUpdateCanvases();
                yield return new WaitForEndOfFrame();
                if (_scrollRect != null)
                {
                    _scrollRect.verticalNormalizedPosition = 0f;
                }
            }

            StartCoroutine(WaitAFrameAndThenScroll());

            ShowCurrentPuzzleIfApplicable();
            _relay.State().PuzzleStarted += OnPuzzleStarted;
            _relay.State().BlankFilled += OnBlankFilled;
            _relay.State().MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(DirectMessage message)
        {
            if (message.Sender == Sender)
            {
                SpawnMessage(message);
            }
        }

        private void SpawnMessage(DirectMessage message)
        {
            if (_directMessageFromSenderPrefab == null)
            {
                return;
            }

            if (_relay == null)
            {
                return;
            }

            var instance = SpawnUtility.Spawn(_directMessageFromSenderPrefab,
                new InstantiateParameters { parent = _contentRoot });
            instance.Populate(message);

            _relay.State().MarkMessageAsRead(message);
        }

        private void OnBlankFilled(Puzzle puzzle, AnswerType answerType)
        {
            CheckSolution(puzzle);
        }

        private void CheckSolution(Puzzle puzzle)
        {
            if (_relay == null)
            {
                return;
            }

            if (!puzzle.AllBlanksFilled())
            {
                return;
            }
            
            if (puzzle.NumberIncorrect() <= 2)
            {
                SetHintText("<color=yellow>Close!</color> 2 or fewer answers incorrect.");
                SoundService.Instance.PlaySound(_puzzleAlmostSolvedSound);
            }
            else
            {
                SetHintText("<color=yellow>Incorrect!</color> Try again.");
                SoundService.Instance.PlaySound(_puzzleWrongSound);
            }

            if (puzzle.IsSolved())
            {
                SetHintText("<color=#75FF82>Solved!</color>");
                SoundService.Instance.PlaySound(_puzzleSolvedSound);
            }
        }

        private void SetHintText(string text)
        {
            if (_hintArea == null)
            {
                return;
            }

            if (_hintTextMesh == null)
            {
                return;
            }

            _hintArea.gameObject.SetActive(true);
            _hintTextMesh.text = text;
        }

        private void OnPuzzleStarted(Puzzle puzzle)
        {
            if (puzzle.Sender == Sender)
            {
                ShowCurrentPuzzleIfApplicable();
            }
        }

        private void ShowCurrentPuzzleIfApplicable()
        {
            if (_relay == null)
            {
                return;
            }

            if (_puzzleRoot == null)
            {
                return;
            }

            var currentPuzzle = _relay.State().InProgressPuzzles.FirstOrDefault(a => a.Sender == Sender);
            if (_puzzlePhraseTextController == null)
            {
                return;
            }

            if (currentPuzzle == null)
            {
                _puzzleRoot.gameObject.SetActive(false);
                return;
            }

            _puzzleRoot.gameObject.SetActive(true);
            _puzzlePhraseTextController.SetupForPuzzle(currentPuzzle);
            CheckSolution(currentPuzzle);
        }
    }
}