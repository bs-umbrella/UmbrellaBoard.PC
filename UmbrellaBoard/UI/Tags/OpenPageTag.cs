using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using UnityEngine;
using UnityEngine.UI;

namespace UmbrellaBoard.UI.Tags
{
    static class AddPageOpenerUtil
    {
        public static void AddPageOpener<T>(this GameObject go) where T : Component
        {
            var opener = go.AddComponent<PageOpener>();
            opener.ActivationSource = go.GetComponent<T>();
        }
    }

    internal class OpenPageClickableText : ClickableTextTag
    {
        public override string[] Aliases => new string[] { "open-page-text" };
        public override GameObject CreateObject(Transform parent)
        {
            var go = base.CreateObject(parent);
            go.AddPageOpener<ClickableText>();
            return go;
        }
    }

    internal class OpenPageClickableImageTag : ClickableImageTag
    {
        public override string[] Aliases => new string[] { "open-page-image", "open-page-img" };
        public override GameObject CreateObject(Transform parent)
        {
            var go = base.CreateObject(parent);
            go.AddPageOpener<ClickableImage>();
            return go;
        }
    }

    internal class OpenPageButtonTag : ButtonTag
    {
        public override string[] Aliases => new string[] { "open-page-button" };
        public override GameObject CreateObject(Transform parent)
        {
            var go = base.CreateObject(parent);
            go.AddPageOpener<Button>();
            return go;
        }
    }

    internal class OpenPagePrimaryButtonTag : PrimaryButtonTag
    {
        public override string[] Aliases => new string[] { "open-page-action-button" };
        public override GameObject CreateObject(Transform parent)
        {
            var go = base.CreateObject(parent);
            go.AddPageOpener<Button>();
            return go;
        }
    }

    internal class OpenPagePageButtonTag : PageButtonTag
    {
        public override string[] Aliases => new string[] { "open-page-page-button" };
        public override GameObject CreateObject(Transform parent)
        {
            var go = base.CreateObject(parent);
            go.AddPageOpener<Button>();
            return go;
        }
    }
}
