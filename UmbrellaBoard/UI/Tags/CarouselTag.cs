using BeatSaberMarkupLanguage.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UmbrellaBoard.UI.Tags
{
    internal class CarouselTag : BSMLTag
    {
        public override string[] Aliases => new []{ "carousel", "bubble-carousel" };

        public override GameObject CreateObject(Transform parent)
        {
            throw new NotImplementedException();
        }
    }
}
