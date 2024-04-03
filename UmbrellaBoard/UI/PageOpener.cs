using System.Reflection;
using UnityEngine;

namespace UmbrellaBoard.UI
{
    internal class PageOpener : MonoBehaviour
    {
        internal string page;
        internal object host;
        internal Component activationSource;
        internal bool openInBrowser;

        internal void OpenPage()
        {
            if (host == null || page == null)
                return;

            if (openInBrowser)
                Application.OpenURL(page);

            //else
            // port this, reflection or something
            // https://github.com/bs-umbrella/UmbrellaBoard.Quest/blob/main/src/UI/PageOpener.cpp#L16-L17
        }
    }
}
