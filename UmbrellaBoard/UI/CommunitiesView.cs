using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using UnityEngine;
using BeatSaberMarkupLanguage;
using System.Reflection;
using UmbrellaBoard.Models;

namespace UmbrellaBoard.UI
{
    internal class CommunitiesView : ViewController, TableView.IDataSource
    {
        private TableView _tableView;
        private LoadingControl _loadingControl;
        private GameObject _parsedContentParent;
        private CustomListTableData _bsmlCommunityList;

        internal event Action<string> CommunityWasSelected;

        public TableCell CellForIdx(TableView tableView, int idx)
        {
            throw new NotImplementedException();
        }

        public float CellSize()
        {
            throw new NotImplementedException();
        }

        public int NumberOfCells()
        {
            throw new NotImplementedException();
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation)
                return;

            _loadingControl = gameObject.AddComponent<LoadingControl>();

            string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.UI.CommunitiesView.bsml");
            BSMLParser.instance.Parse(content, gameObject, this);
        }

        private void Update()
        {
            
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {

        }

        private void HandleCommunitySelected(TableView tableView, int selectedCell)
        {

        }

        internal void RefreshCommunities(string url)
        {

        }
    }
}
