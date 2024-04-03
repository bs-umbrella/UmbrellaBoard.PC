﻿using BeatSaberMarkupLanguage.Components;
using Newtonsoft.Json.Linq;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using UmbrellaBoard.Models;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage;
using System.Reflection;
using System.Threading.Tasks;
using SiraUtil.Logging;
using System.Security.Cryptography;

namespace UmbrellaBoard.UI.Views
{
    internal class CommunitiesView : ViewController, TableView.IDataSource
    {
        [Inject] Config _config;
        [Inject] SiraLog _log;

        [UIComponent("content-parent")]
        private Transform _parsedContentParent;
        private GameObject ParsedContent => _parsedContentParent.gameObject;
        [UIComponent("community-list")]
        private CustomListTableData _bsmlCommunityList;
        private bool _bsmlReady;

        private TableView _tableView;
        private LoadingControl _loadingControl;

        [Inject]
        private DownloaderUtility _downloaderUtility;

        private Task<Response<JObject>> _refreshCommunitiesTask;

        internal event Action<string> CommunityWasSelected;

        void Awake() => gameObject.AddComponent<CanvasGroup>();

        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation) return;
            _log.Info("did activate");
            string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.CommunitiesList.bsml");
            BSMLParser.instance.Parse(content, gameObject, this);

            rectTransform.sizeDelta = new Vector2(24, 60);
            rectTransform.anchorMax = new Vector2(.5f, 0);
            rectTransform.anchorMin = new Vector2(.5f, 1);
            
            _loadingControl = gameObject.AddComponent<LoadingControl>();

            _bsmlReady = true;
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            _log.Info("post parse");
            _tableView = _bsmlCommunityList.tableView;
            Destroy(_bsmlCommunityList);
            _bsmlCommunityList = null;
            _tableView.SetDataSource(this, true);
        }

        void Update()
        {
            if (!_bsmlReady) return;

            if (_refreshCommunitiesTask != null)
            {
                if (_refreshCommunitiesTask.IsCompleted)
                {
                    var response = _refreshCommunitiesTask.Result;
                    _refreshCommunitiesTask = null;

                    if (response.Valid)
                    {
                        ParsedContent.SetActive(true);
                        _loadingControl.ShowLoading(false);
                        HandleCommunitiesReceived(response.content);
                    }
                    else
                    {
                        if (response.httpCode < 200 || response.httpCode >= 300) _loadingControl.ShowError($"Failed to get content, http response: {response.httpCode}");
                        else if (response.content == null) _loadingControl.ShowError("Failed to get or parse content json");
                        else _loadingControl.ShowError();
                    }
                } 
                else
                {
                    ParsedContent.SetActive(false);
                    _loadingControl.ShowLoading(true, "Loading Communities...");
                }
            }
        }

        internal void RefreshCommunities(string communitiesURL) => _refreshCommunitiesTask = Task.Run(() => _downloaderUtility.GetJson(_config.communitiesDiscoveryURL, null));

        internal void RefreshCommunitiesNoRequest() => _tableView.ReloadDataKeepingPosition();

        public TableCell CellForIdx(TableView tableView, int idx)
        {
            var cell = tableView.DequeueReusableCellForIdentifier("UmbrellaCommunities") as CommunityCell;

            if (cell == null)
            {
                cell = CommunityCell.GetCell();
                cell.reuseIdentifier = "UmbrellaCommunities";
            }

            var data = _config.EnabledCommunities[idx];
            return cell.SetData(data.communityName, data.communityPageURL, data.communityBackgroundURL);
        }

        void HandleCommunitiesReceived(JObject content)
        {
            var discovery = content.ToObject<Discovery>();
            foreach (var community in discovery.pcDiscovery)
            {
                // check if this is already in disabled communities, if so update it
                var idx = _config.DisabledCommunities.FindIndex(x => x.communityName == community.communityName);
                if (idx >= 0)
                {
                    _config.DisabledCommunities[idx] = community;
                    continue;
                }

                // check if this is already in enabled communities, if so update it
                idx = _config.EnabledCommunities.FindIndex(x => x.communityName == community.communityName);
                if (idx >= 0)
                {
                    _config.EnabledCommunities[idx] = community;
                    continue;
                }

                // this was a new community, add it to enabled!
                _config.EnabledCommunities.Add(community);
            }

            _tableView.ReloadDataKeepingPosition();
        }

        [UIAction("community-selected")]
        private void HandleCommunitySelected(TableView tableView, int selectedCell)
        {
            _log.Info("handle community was selected");
            CommunityWasSelected.Invoke(_config.EnabledCommunities[selectedCell].communityPageURL);

            foreach (var cell in _tableView.visibleCells)
                cell.SetSelected(false, SelectableCell.TransitionType.Instant, _tableView, false);
            _tableView._selectedCellIdxs.Clear();
            _tableView.RefreshCells(true, false);
        }

        public float CellSize() => 12;

        public int NumberOfCells() => _config?.EnabledCommunities?.Count ?? 0;

        internal struct Discovery
        {
            public Community[] pcDiscovery;
        }
    }
}