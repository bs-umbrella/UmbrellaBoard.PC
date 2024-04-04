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
    internal class PageOpenerHandler : TypeHandler<PageOpener>
    {
        public override Dictionary<string, Action<PageOpener, string>> Setters => new()
        {
            { "page", (component, value) => component.page = value },
            { "openInBrowser", (component, value) => component.openInBrowser = bool.Parse(value) }
        };

        public override Dictionary<string, string[]> Props => new()
        {
            { "page", new[] { "page", "url" } },
            { "openInBrowser", new[] { "in-browser", "open-in-browser" } }
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            PageOpener pageOpener = componentType.component as PageOpener;
            Component activationSource = pageOpener.activationSource;
            pageOpener.host = parserParams.host;

            if (activationSource != null)
            {
                if (activationSource is ClickableText clickableText)
                    clickableText.OnClickEvent += (_) => pageOpener.OpenPage();
                if (activationSource is ClickableImage clickableImage)
                    clickableImage.OnClickEvent += (_) => pageOpener.OpenPage();
                if (activationSource is Button button)
                    button.onClick.AddListener(pageOpener.OpenPage);
            }

            base.HandleType(componentType, parserParams);
        }
    }
}
