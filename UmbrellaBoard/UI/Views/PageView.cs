using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using ModestTree;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace UmbrellaBoard.UI.Views
{
    internal class PageView : ViewController
    {
        private LoadingControl _loadingControl;
        [UIComponent("parsed-content")] 
        private Transform _parsedContentParent;
        private GameObject ParsedContent => _parsedContentParent.gameObject;
        [UIComponent("placeholder-content")]
        private Transform _placeHolderContent;
        private GameObject PlaceHolder => _placeHolderContent.gameObject;

        [Inject] 
        private DownloaderUtility _downloaderUtility;

        private Stack<string> _visitedPages = new();

        private string _nextPageToOpen = String.Empty;
        private bool _nextPageToHistory;
        private bool _nextPageIgnoreHistory;

        private bool _bsmlReady = false;
        private Task<Response<string>> _responseTask;

        internal event Action HistoryWasCleared;


        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                _loadingControl = gameObject.AddComponent<LoadingControl>();

                string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.PlaceholderView.bsml");
                BSMLParser.instance.Parse(
                    "<bg id='parsed-content' size-delta-x='100' size-delta-y='0' anchor-min-x='0.5' anchor-max-x='0.5' anchor-min-y='0' anchor-max-y='1'/>",
                    gameObject,
                    this
                );
                BSMLParser.instance.Parse(content, gameObject, this);

                rectTransform.sizeDelta = new(100, 0);
                rectTransform.anchorMin = new(.5f, 0);
                rectTransform.anchorMax = new(.5F, 1);
                _bsmlReady = true;
            }

            if (!String.IsNullOrEmpty(_nextPageToOpen))
            {
                GotoPage(_nextPageToOpen, _nextPageToHistory, _nextPageIgnoreHistory);
                _nextPageToOpen = string.Empty;
            }
            else if (addedToHierarchy)
            {
                _visitedPages.Push("placeholder");
                PlaceHolder.SetActive(true);
                ParsedContent.SetActive(false);
            }
        }

        internal bool OpenPageNextPresent(string pageURL, bool addToHistory = true, bool ignoreHistory = false)
        {
            if (!_visitedPages.IsEmpty() && !ignoreHistory && _visitedPages.Peek() == pageURL) return false;
            if (String.IsNullOrEmpty(pageURL)) return false;

            ShowLoading(true, "Loading page...");
            _nextPageToOpen = pageURL;
            _nextPageToHistory = addToHistory;
            _nextPageIgnoreHistory = ignoreHistory;

            return true;
        }

        internal void GotoPage(string pageURL, bool addToHistory = true, bool ignoreHistory = false)
        {
            if (!_visitedPages.IsEmpty() && !ignoreHistory && _visitedPages.Peek() == pageURL) return;
            if (String.IsNullOrEmpty(pageURL)) return;

            if (pageURL == "placeholder")
            {
                ShowLoading(false);
                PlaceHolder.SetActive(true);
                ParsedContent.SetActive(false);
                return;
            }

            _responseTask = Task.Run(() => _downloaderUtility.GetString(pageURL));
            ShowLoading(true, "Loading page...");
            if (addToHistory) _visitedPages.Push(pageURL);
        }

        [UIAction("open-page")]
        private void OpenPage(string pageURL) => GotoPage(pageURL);

        [UIAction("back")]
        public void Back()
        {
            // if we have less than 1 page in history, we can't go back a page
            if (_visitedPages.Count <= 1) return; 
               
            // pop one off and go to current top
            _visitedPages.Pop();
            GotoPage(_visitedPages.Peek(), false, true);
        }

        [UIAction("refresh")]
        public void Refresh()
        {
            if (_visitedPages.IsEmpty()) return;

            // we can just go to current top
            GotoPage(_visitedPages.Peek(), false, true);
        }

        [UIAction("clear-history")]
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
             ParsedContent.SetActive(!isLoading);
             PlaceHolder.SetActive(false);
        }

        private void ParseNewContent(string content)
        {
            for (int i = _parsedContentParent.childCount - 1; i >= 0; i--)
                Destroy(_parsedContentParent.GetChild(i).gameObject);

            BSMLParser.instance.Parse(content, ParsedContent, this);
        }

        private void Update()
        {
            if (!_bsmlReady) return;

            if (_responseTask != null)
            {
                if (_responseTask.IsCompleted)
                {
                    var response = _responseTask.Result;
                    _responseTask = null;

                    if (response.Valid)
                    {
                        ShowLoading(false);
                        ParseNewContent(response.content);
                    }
                    else
                    {
                        ParsedContent.SetActive(false);
                        if (response.httpCode < 200 || response.httpCode >= 300) _loadingControl.ShowError($"Failed to get content, http response: {response.httpCode}");
                        else if (response.content == null) _loadingControl.ShowError("No content received");
                        else _loadingControl.ShowError();
                    }
                }
                else
                {
                    ShowLoading(true, "Loading page...");
                }
            }
        }
    }
}
