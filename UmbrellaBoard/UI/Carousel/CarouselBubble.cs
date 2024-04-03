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
        internal ImageView Image 
        { 
            get
            {
                if (!Image)
                    Image = GetComponentInChildren<ImageView>(true);
                return Image;
            }
            private set { Image = value; }
        }
        internal bool Highlighted
        {
            get => Highlighted;
            set
            {
                Highlighted = value;
                UpdateHighlight();
            }
        }

        internal Color HighlightColour
        {
            get => HighlightColour;
            set 
            { 
                HighlightColour = value; 
                UpdateHighlight(); 
            }
        }

        internal Color DefaultColour
        {
            get => DefaultColour;
            set
            {
                DefaultColour = value;
                UpdateHighlight();
            }
        }


        internal void UpdateHighlight()
        {
            image.color = highlighted ? highlightColour : defaultColour;
            image.transform.localScale = highlighted ? new Vector3(1, 1, 1) : new Vector3(.5f, .5f, .5f);
        }
    }
}
