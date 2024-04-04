using BeatSaberMarkupLanguage.Tags;
using System.Reflection;
using UnityEngine;

namespace UmbrellaBoard.UI.Tags
{
    internal class CarouselTag : BSMLTag
    {
        public override string[] Aliases => new[] { "carousel", "bubble-carousel" };

        public override GameObject CreateObject(Transform parent)
        {
            var go = new UnityEngine.GameObject("Carousel");
            go.SetActive(false);
            go.transform.SetParent(parent, false);

            var carousel = go.AddComponent<Carousel.Carousel>();
            var hvGroup = go.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            hvGroup.SetLayoutVertical();
            carousel._carouselLayoutGroup = hvGroup;


            var vp = new UnityEngine.GameObject("Viewport");
            vp.transform.SetParent(go.transform, false);

            var vpMask = vp.AddComponent<UnityEngine.UI.Mask>();
            var vpImg = vp.AddComponent<HMUI.ImageView>();
            var vpRect = vp.transform as UnityEngine.RectTransform;
            vpMask.showMaskGraphic = false;
            vpImg.color = new Color(1, 1, 1, 1);
            vpImg.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            vpImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            vpRect.localPosition = new Vector3(0, 0, 0);
            vpRect.anchorMin = new Vector2(0, 0);
            vpRect.anchorMax = new Vector2(1, 1);
            vpRect.anchoredPosition = new Vector2(0, 0);
            vpRect.sizeDelta = new Vector2(0, 0);

            var content = new UnityEngine.GameObject("Content");
            content.transform.SetParent(vpRect, false);
            var fitter = content.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            carousel._contentSizeFitter = fitter;

            var layout = content.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            carousel._contentLayoutGroup = layout;

            var contentRect = content.transform as UnityEngine.RectTransform;
            contentRect.localPosition = new Vector3(0, 0, 0);
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(0, 1);
            contentRect.anchoredPosition = new Vector2(0, 0);

            carousel._viewPort = vpRect;
            carousel._content = contentRect;

            var externalComponents = content.AddComponent<BeatSaberMarkupLanguage.Components.ExternalComponents>();
            externalComponents.components.Add(go.transform as UnityEngine.RectTransform);
            externalComponents.components.Add(carousel);

            var elem = go.AddComponent<UnityEngine.UI.LayoutElement>();
            elem.preferredHeight = 50;
            elem.preferredWidth = 90;
            externalComponents.components.Add(elem);
            carousel._carouselLayoutElement = elem;

            BeatSaberMarkupLanguage.BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.UI.CarouselTicker.bsml"), carousel.transform.gameObject, carousel);

            carousel._tickerLayoutGroup = carousel._ticker.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            carousel._tickerLayoutElement = carousel._ticker.GetComponent<UnityEngine.UI.LayoutElement>();
            carousel._tickerSizeFitter = carousel._ticker.GetComponent<UnityEngine.UI.ContentSizeFitter>();

            carousel._bubblePrefab.SetActive(false);

            carousel._ticker.SetParent(carousel.transform, false);
            carousel.MoveTicker(Carousel.Carousel.CarouselLocation.Default, Carousel.Carousel.CarouselDirection.Horizontal, true);

            go.SetActive(true);
            // we return content for things to be parented to
            return content;
        }
    }
}
