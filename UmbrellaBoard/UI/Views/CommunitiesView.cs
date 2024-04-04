using BeatSaberMarkupLanguage.Components;
using Newtonsoft.Json.Linq;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage;
using System.Reflection;
using System.Threading.Tasks;
using SiraUtil.Logging;
using TMPro;
using UnityEngine.UI;

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
            rectTransform.anchorMin = new Vector2(.5f, 0);
            rectTransform.anchorMax = new Vector2(.5f, 1);
            
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

            var data = _config.enabledCommunities[idx];
            return cell.SetData(data.communityName, data.communityPageURL, data.communityBackgroundURL);
        }

        void HandleCommunitiesReceived(JObject content)
        {
            var discovery = content.ToObject<Discovery>();
            foreach (var community in discovery.pcDiscovery)
            {
                // check if this is already in disabled communities, if so update it
                var idx = _config.disabledCommunities.FindIndex(x => x.communityName == community.communityName);
                if (idx >= 0)
                {
                    _config.disabledCommunities[idx] = community;
                    continue;
                }

                // check if this is already in enabled communities, if so update it
                idx = _config.enabledCommunities.FindIndex(x => x.communityName == community.communityName);
                if (idx >= 0)
                {
                    _config.enabledCommunities[idx] = community;
                    continue;
                }

                // this was a new community, add it to enabled!
                _config.enabledCommunities.Add(community);
            }

            _tableView.ReloadDataKeepingPosition();
        }

        [UIAction("community-selected")]
        private void HandleCommunitySelected(TableView tableView, int selectedCell)
        {
            _log.Info("handle community was selected");
            CommunityWasSelected.Invoke(_config.enabledCommunities[selectedCell].communityPageURL);

            foreach (var cell in _tableView.visibleCells)
                cell.SetSelected(false, SelectableCell.TransitionType.Instant, _tableView, false);
            _tableView._selectedCellIdxs.Clear();
            _tableView.RefreshCells(true, false);
        }

        public float CellSize() => 12;

        public int NumberOfCells() => _config?.enabledCommunities?.Count ?? 0;

        internal struct Discovery
        {
            public Community[] pcDiscovery;
        }

        internal class CommunityCell : TableCell
        {
            [UIComponent("community-name")]
            private TextMeshProUGUI _communityName;
            [UIComponent("community-background")]
            private ImageView _communityBackground;
            [UIComponent("mask")]
            private ImageView _mask;
            public string CommunityName { get => _communityName.text; private set => _communityName.text = value; }
            public string CommunityURL { get; private set; }

            public override void HighlightDidChange(TransitionType transitionType) => UpdateHighlight();
            public override void SelectionDidChange(TransitionType transitionType) => UpdateHighlight();

            private void UpdateHighlight()
            {
                _communityBackground.color = highlighted || selected ? new Color(1, 1, 1, 1) : new Color(.6f, .6f, .6f, 1);
                _communityName.enabled = !(highlighted || selected);
            }

            internal CommunityCell SetData(string communityName, string communityURL, Sprite background = null)
            {
                CommunityName = communityName;
                CommunityURL = communityURL;
                _communityBackground.sprite = background;
                return this;
            }

            internal CommunityCell SetData(string communityName, string communityURL, string backgroundURL)
            {
                CommunityName = communityName;
                CommunityURL = communityURL;
                _communityBackground.SetImageAsync(backgroundURL);
                return this;
            }

            internal static CommunityCell GetCell()
            {
                var go = new GameObject("CommunityCell");
                go.AddComponent<Touchable>();
                var cell = go.AddComponent<CommunityCell>();
                var content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.CommunityCell.bsml");
                BSMLParser.instance.Parse(content, cell.gameObject, cell);

                cell._mask.rectTransform.sizeDelta = new Vector2(20, 10);
                var mask = cell._mask.gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;

                cell._mask.type = Image.Type.Sliced;
                cell._mask.color = Color.white;
                cell._mask.material = Utilities.ImageResources.NoGlowMat;

                cell._mask.raycastTarget = false;
                cell._communityName.raycastTarget = false;
                cell._communityBackground.raycastTarget = false;

                return cell;
            }
        }
    }
}
