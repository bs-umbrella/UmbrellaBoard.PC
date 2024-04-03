using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [UIAction("next")]
        internal bool NextItem() => Next();

        [UIAction("previous")]
        internal bool PreviousItem() => Previous();

        internal void Skip() => AdvanceWithTimer();

        internal bool Next(bool animated = true)
        {
            return false;
        }

        internal bool Previous(bool animated = true)
        {
            return false;
        }

        internal void SetCurrentlyActiveChildIndex(int index, bool animated = true)
        {

        }
        
        internal void Setup()
        {

        }

        internal void SetupAfterChildren()
        {

        }

        private void Update()
        {

        }

        private void OnDestroy()
        {

        }

        private void OnEnable()
        {

        }

        private void AdvanceWithTimer()
        {

        }

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
}
