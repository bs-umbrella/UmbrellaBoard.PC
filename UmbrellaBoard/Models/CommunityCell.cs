using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace UmbrellaBoard.Models
{
    internal class CommunityCell : TableCell
    {
        private TextMeshProUGUI _communityName;
        private ImageView _communityBackground;

        protected override void HighlightDidChange(TransitionType transitionType)
        {
            base.HighlightDidChange(transitionType);
        }

        protected override void SelectionDidChange(TransitionType transitionType)
        {
            base.SelectionDidChange(transitionType);
        }
    }
}
