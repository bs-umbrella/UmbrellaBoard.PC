using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using UnityEngine.UI;

namespace UmbrellaBoard.UI.Views
{
    internal class CommunityConfigurationView : HMUI.ViewController
    {
        [Inject] 
        private Config _config;
        [Inject] 
        private CommunitiesView _communitiesView;

        [UIComponent("enabled-list")]
        private  CustomListTableData _enabledCommunitiesBSMLList;
        [UIComponent("disabled-list")]
        private  CustomListTableData _disabledCommunitiesBSMLList;

        CommunityConfigurationCellListDataSource _enabledCommunitiesList;
        CommunityConfigurationCellListDataSource _disabledCommunitiesList;

        public override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                gameObject.AddComponent<CanvasGroup>();
                string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.CommunityConfigurationView.bsml");
                BSMLParser.instance.Parse(content, gameObject, this);

                rectTransform.sizeDelta = new Vector2(100, 0);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
            }

            RefreshLists();
        }

        [UIAction("#post-parse")]
        void PostParse()
        {
            {
                var tableView = _enabledCommunitiesBSMLList.tableView;
                var go = _enabledCommunitiesBSMLList.gameObject;
                Destroy(_enabledCommunitiesBSMLList);
                _enabledCommunitiesBSMLList = null;
                _enabledCommunitiesList = go.AddComponent<CommunityConfigurationCellListDataSource>();
                _enabledCommunitiesList.IsEnabledList = true;
                _enabledCommunitiesList.tableView = tableView;
                _enabledCommunitiesList.config = _config;
                _enabledCommunitiesList.CommunityMoved += CommunityDidMove;
                tableView.SetDataSource(_enabledCommunitiesList, false);
            }

            {
                var tableView = _disabledCommunitiesBSMLList.tableView;
                var go = _disabledCommunitiesBSMLList.gameObject;
                Destroy(_disabledCommunitiesBSMLList);
                _disabledCommunitiesBSMLList = null;
                _disabledCommunitiesList = go.AddComponent<CommunityConfigurationCellListDataSource>();
                _disabledCommunitiesList.IsEnabledList = false;
                _disabledCommunitiesList.tableView = tableView;
                _disabledCommunitiesList.config = _config;
                _disabledCommunitiesList.CommunityMoved += CommunityDidMove;
                tableView.SetDataSource(_disabledCommunitiesList, false);
            }
        }

        void RefreshLists()
        {
            _enabledCommunitiesList.tableView.ReloadDataKeepingPosition();
            _disabledCommunitiesList.tableView.ReloadDataKeepingPosition();
        }

        void CommunityDidMove(CommunityConfigurationCell cell, CommunityConfigurationCell.MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case CommunityConfigurationCell.MoveDirection.Left:
                    EnableCommunity(cell);
                    break;
                case CommunityConfigurationCell.MoveDirection.Right:
                    DisableCommunity(cell);
                    break;
                case CommunityConfigurationCell.MoveDirection.Up:
                    MoveCommunity(cell, true);
                    break;
                case CommunityConfigurationCell.MoveDirection.Down:
                    MoveCommunity(cell, false);
                    break;
            }

            RefreshLists();
            _communitiesView.RefreshCommunitiesNoRequest();
        }

        void EnableCommunity(CommunityConfigurationCell cell)
        {
            var idx = _config.DisabledCommunities.FindIndex(comm => comm.communityName == cell.CommunityName);
            if (idx < 0) return; // was not a disabled community

            // remove from disabled and add to enabled
            _config.EnabledCommunities.Add(_config.DisabledCommunities[idx]);
            _config.DisabledCommunities.RemoveAt(idx);
        }

        void DisableCommunity(CommunityConfigurationCell cell)
        {
            var idx = _config.EnabledCommunities.FindIndex(comm => comm.communityName == cell.CommunityName);
            if (idx < 0) return; // was not an enabled community

            // remove from enabled and add to disabled
            _config.DisabledCommunities.Add(_config.EnabledCommunities[idx]);
            _config.EnabledCommunities.RemoveAt(idx);
        }
        
        void MoveCommunity(CommunityConfigurationCell cell, bool up)
        {
            var idx = _config.EnabledCommunities.FindIndex(comm => comm.communityName == cell.CommunityName);
            if (idx < 0) return; // was not an enabled community

            if (up)
            {
                if (idx <= 0) return; // was already first

                // swap with idx - 1
                var other = _config.EnabledCommunities[idx - 1];
                _config.EnabledCommunities[idx - 1] = _config.EnabledCommunities[idx];
                _config.EnabledCommunities[idx] = other;
            }
            else
            {
                if (idx >= _config.EnabledCommunities.Count - 1) return; // was already last
                
                // swap with idx + 1
                var other = _config.EnabledCommunities[idx + 1];
                _config.EnabledCommunities[idx + 1] = _config.EnabledCommunities[idx];
                _config.EnabledCommunities[idx] = other;
            }
        }

        internal class CommunityConfigurationCellListDataSource : UnityEngine.MonoBehaviour, HMUI.TableView.IDataSource
        {
            static readonly string ReuseIdentifier = "CommunityConfigurationCell";

            public event Action<CommunityConfigurationCell, CommunityConfigurationCell.MoveDirection> CommunityMoved;
            
            public TableView tableView { get; internal set; }
            public Config config { get; internal set; }
            public bool IsEnabledList { get; internal set; }
            List<Community> Communities => IsEnabledList ? config?.EnabledCommunities : config?.DisabledCommunities;

            public TableCell CellForIdx(TableView tableView, int idx)
            {
                var cell = tableView.DequeueReusableCellForIdentifier(ReuseIdentifier) as CommunityConfigurationCell;
                if (cell == null)
                {
                    cell = CommunityConfigurationCell.GetCell();
                    cell.reuseIdentifier = ReuseIdentifier;
                    cell.CommunityMovedCallback = CommunityMoved.Invoke;
                    cell.interactable = false;
                }

                var community = Communities[idx];

                return cell.SetData(community.communityName, community.communityBackgroundURL).SetIsEnabledCell(IsEnabledList);
            }

            public float CellSize() => 15.0f;
            public int NumberOfCells() {
                return Communities?.Count ?? 0;
            }
        }

        internal class CommunityConfigurationCell : HMUI.TableCell
        {
            bool _isEnabledCell;

            public string CommunityName => _communityName.text;

            [UIComponent("layout")]
            Transform _layout;
            [UIComponent("mask")]
            ImageView _mask;
            [UIComponent("community-background")]
            ImageView _background;
            [UIComponent("community-name")]
            TMPro.TextMeshProUGUI _communityName;
            [UIComponent("enable-button")]
            Button _enableButton;
            [UIComponent("disable-button")]
            Button _disableButton;
            [UIComponent("up-button")]
            Button _moveUpButton;
            [UIComponent("down-button")]
            Button _moveDownButton;
            [UIComponent("move-buttons")]
            Transform _moveButtons;

            public Action<CommunityConfigurationCell, MoveDirection> CommunityMovedCallback { get; internal set; }
            public static CommunityConfigurationCell GetCell()
            {
                var go = new GameObject("CommunityConfigurationCell");
                go.AddComponent<Touchable>();

                var cell = go.AddComponent<CommunityConfigurationCell>();
                string content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.CommunityConfigurationCell.bsml");
                BSMLParser.instance.Parse(content, go, cell);
                
                cell._mask.rectTransform.sizeDelta = new Vector2(30, 15);
                var mask = cell._mask.gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;

                cell._mask.color = Color.white;
                cell._mask.type = Image.Type.Sliced;

                cell._mask.raycastTarget = false;
                cell._background.raycastTarget = false;
                cell._communityName.raycastTarget = false;

                return cell;
            }

            public override void SelectionDidChange(TransitionType transitionType) => UpdateHighlight();
            public override void HighlightDidChange(TransitionType transitionType) => UpdateHighlight();

            public CommunityConfigurationCell SetData(string communityName, string communityBackgroundURL)
            {
                _communityName.text = communityName;
                _background.SetImageAsync(communityBackgroundURL);
                UpdateHighlight();

                return this;
            }

            public CommunityConfigurationCell SetIsEnabledCell(bool isEnabledCell)
            {
                _isEnabledCell = isEnabledCell;

                _disableButton.gameObject.SetActive(_isEnabledCell);
                _enableButton.gameObject.SetActive(!_isEnabledCell);
                _moveButtons.gameObject.SetActive(_isEnabledCell);
                _layout.GetComponent<LayoutElement>().preferredWidth = _isEnabledCell ? 45 : 35;
                UpdateHighlight();

                return this;
            }

            void UpdateHighlight()
            {
                _moveUpButton.gameObject.SetActive(_isEnabledCell && (highlighted || selected));
                _moveDownButton.gameObject.SetActive(_isEnabledCell && (highlighted || selected));

                _background.color = highlighted || selected ? Color.white : Color.gray;
                _communityName.enabled = !(highlighted || selected);
            }

            [UIAction("move-left")] 
            void MoveLeft() => CommunityMovedCallback?.Invoke(this, MoveDirection.Left);

            [UIAction("move-right")] 
            void MoveRight() => CommunityMovedCallback?.Invoke(this, MoveDirection.Right);

            [UIAction("move-up")] 
            void MoveUp() => CommunityMovedCallback?.Invoke(this, MoveDirection.Up);

            [UIAction("move-down")] 
            void MoveDown() => CommunityMovedCallback?.Invoke(this, MoveDirection.Down);

            public enum MoveDirection
            {
                Left,
                Right,
                Up,
                Down
            }
        }
    }
}
