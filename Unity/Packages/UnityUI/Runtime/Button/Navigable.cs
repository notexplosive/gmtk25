using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using SecretPlan.Core;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace SecretPlan.UI
{
    /// <summary>
    ///     This borrows from Unity's built-in Selectable with a bunch of code stripped out and fields renamed. Any reference
    ///     to Selectable is replaced with references to this class.
    /// </summary>
    [ExecuteAlways]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Navigable :
        UIBehaviour, IMoveHandler,
        ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        private readonly CachedComponent<NavigationLayer> _navigationLayer = new(ComponentSearchMode.InParent);
        
        [SerializeField]
        private List<ButtonDecorator?> _decorators = new();

        // Navigation information.
        [SerializeField]
        private Navigation _navigation = Navigation.defaultNavigation;

        [SerializeField]
        private bool _isInteractive = true;

        [SerializeField]
        private ButtonSkin? _currentSkin;

        /// <summary>
        ///     Only used for editor tooling
        /// </summary>
        [BoxGroup("Editor Tools")]
        [ShowIf(nameof(IsEditMode))]
        [SerializeField]
        private NavigationState _editorSelectedState;

        private readonly List<CanvasGroup> _canvasGroupCache = new();

        private readonly CachedComponent<UIHitBox> _hitBox = new(ComponentSearchMode.InFirstFoundChild);
        private bool _enableCalled;

        private bool _groupsAllowInteraction = true;
        private NavigationState _previousUpdatedState;
        private bool _wasInteractable;

        public ButtonSkin? Skin
        {
            get => _currentSkin;
            set
            {
                _currentSkin = value;
                UpdateDecorators(false);
            }
        }
        

        public Navigation Navigation
        {
            get => _navigation;
            set
            {
                if (SetPropertyUtility.SetStruct(ref _navigation, value))
                {
                    OnSetProperty();
                }
            }
        }

        public bool IsInteractive
        {
            get => _isInteractive;
            set
            {
                if (SetPropertyUtility.SetStruct(ref _isInteractive, value))
                {
                    if (!_isInteractive && EventSystem.current != null &&
                        EventSystem.current.currentSelectedGameObject == gameObject)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }

                    OnSetProperty();
                }
            }
        }

        private bool IsPointerInside { get; set; }
        private bool IsPointerDown { get; set; }
        private bool HasSelection { get; set; }
        private bool HasSubmit { get; set; }

        protected NavigationState CurrentNavigationState
        {
            get
            {
                if (!IsInteractable())
                {
                    return NavigationState.Disabled;
                }

                if (IsPointerDown || HasSubmit)
                {
                    return NavigationState.Pressed;
                }

                if (IsPointerInside && InputModeDispatcher.Instance.CurrentInputMode == InputMode.Mouse)
                {
                    return NavigationState.NavigatedTo;
                }

                if (HasSelection && InputModeDispatcher.Instance.CurrentInputMode == InputMode.Directional)
                {
                    return NavigationState.NavigatedTo;
                }

                return NavigationState.Normal;
            }
        }

        private void Update()
        {
            var isInteractable = IsInteractable();
            if (isInteractable != _wasInteractable)
            {
                UpdateDecorators(false);
            }

            _wasInteractable = isInteractable;
        }

        // Select on enable and add to the list.
        protected override void OnEnable()
        {
            //Check to avoid multiple OnEnable() calls for each selectable
            if (_enableCalled)
            {
                return;
            }

            base.OnEnable();

            if (Application.isPlaying)
            {
                InputModeDispatcher.Instance.InputModeChanged += OnInputModeChanged;
                _hitBox.GetOrAdd(this).Initialize(this);
            }

            if (_navigationLayer.Exists(this))
            {
                _navigationLayer.Get(this).AddNavigable(this);
            }

            if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                HasSelection = true;
            }

            IsPointerDown = false;
            _groupsAllowInteraction = ParentGroupAllowsInteraction();

            UpdateDecorators(true);

            _enableCalled = true;
        }

        protected override void OnDisable()
        {
            //Check to avoid multiple OnDisable() calls for each selectable
            if (!_enableCalled)
            {
                return;
            }
            
            if (_navigationLayer.Exists(this))
            {
                _navigationLayer.Get(this).RemoveNavigable(this);
            }

            if (Application.isPlaying)
            {
                InputModeDispatcher.Instance.InputModeChanged -= OnInputModeChanged;
            }

            base.OnDisable();

            _enableCalled = false;
        }

        protected override void OnCanvasGroupChanged()
        {
            var parentGroupAllowsInteraction = ParentGroupAllowsInteraction();

            if (parentGroupAllowsInteraction != _groupsAllowInteraction)
            {
                _groupsAllowInteraction = parentGroupAllowsInteraction;
                OnSetProperty();
            }
        }

        // Call from unity if animation properties have changed
        protected override void OnDidApplyAnimationProperties()
        {
            OnSetProperty();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            OnCanvasGroupChanged();
        }

        /// <summary>
        ///     Unset selection and transition to appropriate state.
        /// </summary>
        public void OnDeselect(BaseEventData eventData)
        {
            HasSelection = false;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        ///     Determine in which of the 4 move directions the next selectable object should be found.
        /// </summary>
        public void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    Navigate(eventData, FindNavigableOnRight());
                    break;

                case MoveDirection.Up:
                    Navigate(eventData, FindNavigableOnUp());
                    break;

                case MoveDirection.Left:
                    Navigate(eventData, FindNavigableOnLeft());
                    break;

                case MoveDirection.Down:
                    Navigate(eventData, FindNavigableOnDown());
                    break;
            }
        }

        /// <summary>
        ///     Set selection and transition to appropriate state.
        /// </summary>
        public void OnSelect(BaseEventData eventData)
        {
            HasSelection = true;
            EvaluateAndTransitionToSelectionState();
        }

        public event Action? Submitted;

        public void OnSubmit(BaseEventData eventData)
        {
            HasSubmit = true;
            EvaluateAndTransitionToSelectionState();
            Submitted?.Invoke();
            HasSubmit = false;
        }

        /// <summary>
        ///     Evaluate current state and transition to pressed state.
        /// </summary>
        public void ReceivePointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            // Selection tracking
            if (IsInteractable() && Navigation.mode != Navigation.Mode.None && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);
            }

            IsPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        ///     Evaluate current state and transition to appropriate state.
        ///     New state could be pressed or hover depending on pressed state.
        /// </summary>
        public void ReceivePointerEnter()
        {
            IsPointerInside = true;
            ForceNavigateTo();
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        ///     Evaluate current state and transition to normal state.
        /// </summary>
        public void ReceivePointerExit()
        {
            IsPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        ///     Evaluate eventData and transition to appropriate state.
        /// </summary>
        public void ReceivePointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            IsPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        private bool ParentGroupAllowsInteraction()
        {
            var localTransform = transform;
            while (localTransform != null)
            {
                localTransform.GetComponents(_canvasGroupCache);
                for (var i = 0; i < _canvasGroupCache.Count; i++)
                {
                    if (_canvasGroupCache[i].enabled && !_canvasGroupCache[i].interactable)
                    {
                        return false;
                    }

                    if (_canvasGroupCache[i].ignoreParentGroups)
                    {
                        return true;
                    }
                }

                localTransform = localTransform.parent;
            }

            return true;
        }

        /// <summary>
        ///     Is the object interactable.
        /// </summary>
        public bool IsInteractable()
        {
            return _groupsAllowInteraction && _isInteractive;
        }

        private void OnSetProperty()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            UpdateDecorators(false);
        }

        private void UpdateDecorators(bool instant)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var newState = CurrentNavigationState;

            foreach (var decorator in _decorators)
            {
                if (decorator != null)
                {
                    decorator.OnState(_previousUpdatedState, newState, instant, _currentSkin);
                }
            }

            _previousUpdatedState = newState;
        }

        private void OnInputModeChanged(InputMode inputMode)
        {
            if (IsPointerInside && inputMode == InputMode.Mouse)
            {
                ForceNavigateTo();
            }

            UpdateDecorators(false);
        }

        /// <summary>
        ///     Finds the selectable object next to this one.
        /// </summary>
        public Navigable? FindNavigable(Vector3 dir)
        {
            dir = dir.normalized;
            var localDir = Quaternion.Inverse(transform.rotation) * dir;
            var pos = transform.TransformPoint((transform as RectTransform).GetPointOnRectEdge(localDir));
            var maxScore = Mathf.NegativeInfinity;
            var maxFurthestScore = Mathf.NegativeInfinity;

            var wantsWrapAround = Navigation.wrapAround && (_navigation.mode == Navigation.Mode.Vertical ||
                                                            _navigation.mode == Navigation.Mode.Horizontal);

            Navigable? bestPick = null;
            Navigable? bestFurthestPick = null;

            foreach(var navigable in AllNavigablesInLayer())
            {
                if (navigable == this)
                {
                    continue;
                }

                if (navigable == null)
                {
                    continue;
                }

                if (!navigable.IsInteractable() || navigable.Navigation.mode == Navigation.Mode.None)
                {
                    continue;
                }

#if UNITY_EDITOR
                // Apart from runtime use, FindSelectable is used by custom editors to
                // draw arrows between different selectables. For scene view cameras,
                // only selectables in the same stage should be considered.
                if (Camera.current != null &&
                    !StageUtility.IsGameObjectRenderedByCamera(navigable.gameObject, Camera.current))
                {
                    continue;
                }
#endif

                var selRect = navigable.transform as RectTransform;
                var selCenter = selRect != null ? (Vector3) selRect.rect.center : Vector3.zero;
                var myVector = navigable.transform.TransformPoint(selCenter) - pos;

                // Value that is the distance out along the direction.
                var dot = Vector3.Dot(dir, myVector);

                // If element is in wrong direction and we have wrapAround enabled check and cache it if furthest away.
                float score;
                if (wantsWrapAround && dot < 0)
                {
                    score = -dot * myVector.sqrMagnitude;

                    if (score > maxFurthestScore)
                    {
                        maxFurthestScore = score;
                        bestFurthestPick = navigable;
                    }

                    continue;
                }

                // Skip elements that are in the wrong direction or which have zero distance.
                // This also ensures that the scoring formula below will not have a division by zero error.
                if (dot <= 0)
                {
                    continue;
                }

                // This scoring function has two priorities:
                // - Score higher for positions that are closer.
                // - Score higher for positions that are located in the right direction.
                // This scoring function combines both of these criteria.
                // It can be seen as this:
                //   Dot (dir, myVector.normalized) / myVector.magnitude
                // The first part equals 1 if the direction of myVector is the same as dir, and 0 if it's orthogonal.
                // The second part scores lower the greater the distance is by dividing by the distance.
                // The formula below is equivalent but more optimized.
                //
                // If a given score is chosen, the positions that evaluate to that score will form a circle
                // that touches pos and whose center is located along dir. A way to visualize the resulting functionality is this:
                // From the position pos, blow up a circular balloon so it grows in the direction of dir.
                // The first Selectable whose center the circular balloon touches is the one that's chosen.
                score = dot / myVector.sqrMagnitude;

                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = navigable;
                }
            }

            if (wantsWrapAround && null == bestPick)
            {
                return bestFurthestPick;
            }

            return bestPick;
        }

        private IEnumerable<Navigable?> AllNavigablesInLayer()
        {
            var layer = _navigationLayer.GetOrNull(this);
            if (layer == null)
            {
                Debug.LogWarning($"{gameObject} is not in a Navigation Layer so we couldn't navigate from it");
                yield break;
            }

            foreach (var navigable in layer.AllNavigables())
            {
                yield return navigable;
            }
        }

        private void Navigate(AxisEventData eventData, Navigable? navigable)
        {
            if (navigable != null && navigable.IsActive())
            {
                eventData.selectedObject = navigable.gameObject;
            }
        }

        /// <summary>
        ///     Find the selectable object to the left of this one.
        /// </summary>
        public Navigable? FindNavigableOnLeft()
        {
            if (_navigation.mode == Navigation.Mode.Explicit)
            {
                return _navigation.selectOnLeft;
            }

            if ((_navigation.mode & Navigation.Mode.Horizontal) != 0)
            {
                return FindNavigable(transform.rotation * Vector3.left);
            }

            return null;
        }

        /// <summary>
        ///     Find the selectable object to the right of this one.
        /// </summary>
        public Navigable? FindNavigableOnRight()
        {
            if (_navigation.mode == Navigation.Mode.Explicit)
            {
                return _navigation.selectOnRight;
            }

            if ((_navigation.mode & Navigation.Mode.Horizontal) != 0)
            {
                return FindNavigable(transform.rotation * Vector3.right);
            }

            return null;
        }

        /// <summary>
        ///     The Selectable object above current
        /// </summary>
        public Navigable? FindNavigableOnUp()
        {
            if (_navigation.mode == Navigation.Mode.Explicit)
            {
                return _navigation.selectOnUp;
            }

            if ((_navigation.mode & Navigation.Mode.Vertical) != 0)
            {
                return FindNavigable(transform.rotation * Vector3.up);
            }

            return null;
        }

        /// <summary>
        ///     Find the selectable object below this one.
        /// </summary>
        public Navigable? FindNavigableOnDown()
        {
            if (_navigation.mode == Navigation.Mode.Explicit)
            {
                return _navigation.selectOnDown;
            }

            if ((_navigation.mode & Navigation.Mode.Vertical) != 0)
            {
                return FindNavigable(transform.rotation * Vector3.down);
            }

            return null;
        }

        // Change the button to the correct state
        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !IsInteractable())
            {
                return;
            }

            UpdateDecorators(false);
        }

        public void ForceNavigateTo()
        {
            if (EventSystem.current == null || EventSystem.current.alreadySelecting)
            {
                return;
            }

            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        [UsedImplicitly]
        [Button("Add Decorators")]
        public void AddDecorators()
        {
            _decorators.Clear();
            var decorators = GetComponentsInChildren<ButtonDecorator>(true);
            foreach (var decorator in decorators)
            {
                _decorators.Add(decorator);
            }
        }

        private bool IsEditMode()
        {
            return !Application.isPlaying && SpEditorUtilities.IsRootOfPrefab(transform);
        }

        [ShowIf(nameof(IsEditMode))]
        [UsedImplicitly]
        [Button]
        public void PreviewState()
        {
            foreach (var decorator in _decorators)
            {
                if (decorator != null)
                {
                    decorator.OnState(_editorSelectedState, _editorSelectedState, true, _currentSkin);
                }
            }
        }
    }
}
