using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace UmbrellaBoard.Models
{
    internal class CommunityCell : TableCell
    {
        private TextMeshProUGUI _communityName;
        private ImageView _communityBackground;
        internal string _communityURL { get; private set; }

        public override void HighlightDidChange(TransitionType transitionType) => UpdateHighlight();
        public override void SelectionDidChange(TransitionType transitionType) => UpdateHighlight();

        private void UpdateHighlight() => _communityBackground.color = highlighted || selected ? new Color(1, 1, 1, 1) : new Color(.6f, .6f, .6f, 1);

        internal CommunityCell SetData(string communityName, string communityURL, Sprite background = null)
        {
            _communityName.text = communityName;
            _communityURL = communityURL;
            _communityBackground.sprite = background;
            _communityBackground.color = new Color(.8f, .8f, .8f, 1);
            return this;
        }

        internal CommunityCell SetData(string communityName, string communityURL, string backgroundURL)
        {
            _communityName.text = communityName;
            _communityURL = communityURL;
            _communityBackground.SetImageAsync(backgroundURL);
            _communityBackground.color = new Color(.8f, .8f, .8f, 1);
            return this;
        }

        internal static CommunityCell GetCell()
        {
            GameObject go = new GameObject("CommunityCell");
            CommunityCell cell = go.AddComponent<CommunityCell>();
            BSMLParser.instance.Parse(
                "<stack pref-width='20' pref-height='10' vertical-fit='PreferredSize' horizontal-fit='PreferredSize'><img id='_communityBackground'/><text id='_communityName' font-size='3' font-align='Center'/></stack>",
                cell.gameObject, 
                cell
            );
            return cell;
        }
    }
}
