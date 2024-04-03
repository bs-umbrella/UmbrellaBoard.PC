using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UmbrellaBoard.Views;
using UnityEngine;
using Zenject;

namespace UmbrellaBoard
{
    internal class BoardViewController : NavigationController, IInitializable, IDisposable
    {
        [Inject] Config _config;
        private MainFlowCoordinator _mainFlowCoordinator;
        private CommunitiesView _communitiesView;
        private PageView _pageView;

        private ViewController _activeViewController;
        private ViewController _viewControllerToPresent;

        private GameObject _headerContent;
        private TextMeshProUGUI _headerText;

        private Action _presentAfterPopAction;
        private Action _finishPushAction;


        [Inject]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, CommunitiesView communitiesView, PageView pageView)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _communitiesView = communitiesView;
            _pageView = pageView;
            _alignment = Alignment.Beginning;
        }

        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                _mainFlowCoordinator.SetViewControllersToNavigationController(this, new ViewController[] { _communitiesView, _pageView });
                _activeViewController = _pageView;
                gameObject.AddComponent<Touchable>();

                string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.HeaderContent.bsml");
                BSMLParser.instance.Parse(content, gameObject, this);
            }

            StartRefreshContent();
        }

        internal void StartRefreshContent()
        {
            _communitiesView.RefreshCommunities(_config.communitiesDiscoveryURL);
            SwitchDisplayedView(_pageView);
        }

        private void SwitchDisplayedView(ViewController targetView)
        {
            if (_viewControllerToPresent)
                return;

            _viewControllerToPresent = targetView;
            if (_activeViewController != null)
            {
                _mainFlowCoordinator.PopViewControllerFromNavigationController(this, _presentAfterPopAction);
                return;
            }

            _mainFlowCoordinator.PushViewControllerToNavigationController(this, targetView, _finishPushAction);
        }

        private void PresentViewControllerAfterPop()
        {
            if (!_viewControllerToPresent)
                return;

            _activeViewController = null;
            _mainFlowCoordinator.PushViewControllerToNavigationController(this, _viewControllerToPresent, _finishPushAction);
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
            _pageView.HistoryWasCleared += StartRefreshContent;
        }
        public void Dispose()
        {
            _communitiesView.CommunityWasSelected -= CommunityWasSelected;
            _pageView.HistoryWasCleared -= StartRefreshContent;
        }

    }
}
