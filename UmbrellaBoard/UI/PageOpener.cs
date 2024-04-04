using System;
using System.Reflection;
using UnityEngine;

namespace UmbrellaBoard.UI
{
    internal class PageOpener : MonoBehaviour
    {
        public string Page { get; set; }
        public Component ActivationSource { get; internal set; }
        public bool OpenInBrowser { get; set; }
        public event Action<string> OpenPageEvent;

        internal void OpenPage()
        {
            if (String.IsNullOrEmpty(Page)) return;

            if (OpenInBrowser)
                Application.OpenURL(Page);
            else
                OpenPageEvent?.Invoke(Page);
        }
    }
}
