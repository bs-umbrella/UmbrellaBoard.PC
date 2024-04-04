using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using System;
using System.Collections.Generic;
using static UmbrellaBoard.UI.Carousel.Carousel;

namespace UmbrellaBoard.UI.TypeHandlers
{
    [ComponentHandler(typeof(Carousel.Carousel))]
    internal class CarouselHandler : TypeHandler<Carousel.Carousel>
    {
        public override Dictionary<string, Action<Carousel.Carousel, string>> Setters => new()
        {
            { "direction", (component, value) => component.Direction = ParseDirection(value) },
            { "location", (component, value) => component.Location = ParseLocation(value) },
            { "timerBehaviour", (component, value) => component.TimerBehaviour = ParseTimerBehavior(value) },
            { "timerLength", (component, value) => component.TimerLength = float.Parse(value) },
            { "contentAlignment", (component, value) => component.Alignment = ParseAlignment(value) },
            { "showButtons", (component, value) => component.ShowButtons = bool.Parse(value) },
            { "pauseOnHover", (component, value) => component.PauseOnHover = bool.Parse(value) },
            { "inactiveAlpha", (component, value) => component.InactiveAlpha = float.Parse(value) },
        };

        public override Dictionary<string, string[]> Props => new()
        {
            { "startChildIndex", new[] { "start-child-index", "start-index" } },
            { "direction", new[] { "direction" } },
            { "location", new[] { "location" } },
            { "timerBehaviour", new[] { "timer-behaviour"} },
            { "timerLength", new[] { "timer-length"} },
            { "contentAlignment", new[] { "content-alignment" } },
            { "showButtons", new[] { "show-buttons" } },
            { "pauseOnHover", new[] { "pause-on-hover" } },
            { "inactiveAlpha", new[] { "inactive-alpha" } }
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            Carousel.Carousel carousel = componentType.component as Carousel.Carousel;
            carousel.Setup();
        }

        public override void HandleTypeAfterChildren(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleTypeAfterChildren(componentType, parserParams);
            Carousel.Carousel carousel = componentType.component as Carousel.Carousel;
            carousel.SetupAfterChildren();

            if (componentType.data.TryGetValue("startChildIndex", out string startChildIndex))
                carousel.SetCurrentlyActiveChildIndex(int.Parse(startChildIndex), false);
        }


        private CarouselDirection ParseDirection(string value)
        {
            switch (value)
            {
                case "Vertical":
                    return CarouselDirection.Vertical;
               
                case "Horizontal": 
                default:
                    return CarouselDirection.Horizontal;
            }
        }

        private CarouselLocation ParseLocation(string value)
        {
            switch (value)
            {
                case "Bottom":
                    return CarouselLocation.Bottom;
                case "Top":
                    return CarouselLocation.Top;
                case "Left":
                    return CarouselLocation.Left;
                case "Right":
                    return CarouselLocation.Right;
                case "Default":
                default:
                    return CarouselLocation.Default;
            }
        }

        private CarouselTimerBehaviour ParseTimerBehavior(string value)
        {
            switch (value)
            {
                case "PingPong":
                    return CarouselTimerBehaviour.PingPong;
                case "Loop":
                    return CarouselTimerBehaviour.Loop;
                case "LoopForward":
                    return CarouselTimerBehaviour.LoopForward;
                case "LoopBackward":
                    return CarouselTimerBehaviour.LoopBackward;
                case "None":
                default:
                    return CarouselTimerBehaviour.None;
            }
        }

        private CarouselAlignment ParseAlignment(string value)
        {
            switch (value)
            {
                case "Beginning":
                    return CarouselAlignment.Beginning;
                case "Middle":
                    return CarouselAlignment.Middle;
                case "End":
                    return CarouselAlignment.End;
                case "Center":
                default:
                    return CarouselAlignment.Center;
            }
        }
    }
}
