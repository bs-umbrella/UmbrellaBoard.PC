using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UmbrellaBoard.Models
{
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
            GameObject go = new GameObject("CommunityCell");
            CommunityCell cell = go.AddComponent<CommunityCell>();
            var content = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.CommunityCell.bsml");
            BSMLParser.instance.Parse(content, cell.gameObject, cell);

            cell._mask.rectTransform.sizeDelta = new Vector2(20, 10);
            var mask = cell._mask.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            cell._mask.type = Image.Type.Sliced;
            cell._mask.color = Color.white;
            cell._mask.material = Utilities.ImageResources.NoGlowMat;
            cell._communityBackground.raycastTarget = false;

            return cell;
        }
    }
}
