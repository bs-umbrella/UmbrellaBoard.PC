using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using UnityEngine;
using UnityEngine.UI;

namespace UmbrellaBoard.UI.Tags
{
    
    internal class OpenPageClickableText : ClickableTextTag
    {
        public override string[] Aliases => new string[] { "open-page-text" };
        public override GameObject CreateObject(Transform parent)
        {
            GameObject go = base.CreateObject(parent);
            PageOpener opener = go.AddComponent<PageOpener>();
            opener.activationSource = go.GetComponent<ClickableText>();
            return go;
        }
    }

    internal class OpenPageClickableImageTag : ClickableImageTag
    {
        public override string[] Aliases => new string[] { "open-page-image", "open-page-img" };
        public override GameObject CreateObject(Transform parent)
        {
            GameObject go = base.CreateObject(parent);
            PageOpener opener = go.AddComponent<PageOpener>();
            opener.activationSource = go.GetComponent<ClickableImage>();
            return go;
        }
    }

    internal class OpenPageButtonTag : ButtonTag
    {
        public override string[] Aliases => new string[] { "open-page-button" };
        public override GameObject CreateObject(Transform parent)
        {
            GameObject go = base.CreateObject(parent);
            PageOpener opener = go.AddComponent<PageOpener>();
            opener.activationSource = go.GetComponent<Button>();
            return go;
        }
    }

    internal class OpenPagePrimaryButtonTag : PrimaryButtonTag
    {
        public override string[] Aliases => new string[] { "open-page-action-button" };
        public override GameObject CreateObject(Transform parent)
        {
            GameObject go = base.CreateObject(parent);
            PageOpener opener = go.AddComponent<PageOpener>();
            opener.activationSource = go.GetComponent<Button>();
            return go;
        }
    }

    internal class OpenPagePageButtonTag : PageButtonTag
    {
        public override string[] Aliases => new string[] { "open-page-page-button" };
        public override GameObject CreateObject(Transform parent)
        {
            GameObject go = base.CreateObject(parent);
            PageOpener opener = go.AddComponent<PageOpener>();
            opener.activationSource = go.GetComponent<Button>();
            return go;
        }
    }
}
