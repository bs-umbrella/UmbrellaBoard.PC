using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UmbrellaBoard.UI.Carousel
{
    internal class Carousel : MonoBehaviour
    {
        internal Button _nextButton;
        internal Button _prevButton;
        internal VerticalLayoutGroup _carouselLayoutGroup;
        internal LayoutElement _carouselLayoutElement;
        internal HorizontalLayoutGroup _tickerLayoutGroup;
        internal ContentSizeFitter _tickerSizeFitter;
        internal HorizontalLayoutGroup _contentLayoutGroup;
        internal ContentSizeFitter _contentSizeFitter;
        internal Transform _ticker;
        internal GameObject _bubblePrefab;
        internal List<CarouselBubble> _carouselBubbles;
        internal List<CanvasGroup> _carouselCanvasGroups;
        internal RectTransform _content;
        internal RectTransform _viewPort;
        internal HoverDetection _viewPortHoverDetection;

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

        internal bool NextItem()
        {

        }

        internal bool PreviousItem()
        {

        }

        internal void Skip()
        {

        }
        internal bool Next(bool animated = true)
        {

        }

        internal bool Preview(bool animated = true)
        {

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
