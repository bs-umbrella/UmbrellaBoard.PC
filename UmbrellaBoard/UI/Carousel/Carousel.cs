﻿using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace UmbrellaBoard.UI.Carousel
{
    internal class Carousel : MonoBehaviour
    {
        [UIComponent("next-button")]
        internal Button _nextButton;
        [UIComponent("prev-button")]
        internal Button _prevButton;

        [UIComponent("bubble-prefab")]
        internal Transform _bubblePrefab;

        internal VerticalLayoutGroup _carouselLayoutGroup;
        internal LayoutElement _carouselLayoutElement;

        [UIComponent("ticker")]
        internal Transform _ticker;
        internal HorizontalLayoutGroup _tickerLayoutGroup;
        internal ContentSizeFitter _tickerSizeFitter;
        internal LayoutElement _tickerLayoutElement;

        internal RectTransform _content;
        internal HorizontalLayoutGroup _contentLayoutGroup;
        internal ContentSizeFitter _contentSizeFitter;

        internal RectTransform _viewPort;
        internal HoverDetection _viewPortHoverDetection;

        internal List<CarouselBubble> _carouselBubbles = new();
        internal List<CanvasGroup> _carouselCanvasGroups = new();

        internal event Action<Carousel, int> ActiveChildChanged;
        private int _currentChildIndex = 0;
        internal int CurrentChildIndex 
        {
            get => _currentChildIndex;
            set => SetCurrentlyActiveChildIndex(value, false);
        }
        private CarouselDirection _carouselDirection = CarouselDirection.Horizontal;
        internal CarouselDirection Direction 
        {
            get => _carouselDirection;
            set => MoveTicker(Location, value);
        } 
        private CarouselLocation _carouselLocation = CarouselLocation.Default;
        internal CarouselLocation Location 
        {
            get => _carouselLocation;
            set => MoveTicker(value, Direction);
        }
        private CarouselTimerBehaviour _carouselTimerBehaviour = CarouselTimerBehaviour.None;
        internal CarouselTimerBehaviour TimerBehaviour
        {
            get => _carouselTimerBehaviour;
            set
            {
                _carouselTimerBehaviour = value;
                UpdateButtonsInteractable();
            }
        }
        private CarouselAlignment _carouselAlignment = CarouselAlignment.Center;
        internal CarouselAlignment Alignment 
        {
            get => _carouselAlignment;
            set
            {
                _carouselAlignment = value;
                StartCoroutine(GotoChild(CurrentChildIndex, false));
            }
        }
        internal float TimerLength { get; set; } = 5.0f;
        private bool _showButtons = true;
        internal bool ShowButtons
        {
            get => _showButtons;
            set
            {
                _showButtons = value;
                _nextButton?.gameObject.SetActive(_showButtons);
                _prevButton?.gameObject.SetActive(_showButtons);
            }
        }

        internal bool PauseOnHover { get; set; } = false;
        private float _inactiveAlpha = 0.2f;
        internal float InactiveAlpha 
        {
            get => _inactiveAlpha;
            set
            {
                _inactiveAlpha = value;
                SetAlphaToGroups(CurrentChildIndex);
            }
        }
        private bool TimerPassed => TimerLength >= 0 && _timer > TimerLength;

        private int _movingDirection = 1;
        private bool _isAnimating = false;
        private bool _startRealignNextFrame = true;

        private float _timer;
        private bool _beingHovered;

        public void OnEnable() => _startRealignNextFrame = true;

        public void OnDestroy()
        {
            if (_viewPortHoverDetection)
            {
                _viewPortHoverDetection.Enter -= OnPointerEnterViewport;
                _viewPortHoverDetection.Exit -= OnPointerExitViewport;
            }
        }

        void OnPointerEnterViewport() => SetIsHovered(true);

        void OnPointerExitViewport() => SetIsHovered(false);

        void SetIsHovered(bool isHovered) => _beingHovered = isHovered;

        void SetActiveBubble(int index)
        {
            for (int i = 0; i < _carouselBubbles.Count; i++)
                _carouselBubbles[i].Highlighted = index == i;
        }

        void SetAlphaToGroups(int index)
        {
            for (int i = 0; i < _carouselCanvasGroups.Count; i++)
            {
                bool isActiveGroup = index == i;
                _carouselCanvasGroups[i].alpha = isActiveGroup ? 1.0f : InactiveAlpha;
                _carouselCanvasGroups[i].interactable = isActiveGroup;
            }
        }


        public void Update()
        {
            if (_isAnimating)
                return;

            if (!PauseOnHover|| !_beingHovered)
                _timer += Time.deltaTime;
            if (TimerPassed)
                AdvanceWithTimer();
            if (_startRealignNextFrame)
            {
                _startRealignNextFrame = false;
                StartCoroutine(GotoChild(CurrentChildIndex, true));
            }
        }

        [UIAction("next")]
        private bool NextItem() => Next(true);

        [UIAction("previous")]
        private bool PreviousItem() => Previous(true);

        private void Skip() => AdvanceWithTimer();

        private bool Next(bool animated = true)
        {
            if (_isAnimating) return false;
            int nextChild = ClampedWithTimerBehaviour(CurrentChildIndex + 1);
            if (nextChild == CurrentChildIndex) return false;

            StartCoroutine(GotoChild(nextChild, animated));
            return true;
        }

        private bool Previous(bool animated = true)
        {
            if (_isAnimating)
                return false;
            int nextChild = ClampedWithTimerBehaviour(CurrentChildIndex - 1);
            if (nextChild == CurrentChildIndex)
                return false;

            StartCoroutine(GotoChild(nextChild, animated));
            return true;
        }

        internal void SetCurrentlyActiveChildIndex(int index, bool animated = true)
        {
            if (_isAnimating) return;
            StartCoroutine(GotoChild(index, animated));
        }

        internal void Setup()
        {
            _viewPortHoverDetection = _viewPort.gameObject.AddComponent<HoverDetection>();
            _viewPortHoverDetection.Enter += OnPointerEnterViewport;
            _viewPortHoverDetection.Exit += OnPointerExitViewport;
        }

        internal void SetupAfterChildren()
        {
            int childCount = _content.childCount;
            Transform parent = _bubblePrefab.transform.parent;
            _bubblePrefab.gameObject.AddComponent<CarouselBubble>();
            
            for (int i = 0; i < childCount; i++)
            {
                GameObject bubbleGO = Instantiate(_bubblePrefab.gameObject, parent);
                bubbleGO.transform.localScale = new Vector3(.7f, .7f, .7f);
                CarouselBubble bubble = bubbleGO.GetComponent<CarouselBubble>();
                _carouselBubbles.Add(bubble);

                Transform child = _content.GetChild(i);
                _carouselCanvasGroups.Add(child.gameObject.AddComponent<CanvasGroup>());
                bubbleGO.SetActive(true);
            }

            _nextButton.transform.SetAsLastSibling();

            UpdateViewport();
            StartCoroutine(GotoChild(CurrentChildIndex, false));
        }

        private void AdvanceWithTimer()
        {
            int nextChild = 0;
            switch (TimerBehaviour)
            {
                case CarouselTimerBehaviour.None: return; // don't do anything
                case CarouselTimerBehaviour.LoopForward:
                    nextChild = ClampedWithTimerBehaviour(CurrentChildIndex + 1);
                    break;
                case CarouselTimerBehaviour.LoopBackward:
                    nextChild = ClampedWithTimerBehaviour(CurrentChildIndex - 1);
                    break;
                case CarouselTimerBehaviour.PingPong:
                    nextChild = ClampedWithTimerBehaviour(CurrentChildIndex + _movingDirection);
                    break;
            }
            StartCoroutine(GotoChild(nextChild, true));
        }


        internal void MoveTicker(CarouselLocation location, CarouselDirection direction, bool force = false)
        {
            // no movement needed, was already in that place
            if (!force && location == Location && direction == Direction) return;
            // carousel layout is the ticker & viewport
            // ticker layout is the ticker itself

            if (force || location != Location)
            {
                switch (location)
                {
                    case CarouselLocation.Bottom:
                        SetTickerHorizontal();
                        _ticker.SetAsLastSibling();
                        break;
                    case CarouselLocation.Top:
                        SetTickerHorizontal();
                        _ticker.SetAsFirstSibling();
                        break;
                    case CarouselLocation.Left:
                        SetTickerVertical();
                        _ticker.SetAsFirstSibling();
                        break;
                    case CarouselLocation.Right:
                        SetTickerVertical();
                        _ticker.SetAsFirstSibling();
                        break;
                    case CarouselLocation.Default:
                        switch (direction)
                        {
                            case CarouselDirection.Horizontal:
                                SetTickerHorizontal();
                                _ticker.SetAsLastSibling();
                                break;
                            case CarouselDirection.Vertical:
                                SetTickerVertical();
                                _ticker.SetAsFirstSibling();
                                break;
                        }
                        break;
                }
            }

            if (force || direction != Direction)
            {
                switch (direction)
                {
                    case CarouselDirection.Horizontal:
                        SetContentHorizontal();
                        break;
                    case CarouselDirection.Vertical:
                        SetContentVertical();
                        break;
                }
            }

            // update variables
            Location = location;
            Direction = direction;

            UpdateViewport();
        }


        void UpdateViewport()
        {
            switch (Direction)
            {
                case CarouselDirection.Horizontal:
                    // set size to rect width for horizontal
                    SetContentSize(_content.rect.width);
                    break;
                case CarouselDirection.Vertical:
                    // set size to rect height for vertical
                    SetContentSize(_content.rect.height);
                    break;
            }
        }

        void SetContentSize(float size)
        {
            switch (Direction)
            {
                case CarouselDirection.Horizontal:
                    _content.sizeDelta = new Vector2(size, 0);
                    break;
                case CarouselDirection.Vertical:
                    _content.sizeDelta = new Vector2(0, size);
                    break;
            }
        }

        void SetTickerVertical()
        {
            // a vertical ticker means content & ticker are side by side
            _carouselLayoutGroup.SetLayoutHorizontal();
            // turn ticker vertical
            _tickerLayoutGroup.SetLayoutVertical();

            // set height & width for ticker layout
            _tickerLayoutElement.preferredHeight = -1;
            _tickerLayoutElement.preferredWidth = 8;

            // update fit modes
            _tickerSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            _tickerSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            // rotate buttons
            _prevButton.SetButtonDirection(SetButtonDirectionExtension.PageButtonDirection.Up);
            _nextButton.SetButtonDirection(SetButtonDirectionExtension.PageButtonDirection.Down);
        }

        void SetTickerHorizontal()
        {
            // a horizontal ticker means content & ticker are over/under
            _carouselLayoutGroup.SetLayoutVertical();
            // turn ticker horizontal
            _tickerLayoutGroup.SetLayoutHorizontal();

            // set height & width for ticker layout
            _tickerLayoutElement.preferredHeight = 8;
            _tickerLayoutElement.preferredWidth = -1;

            // update fit modes
            _tickerSizeFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            _tickerSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            // rotate buttons
            _prevButton.SetButtonDirection(SetButtonDirectionExtension.PageButtonDirection.Left);
            _nextButton.SetButtonDirection(SetButtonDirectionExtension.PageButtonDirection.Right);
        }

        void SetContentVertical()
        {
            // set content vertical
            _contentLayoutGroup.SetLayoutVertical();
            // the content direction should be preferred, the other unconstrained
            _contentSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            _contentSizeFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;

            // update anchor stuff for vertical
            _content.anchorMin = new Vector2(0, 1);
            _content.anchorMax = new Vector2(1, 1);
            _content.pivot = new Vector2(0.5f, 1);
        }

        void SetContentHorizontal()
        {
            // set content horizontal
            _contentLayoutGroup.SetLayoutHorizontal();
            // the content direction should be preferred, the other unconstrained
            _contentSizeFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            _contentSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;

            // update anchor stuff for horizontal
            _content.anchorMin = new Vector2(0, 0);
            _content.anchorMax = new Vector2(0, 1);
            _content.pivot = new Vector2(0, 0.5f);
        }

        int ClampedWithTimerBehaviour(int index)
        {
            switch (TimerBehaviour)
            {
                case CarouselTimerBehaviour.None:
                    {
                        if (index >= _content.childCount) return _content.childCount - 1;
                        if (index < 0) return 0;
                    }
                    break;
                // both loop kinds need to clamp the same way
                case CarouselTimerBehaviour.LoopForward:
                    {
                        goto case CarouselTimerBehaviour.LoopBackward;
                    }

                case CarouselTimerBehaviour.LoopBackward:
                    {
                        if (index >= _content.childCount) return 0;
                        if (index < 0) return _content.childCount - 1;
                    }
                    break;
                case CarouselTimerBehaviour.PingPong:
                    {
                        if (index >= _content.childCount || index < 0)
                        {
                            // reverse dir
                            _movingDirection = -_movingDirection;
                            return CurrentChildIndex + _movingDirection;
                        }
                    }
                    break;
            }
            return index;
        }

        internal IEnumerator GotoChild(int childIndex, bool animated)
        {
            if (_isAnimating) yield return null;
            if (childIndex >= _content.childCount || childIndex < 0) yield return null;
            _isAnimating = true;

            var targetChild = _content.GetChild(childIndex) as RectTransform;

            var targetPos = targetChild.anchoredPosition;
            var childRect = targetChild.rect;
            var viewPortRect = _viewPort.rect;

            var currentPos = _content.anchoredPosition;

            // setup alignment
            switch (Alignment)
            {
                case CarouselAlignment.Beginning:
                    // to get beginning we have to subtract half the childrect sizes
                    targetPos -= childRect.size * 0.5f;
                    break;
                case CarouselAlignment.Center:
                    // to get center we just subtract half the viewport size
                    targetPos -= viewPortRect.size * 0.5f;
                    break;
                case CarouselAlignment.End:
                    // to get end we add have to add half the childrect sizes and subtract the entire viewport rect
                    targetPos += childRect.size * 0.5f;
                    targetPos -= viewPortRect.size;
                    break;
            }

            // 0 on the non used axis and flip the other
            switch (Direction)
            {
                case CarouselDirection.Horizontal:
                    targetPos *= new Vector2(-1, 0);
                    break;
                case CarouselDirection.Vertical:
                    targetPos *= new Vector2(0, -1);
                    break;
            }

            SetActiveBubble(childIndex);
            var oldCanvasGroup = _carouselCanvasGroups[CurrentChildIndex];
            var newCanvasGroup = _carouselCanvasGroups[childIndex];

            float oldAlpha = oldCanvasGroup.alpha;
            float newAlpha = newCanvasGroup.alpha;

            // if animated, move it
            if (animated)
            {
                for (var t = 0.0f; t < 1.0f; t += Time.deltaTime * 5.0f)
                {
                    float eased = eased_t(t);
                    _content.anchoredPosition = Vector2.Lerp(currentPos, targetPos, eased);
                    oldCanvasGroup.alpha = Mathf.Lerp(oldAlpha, InactiveAlpha, eased);
                    newCanvasGroup.alpha = Mathf.Lerp(newAlpha, 1.0f, eased);
                    yield return null;
                }
            }

            _content.anchoredPosition = targetPos;
            _currentChildIndex = childIndex;
            UpdateButtonsInteractable();
            SetAlphaToGroups(CurrentChildIndex);

            _isAnimating = false;
            _timer = 0;
        }

        void UpdateButtonsInteractable()
        {
            switch (TimerBehaviour)
            {
                // pingpong and none should stop advancing beyond bounds
                case CarouselTimerBehaviour.PingPong:
                case CarouselTimerBehaviour.None:
                    // if child index smaller than count - 1, interactable
                    _nextButton.interactable = CurrentChildIndex < (_content.childCount - 1);
                    // if child index greater than 0 (and we have at least 1 child), interactable
                    _prevButton.interactable = CurrentChildIndex > 0 && _content.childCount > 0;
                    break;
                // loop should just allow
                case CarouselTimerBehaviour.LoopBackward:
                case CarouselTimerBehaviour.LoopForward:
                    _nextButton.interactable = true;
                    _prevButton.interactable = true;
                    break;
            }
        }
        
        private float flip(float t) { return 1.0f - t; }
        private float square(float t) { return t * t; }
        private float ease_in(float t) { return square(t); }
        private float ease_out(float t) { return flip(square(flip(t))); }
        private float lerp(float a, float b, float t) { return a + (b - a) * t; }
        private float eased_t(float t) { return lerp(ease_in(t), ease_out(t), t); }

        internal enum CarouselTimerBehaviour
        {
            None,
            PingPong,
            Loop,
            LoopForward = Loop,
            LoopBackward
        }

        internal enum CarouselDirection
        {
            Horizontal,
            Vertical
        }

        internal enum CarouselLocation
        {
            Default,
            Bottom,
            Top,
            Left,
            Right
        }

        internal enum CarouselAlignment
        {
            Beginning,
            Center,
            Middle = Center,
            End
        }
    }

    static internal class SetButtonDirectionExtension
    {
        internal enum PageButtonDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        static public void SetButtonDirection(this Button pageButton, PageButtonDirection pageButtonDirection)
        {
            bool isHorizontal = false;
            int angle = 0;
            var buttonTransform = pageButton.transform.Find("Icon") as RectTransform;
            buttonTransform.anchoredPosition = new Vector2(0, 0);
            var layoutElement = pageButton.GetComponent<LayoutElement>();
            switch (pageButtonDirection)
            {
                case PageButtonDirection.Up:
                    isHorizontal = true;
                    angle = -180;
                    break;
                case PageButtonDirection.Down:
                    isHorizontal = true;
                    angle = 0;
                    break;
                case PageButtonDirection.Left:
                    isHorizontal = false;
                    angle = -90;
                    break;
                case PageButtonDirection.Right:
                    isHorizontal = false;
                    angle = 90;
                    break;
            }
            buttonTransform.localRotation = Quaternion.Euler(0, 0, angle);
            if (layoutElement.preferredHeight == -1)
                layoutElement.preferredHeight = (isHorizontal ? 8 : 8);

            if (layoutElement.preferredHeight == -1)
                layoutElement.preferredHeight = (isHorizontal ? 8 : 8);
        }
    }
}
