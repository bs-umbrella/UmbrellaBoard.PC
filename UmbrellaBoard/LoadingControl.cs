using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using ModestTree;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace UmbrellaBoard
{
    internal class LoadingControl : MonoBehaviour
    {
        private GameObject _loadingContent;
        [UIValue("_loadingText")] private TextMeshProUGUI _loadingText;
        private GameObject _errorContent;
        [UIValue("_errorText")] private TextMeshProUGUI _errorText;

        private void Awake()
        {
            string loadingContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.LoadingContent.bsml");
            string errorContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.ErrorContent.bsml");

            BSMLParser.instance.Parse(loadingContent, gameObject, this);
            BSMLParser.instance.Parse(errorContent, gameObject, this);
        }

        internal void ShowLoading(bool isLoading, string loadingText = "")
        {
            if (isLoading && loadingText.IsEmpty())
                loadingText = "Loading...";

            _loadingText.text = loadingText;
            _loadingContent.SetActive(isLoading);
            _errorContent.SetActive(false);
        }

        internal void ShowError(string errorText = "")
        {
            if (errorText.IsEmpty())
                errorText = "An error occurred!";

            _errorText.text = errorText;
            _loadingContent.SetActive(false);
            _errorContent.SetActive(true);
        }
    }
}
