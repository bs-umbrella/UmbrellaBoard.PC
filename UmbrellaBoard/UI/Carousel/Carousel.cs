using BeatSaberMarkupLanguage.Attributes;
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
        internal GameObject _bubblePrefab;

        internal VerticalLayoutGroup _carouselLayoutGroup;
        internal LayoutElement _carouselLayoutElement;

        internal Transform _ticker;
        internal HorizontalLayoutGroup _tickerLayoutGroup;
        internal ContentSizeFitter _tickerSizeFitter;
        internal LayoutElement _tickerLayoutElement;

        internal RectTransform _content;
        internal HorizontalLayoutGroup _contentLayoutGroup;
        internal ContentSizeFitter _contentSizeFitter;

        internal RectTransform _viewPort;
        internal HoverDetection _viewPortHoverDetection;

        internal List<CarouselBubble> _carouselBubbles;
        internal List<CanvasGroup> _carouselCanvasGroups;

        internal event Action<Carousel, int> ActiveChildChanged;
        internal int CurrentChildIndex { get; set; }
        internal CarouselDirection Direction { get; set; }
        internal CarouselLocation Location { get; set; }
        internal CarouselTimerBehaviour TimerBehaviour { get; set; }
        internal CarouselAlignment Alignment { get; set; }
        internal float TimerLength { get; set; }
        internal bool ShowButtons { get; set; }
        internal bool PauseOnHover { get; set; }
        internal float InactiveAlpha { get; set; }
        private bool TimerPassed
        {
            get => _timerLength >= 0 && _timer > _timerLength;
        }

        private int _currentChildIndex;
        private int _movingDirection;
        private bool _isAnimating;
        private bool _startRealignNextFrame;


        private CarouselDirection _carouselDirection;
        private CarouselLocation _carouselLocation;
        private CarouselTimerBehaviour _carouselTimerBehaviour;
        private CarouselAlignment _carouselAlignment;
        private float _timerLength;
        private float _timer;
        private bool _showButtons;
        private bool _pauseOnHover;
        private bool _beingHovered;
        private float _inactiveAlpha;

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
                _carouselCanvasGroups[i].alpha = isActiveGroup ? 1.0f : _inactiveAlpha;
                _carouselCanvasGroups[i].interactable = isActiveGroup;
            }
        }


        public void Update()
        {
            if (_isAnimating)
                return;

            if (!_pauseOnHover || !_beingHovered)
                _timer += Time.deltaTime;
            if (TimerPassed)
                AdvanceWithTimer();
            if (_startRealignNextFrame)
            {
                _startRealignNextFrame = false;
                StartCoroutine(GotoChild(_currentChildIndex, true));
            }
        }

        [UIAction("next")]
        private bool NextItem() => Next(true);

        [UIAction("previous")]
        private bool PreviousItem() => Previous();

        private void Skip() => AdvanceWithTimer();

        private bool Next(bool animated = true)
        {
            if (_isAnimating) return false;
            int nextChild = ClampedWithTimerBehaviour(_currentChildIndex + 1);
            if (nextChild == _currentChildIndex) return false;

            StartCoroutine(GotoChild(nextChild, animated));
            return true;
        }

        private bool Previous(bool animated = true)
        {
            if (_isAnimating)
                return false;
            int nextChild = ClampedWithTimerBehaviour(_currentChildIndex - 1);
            if (nextChild == _currentChildIndex)
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
            _movingDirection = 1;
            _carouselBubbles = new List<CarouselBubble>();
            _carouselCanvasGroups = new List<CanvasGroup>();
            _timerLength = 5.0f;
            _inactiveAlpha = 0.2f;
            _carouselAlignment = CarouselAlignment.Center;
        }

        internal void SetupAfterChildren()
        {
            int childCount = _content.childCount;
            Transform parent = _bubblePrefab.transform.parent;
            _bubblePrefab.AddComponent<CarouselBubble>();
            
            for (int i = 0; i < childCount; i++)
            {
                GameObject bubbleGO = Instantiate(_bubblePrefab, parent);
                bubbleGO.transform.localScale = new Vector3(.7f, .7f, .7f);
                CarouselBubble bubble = bubbleGO.GetComponent<CarouselBubble>();
                _carouselBubbles.Add(bubble);

                Transform child = _content.GetChild(i);
                _carouselCanvasGroups.Add(child.gameObject.AddComponent<CanvasGroup>());
                bubbleGO.SetActive(true);
            }

            _nextButton.transform.SetAsLastSibling();

            UpdateViewport();
            StartCoroutine(GotoChild(_currentChildIndex, false));
        }

        private void AdvanceWithTimer()
        {
            int nextChild = 0;
            switch (_carouselTimerBehaviour)
            {
                case CarouselTimerBehaviour.None: return; // don't do anything
                case CarouselTimerBehaviour.LoopForward:
                    nextChild = ClampedWithTimerBehaviour(_currentChildIndex + 1);
                    break;
                case CarouselTimerBehaviour.LoopBackward:
                    nextChild = ClampedWithTimerBehaviour(_currentChildIndex - 1);
                    break;
                case CarouselTimerBehaviour.PingPong:
                    nextChild = ClampedWithTimerBehaviour(_currentChildIndex + _movingDirection);
                    break;
            }
            StartCoroutine(GotoChild(nextChild, true));
        }


        internal void MoveTicker(CarouselLocation location, CarouselDirection direction, bool force)
        {
            // no movement needed, was already in that place
            if (!force && location == _carouselLocation && direction == _carouselDirection) return;
            // carousel layout is the ticker & viewport
            // ticker layout is the ticker itself

            if (force || location != _carouselLocation)
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

            if (force || direction != _carouselDirection)
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
            _carouselLocation = location;
            _carouselDirection = direction;

            UpdateViewport();
        }


        void UpdateViewport()
        {
            switch (_carouselDirection)
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
            switch (_carouselDirection)
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
            SetButtonDirection(_prevButton, PageButtonDirection.Up);
            SetButtonDirection(_nextButton, PageButtonDirection.Down);
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
            SetButtonDirection(_prevButton, PageButtonDirection.Left);
            SetButtonDirection(_nextButton, PageButtonDirection.Right);
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
            switch (_carouselTimerBehaviour)
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
                            return _currentChildIndex + _movingDirection;
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

            var currentChild = _content.GetChild(_currentChildIndex) as UnityEngine.RectTransform;
            var targetChild = _content.GetChild(childIndex) as UnityEngine.RectTransform;

            var targetPos = targetChild.anchoredPosition;
            var childRect = targetChild.rect;
            var viewPortRect = _viewPort.rect;

            var currentPos = _content.anchoredPosition;

            // setup alignment
            switch (_carouselAlignment)
            {
                case CarouselAlignment.Beginning:
                    { // to get beginning we have to subtract half the childrect sizes
                        targetPos.x -= childRect.width * 0.5f;
                        targetPos.y -= childRect.height * 0.5f;
                    }
                    break;
                case CarouselAlignment.Center:
                    { // to get center we just subtract half the viewport size
                        targetPos.x -= viewPortRect.width * 0.5f;
                        targetPos.y -= viewPortRect.height * 0.5f;
                    }
                    break;
                case CarouselAlignment.End:
                    { // to get end we add have to add half the childrect sizes and subtract the entire viewport rect
                        targetPos.x += childRect.width * 0.5f;
                        targetPos.x -= viewPortRect.width;
                        targetPos.y += childRect.height * 0.5f;
                        targetPos.y -= viewPortRect.height;
                    }
                    break;
            }

            // 0 on the non used axis and flip the other
            switch (_carouselDirection)
            {
                case CarouselDirection.Horizontal:
                    {
                        targetPos.y = 0;
                        targetPos.x = -1;
                    }
                    break;
                case CarouselDirection.Vertical:
                    {
                        targetPos.x = 0;
                        targetPos.y = -1;
                    }
                    break;
            }

            SetActiveBubble(childIndex);
            var oldCanvasGroup = _carouselCanvasGroups[_currentChildIndex];
            var newCanvasGroup = _carouselCanvasGroups[childIndex];

            float oldAlpha = oldCanvasGroup.alpha;
            float newAlpha = newCanvasGroup.alpha;

            // if animated, move it
            if (animated)
            {
                for (var t = 0.0f; t < 1.0f; t += UnityEngine.Time.deltaTime * 5.0f)
                {
                    float eased = eased_t(t);
                    _content.anchoredPosition = lerp(currentPos, targetPos, eased);
                    oldCanvasGroup.alpha = lerp(oldAlpha, _inactiveAlpha, eased);
                    newCanvasGroup.alpha = lerp(newAlpha, 1.0f, eased);
                    yield return null;
                }
            }

            _content.anchoredPosition = targetPos;
            _currentChildIndex = childIndex;
            UpdateButtonsInteractable();
            SetAlphaToGroups(_currentChildIndex);

            _isAnimating = false;
            _timer = 0;
            yield return null;
        }

        void UpdateButtonsInteractable()
        {
            switch (_carouselTimerBehaviour)
            {
                // pingpong and none should stop advancing beyond bounds
                case CarouselTimerBehaviour.PingPong:
                case CarouselTimerBehaviour.None:
                    // if >= count not interactable
                    _nextButton.interactable = _currentChildIndex >= _content.childCount;
                    // if <= 0 not interactable
                    _prevButton.interactable = _currentChildIndex <= 0;
                    break;
                // loop should just allow
                case CarouselTimerBehaviour.LoopBackward:
                case CarouselTimerBehaviour.LoopForward:
                    _nextButton.interactable = true;
                    _prevButton.interactable = true;
                    break;
            }
        }

        void SetButtonDirection(Button pageButton, PageButtonDirection pageButtonDirection)
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

        private float flip(float t) { return 1.0f - t; }
        private float square(float t) { return t * t; }
        private float ease_in(float t) { return square(t); }
        private float ease_out(float t) { return flip(square(flip(t))); }
        private float lerp(float a, float b, float t) { return a + (b - a) * t; }
        private float eased_t(float t) { return lerp(ease_in(t), ease_out(t), t); }
        private Vector2 lerp(Vector2 a, Vector2 b, float t) { return new Vector2(lerp(a.x, b.x, t), lerp(a.y, b.y, t)); }

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

        internal enum PageButtonDirection
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
