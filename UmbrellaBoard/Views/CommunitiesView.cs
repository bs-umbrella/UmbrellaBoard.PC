using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using UmbrellaBoard.Models;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage;
using System.Reflection;
using SiraUtil.Logging;

namespace UmbrellaBoard.Views
{
    internal class CommunitiesView : ViewController, TableView.IDataSource
    {
        [Inject] Config _config;
        [Inject] SiraLog _log;

        private TableView _tableView;
        private LoadingControl _loadingControl;
        private GameObject _parsedContentParent;
        private CustomListTableData _bsmlCommunityList;
        private DownloaderUtility _downloaderUtility;
        private Response _response;
        private bool _bsmlReady;


        internal event Action<string> CommunityWasSelected;

        public TableCell CellForIdx(TableView tableView, int idx)
        {
            var cell = (CommunityCell)tableView.DequeueReusableCellForIdentifier("UmbrellaCommunities") ?? null;

            if (cell == null)
            {
                cell = CommunityCell.GetCell();
                cell.reuseIdentifier = "UmbrellaCommunities";
            }

            Community data = _config.enabledCommunities[idx];
            cell.SetData(data.communityName, data.communityPageURL, data.communityBackgroundURL);
            return cell;
        }

        public float CellSize() => 12;

        public int NumberOfCells() => _config.enabledCommunities.Count;

        internal void RefreshCommunities(string communitiesURL) => _response = _downloaderUtility.GetJson(communitiesURL, null);

        [UIAction("#post-parse")]
        private void PostParse()
        {
            _log.Info("post parse");
            _tableView = _bsmlCommunityList.tableView;
            Destroy(_bsmlCommunityList);
            _bsmlCommunityList = null;
            _tableView.SetDataSource(this, true);
        }

        private void HandleCommunitySelected(TableView tableView, int selectedCell)
        {
            _log.Info("handle community was selected");
            CommunityWasSelected.Invoke(_config.enabledCommunities[selectedCell].communityPageURL);

            foreach (var cell in _tableView.visibleCells)
                cell.SetSelected(false, SelectableCell.TransitionType.Instant, _tableView, false);
            _tableView._selectedCellIdxs.Clear();
            _tableView.RefreshCells(true, false);
        }
        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation)
                return;
            _log.Info("did activate");
            _loadingControl = gameObject.AddComponent<LoadingControl>();

            string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.CommunitiesList.bsml");
            BSMLParser.instance.Parse(content, gameObject, this);

            RectTransform rt = rectTransform;
            rt.sizeDelta = new Vector2(24, 60);
            rt.anchorMax = new Vector2(.5f, 0);
            rt.anchorMin = new Vector2(.5f, 1);
            _bsmlReady = true;
        }
    }
}
