using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UmbrellaBoard.UI.Carousel
{
    internal class CarouselBubble : MonoBehaviour
    {
        ImageView _image;
        internal ImageView Image 
        { 
            get
            {
                if (_image == null) _image = GetComponentInChildren<ImageView>(true);
                return _image;
            }
            private set { _image = value; }
        }

        bool _highlighted;
        internal bool Highlighted
        {
            get => _highlighted;
            set
            {
                _highlighted = value;
                UpdateHighlight();
            }
        }

        Color _highlighColor = Color.white;
        internal Color HighlightColour
        {
            get => _highlighColor;
            set 
            { 
                _highlighColor = value; 
                UpdateHighlight(); 
            }
        }

        Color _defaultColor = Color.gray;
        internal Color DefaultColour
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                UpdateHighlight();
            }
        }

        internal void UpdateHighlight()
        {
            Image.color = Highlighted ? HighlightColour : DefaultColour;
            Image.transform.localScale = Highlighted ? new Vector3(1, 1, 1) : new Vector3(.5f, .5f, .5f);
        }
    }
}
