using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using ModestTree;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace UmbrellaBoard.Views
{
    internal class PageView : ViewController
    {
        private LoadingControl _loadingControl;
        [UIValue("_parsedContentParent")] private GameObject _parsedContentParent;
        private GameObject _placeHolderContent;

        [Inject] private DownloaderUtility _downloaderUtility;
        private Stack<string> _visitedPages;

        private string _nextPageToOpen;
        private bool _nextPageToHistory;
        private bool _bsmlReady = false;
        private Response _response;

        internal event Action HistoryWasCleared;


        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                _loadingControl = gameObject.AddComponent<LoadingControl>();

                string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.PlaceholderView.bsml");
                BSMLParser.instance.Parse(
                    "<bg id='_parsedContentParent' size-delta-x='100' size-delta-y='0' anchor-min-x='0.5' anchor-max-x='0.5' anchor-min-y='0' anchor-max-y='1'/>",
                    gameObject, 
                    this
                );
                BSMLParser.instance.Parse(content, gameObject, this);

                RectTransform rt = rectTransform;
                rt.sizeDelta = new(100, 0);
                rt.anchorMin = new(.5f, 0);
                rt.anchorMax = new(.5F, 1);
                _bsmlReady = true;
            }

            if (!_nextPageToOpen.IsEmpty())
            {
                GotoPage(_nextPageToOpen, _nextPageToHistory);
                _nextPageToOpen = string.Empty;
            }
            else if (addedToHierarchy)
            {
                _visitedPages.Push("placeholder");
                _placeHolderContent.SetActive(true);
                _parsedContentParent.SetActive(false);
            }
        }

        internal bool OpenPageNextPresent(string pageURL, bool addToHistory = true)
        {
            if (!_visitedPages.IsEmpty() && _visitedPages.Peek() == pageURL || pageURL.IsEmpty())
                return false;

            ShowLoading(true, "Loading page...");
            _nextPageToOpen = pageURL;
            _nextPageToHistory = addToHistory;

            return true;
        }

        internal void GotoPage(string pageURL, bool addToHistory = true)
        {
            if (!_visitedPages.IsEmpty() && _visitedPages.Peek() == pageURL || pageURL.IsEmpty())
                return;

            _response = _downloaderUtility.GetString(pageURL);
            ShowLoading(true, "Loading page...");
            if (addToHistory)
                _visitedPages.Push(pageURL);
        }

        private void OpenPage(string pageURL) => GotoPage(pageURL);

        private void Back()
        {
            if (_visitedPages.IsEmpty()) 
                return;

            _visitedPages.Pop();
            GotoPage(_visitedPages.Peek(), false);
        }

        private void Refresh()
        {
            if (_visitedPages.IsEmpty())
                return;

            GotoPage(_visitedPages.Peek(), false);
        }

        private void ClearHistory()
        {
            _visitedPages.Clear();
            HistoryWasCleared.Invoke();
        }

        private void ShowLoading(bool isLoading, string loadingText = "")
        {
            if (isLoading && loadingText.IsEmpty())
                loadingText = "Loading page...";

             _loadingControl.ShowLoading(isLoading, loadingText);
             _parsedContentParent.SetActive(!isLoading);
             _placeHolderContent.SetActive(false);
        }

        private void ParseNewContent(string content)
        {
            for (int i = _parsedContentParent.transform.childCount - 1; i >= 0; i--)
                Destroy(_parsedContentParent.transform.GetChild(i).gameObject);

            BSMLParser.instance.Parse(content, _parsedContentParent, this);
        }

        private void Update()
        {
            if (!_bsmlReady)
                return;

            if (_response.content == null)
            {
                _parsedContentParent.SetActive(false);

                if (_response.httpCode < 200 || _response.httpCode >= 300)
                    _loadingControl.ShowError("Http respone code " + _response.httpCode);
                else if ((_response.content as string).IsEmpty())
                    _loadingControl.ShowError("No content received");
                else
                    _loadingControl.ShowError();
            }
            else
            {
                ShowLoading(false);
                ParseNewContent((string) _response.content);
            }
        }
    }
}
