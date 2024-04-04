using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UmbrellaBoard.UI.TypeHandlers
{
    [ComponentHandler(typeof(PageOpener))]
    internal class PageOpenerHandler : TypeHandler<PageOpener>
    {
        public override Dictionary<string, Action<PageOpener, string>> Setters => new()
        {
            { "page", (component, value) => component.Page = value },
            { "openInBrowser", (component, value) => component.OpenInBrowser = bool.Parse(value) }
        };

        public override Dictionary<string, string[]> Props => new()
        {
            { "page", new[] { "page", "url" } },
            { "openInBrowser", new[] { "in-browser", "open-in-browser" } }
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            var pageOpener = componentType.component as PageOpener;
            var activationSource = pageOpener.ActivationSource;

            if (activationSource != null)
            {
                if (activationSource is ClickableText clickableText)
                    clickableText.OnClickEvent += (_) => pageOpener.OpenPage();
                if (activationSource is ClickableImage clickableImage)
                    clickableImage.OnClickEvent += (_) => pageOpener.OpenPage();
                if (activationSource is Button button)
                    button.onClick.AddListener(pageOpener.OpenPage);
            } 

            pageOpener.OpenPageEvent += delegate (string page)
            {
                if (!parserParams.actions.TryGetValue("open-page", out BSMLAction openPageAction))
                    throw new Exception($"open-page action not found");

                openPageAction.Invoke(page);
            };

            base.HandleType(componentType, parserParams);
        }
    }
}
