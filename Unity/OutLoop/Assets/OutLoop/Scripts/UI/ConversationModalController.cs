using System.Collections;
using System.Linq;
using OutLoop.Core;
using SecretPlan.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class ConversationModalController : ModalController
    {
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

        public Account? Sender { get; private set; }

        private void OnEnable()
        {
            if (_hintArea != null)
            {
                _hintArea.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_relay == null)
            {
                return;
            }

            _relay.State().PuzzleStarted -= OnPuzzleStarted;
            _relay.State().BlankFilled -= OnBlankFilled;
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
                    var instance = SpawnUtility.Spawn(_directMessageFromSenderPrefab,
                        new InstantiateParameters { parent = _contentRoot });
                    instance.Populate(message);

                    _relay.State().MarkMessageAsRead(message);
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
        }

        private void OnBlankFilled(Puzzle puzzle)
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
            }
            else
            {
                SetHintText("<color=red>Incorrect!</color> Try again.");
            }

            if (puzzle.IsSolved())
            {
                SetHintText("<color=#75FF82>Solved!</color>");
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

            var currentPuzzle = _relay.State().InProgressPuzzles.FirstOrDefault(a => a.Sender == Sender);
            if (_puzzlePhraseTextController == null)
            {
                return;
            }

            if (currentPuzzle == null)
            {
                return;
            }

            _puzzlePhraseTextController.SetupForPuzzle(currentPuzzle);
            CheckSolution(currentPuzzle);
        }
    }
}