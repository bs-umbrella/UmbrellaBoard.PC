using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UmbrellaBoard.UI.Carousel
{
    internal class HoverDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
    {
        internal event Action Enter;
        internal event Action Exit;

        public void OnPointerEnter(PointerEventData eventData) => Enter.Invoke();
        public void OnPointerExit(PointerEventData eventData) => Exit.Invoke();
    }
}
