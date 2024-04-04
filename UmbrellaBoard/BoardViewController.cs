using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Reflection;
using System.Linq;
using TMPro;
using UmbrellaBoard.UI.Views;
using UnityEngine;
using Zenject;
using SiraUtil.Logging;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace UmbrellaBoard
{
    internal class BoardViewController : NavigationController, IInitializable, IDisposable
    {
        [Inject] Config _config;
        [Inject] SiraLog _log;

        [Inject] private MainFlowCoordinator _mainFlowCoordinator;
        [Inject] private CommunitiesView _communitiesView;
        [Inject] private PageView _pageView;
        [Inject] private CommunityConfigurationView _configurationView;

        private ViewController _activeViewController;
        private ViewController _viewControllerToPresent;

        [UIComponent("header-content")]
        private Transform _headerContent;
        [UIComponent("header-bg")]
        private HorizontalLayoutGroup _headerBG;
        [UIComponent("nav-buttons")]
        private Transform _navButtons;
        [UIComponent("header-text")]
        private TextMeshProUGUI _headerText;
        [UIComponent("open-settings")]
        private ClickableImage _openSettings;
        [UIComponent("close-settings")]
        private ClickableImage _closeSettings;

        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            _log.Info("DidActivate");

            if (firstActivation)
            {
                _alignment = Alignment.Beginning;
                _mainFlowCoordinator.SetViewControllersToNavigationController(this, new ViewController[] { _communitiesView, _pageView });
                _activeViewController = _pageView;
                gameObject.AddComponent<Touchable>();

                string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.HeaderContent.bsml");
                BSMLParser.instance.Parse(content, gameObject, this);
            }

            if (addedToHierarchy) DiscoverCommunities();
        }

        [UIAction("#post-parse")]
        void PostParse()
        {
            var titleImage = Resources.FindObjectsOfTypeAll<ImageView>().FirstOrDefault(x => x != null && x.sprite?.name == "RoundRect10" && x.transform.parent?.name == "TitleViewController" && x.gameObject.name == "BG");
            if (titleImage != null)
            {
                var bg = _headerBG.GetComponent<Backgroundable>();
                var img = bg.background as ImageView;

                img.sprite = titleImage.sprite;
                img.gradient = true;
                img.material = titleImage.material;
                img.color = Color.white;
                // ff69b4ff
                img.color0 = new Color(1f, 0.4f, 0.7f, 1.0f);
                img.color1 = new Color(1f, 0.4f, 0.7f, 0.0f);

                img._gradientDirection = titleImage._gradientDirection;
                img._flipGradientColors = titleImage._flipGradientColors;
                img._skew = titleImage._skew;
                img.alphaHitTestMinimumThreshold = titleImage.alphaHitTestMinimumThreshold;
                img.fillAmount = titleImage.fillAmount;
                img.fillCenter = titleImage.fillCenter;
                img.fillClockwise = titleImage.fillClockwise;
                img.fillMethod = titleImage.fillMethod;
                img.fillOrigin = titleImage.fillOrigin;
                img.hideFlags = titleImage.hideFlags;
                img.maskable = titleImage.maskable;
                img.material = titleImage.material;
                img.onCullStateChanged = titleImage.onCullStateChanged;
                img.overrideSprite = titleImage.overrideSprite;
                img.pixelsPerUnitMultiplier = titleImage.pixelsPerUnitMultiplier;
                img.preserveAspect = titleImage.preserveAspect;
                img.raycastTarget = titleImage.raycastTarget;
                img.sprite = titleImage.sprite;
                img.tag = titleImage.tag;
                img.type = titleImage.type;
                img.useGUILayout = titleImage.useGUILayout;
                img.useSpriteMesh = titleImage.useSpriteMesh;
            }

            _navButtons.GetChild(0).eulerAngles = new Vector3(0, 0, -90);

        }

        [UIAction("discover-communities")]
        internal void DiscoverCommunities()
        {
            _communitiesView.RefreshCommunities(_config.CommunitiesDiscoveryURL);
            SwitchDisplayedView(_pageView);
        }

        static IEnumerator FadeInOverSeconds(ViewController view, float time)
        {
            float step = 1.0f / time;
            var group = view.canvasGroup;
            if (!group) yield break;

            for (float t = 0; t < 1.0f; t += Time.deltaTime * step)
            {
                group.alpha = SimpleEasing.Max(group.alpha, SimpleEasing.EaseInQuart(t));
                yield return null;
            }

            group.alpha = 1.0f;
        }

        static IEnumerator FadeOutOverSeconds(ViewController view, float time)
        {
            float step = 1.0f / time;
            var group = view.canvasGroup;
            if (!group) yield break;

            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime * step)
            {
                group.alpha = SimpleEasing.Min(group.alpha, SimpleEasing.EaseInQuart(t));
                yield return null;
            }

            group.alpha = 0.0f;
        }

        private void SwitchDisplayedView(ViewController targetView)
        {
            if (_viewControllerToPresent != null) return;

            _viewControllerToPresent = targetView;
            // into settings -> no nav buttons
            _navButtons.gameObject.SetActive(_viewControllerToPresent != _configurationView);

            // when going into / out of settings change the buttons
            _openSettings.gameObject.SetActive(_viewControllerToPresent != _configurationView);
            _closeSettings.gameObject.SetActive(_viewControllerToPresent == _configurationView);

            // check if we have an active one, then remove and push the new one after removal is complete
            if (_activeViewController != null)
            {
                _mainFlowCoordinator.StartCoroutine(FadeOutOverSeconds(_activeViewController, 0.2f));
                _mainFlowCoordinator.PopViewControllerFromNavigationController(this, PresentViewControllerAfterPop);
                return;
            }

            // we had no active view, so we can just directly push this one
            _mainFlowCoordinator.StartCoroutine(FadeInOverSeconds(targetView, 0.2f));
            _mainFlowCoordinator.PushViewControllerToNavigationController(this, targetView, FinishPushingViewController);
        }

        private void PresentViewControllerAfterPop()
        {
            if (!_viewControllerToPresent) return;

            _activeViewController = null;
            _mainFlowCoordinator.StartCoroutine(FadeInOverSeconds(_viewControllerToPresent, 0.2f));
            _mainFlowCoordinator.PushViewControllerToNavigationController(this, _viewControllerToPresent, FinishPushingViewController);
        }

        private void FinishPushingViewController()
        {
            _activeViewController = _viewControllerToPresent;
            _viewControllerToPresent = null;
        }

        private void CommunityWasSelected(string communityURL)
        {
            if (_pageView.OpenPageNextPresent(communityURL))
                SwitchDisplayedView(_pageView);
        }

        public void Initialize()
        {
            _communitiesView.CommunityWasSelected += CommunityWasSelected;
            _pageView.HistoryWasCleared += DiscoverCommunities;
        }
        public void Dispose()
        {
            _communitiesView.CommunityWasSelected -= CommunityWasSelected;
            _pageView.HistoryWasCleared -= DiscoverCommunities;
        }

        [UIAction("back-page")]
        private void BackPage() => _pageView.Back();

        [UIAction("refresh-page")]
        private void RefreshPage() => _pageView.Refresh();

        [UIAction("open-config")]
        private void OpenCommunityConfig() => SwitchDisplayedView(_configurationView);

        [UIAction("close-config")]
        private void CloseCommunityConfig() => SwitchDisplayedView(_pageView);

        static class SimpleEasing
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float Flip(float t) { return 1.0f - t; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float Square(float t) { return t * t; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float EaseIn(float t) { return Square(t); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float EaseOut(float t) { return Flip(Square(Flip(t))); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float EaseInQuart(float t) { return Square(Square(t)); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float Lerp(float a, float b, float t) { return a + (b - a) * t; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float EasedT(float t) { return Lerp(EaseIn(t), EaseOut(t), t); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float Min(float a, float b) { return a < b ? a : b; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static float Max(float a, float b) { return a > b ? a : b; }
        }
    }
}
