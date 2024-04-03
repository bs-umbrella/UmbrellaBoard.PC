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
        [UIComponent("loading-content")]
        private Transform _loadingContent;
        private GameObject LoadingContent => _loadingContent.gameObject;
        
        [UIComponent("loading-text")] 
        private TextMeshProUGUI _loadingText;
        
        [UIComponent("error-content")]
        private Transform _errorContent;
        private GameObject ErrorContent => _errorContent.gameObject;
        
        [UIComponent("_errorText")] 
        private TextMeshProUGUI _errorText;

        private void Awake()
        {
            string loadingContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.LoadingContent.bsml");
            string errorContent = Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "UmbrellaBoard.Assets.ErrorContent.bsml");

            BSMLParser.instance.Parse(loadingContent, gameObject, this);
            BSMLParser.instance.Parse(errorContent, gameObject, this);
            ShowLoading(false);
        }

        internal void ShowLoading(bool isLoading, string loadingText = "")
        {
            if (isLoading && loadingText.IsEmpty())
                loadingText = "Loading...";

            _loadingText.text = loadingText;
            LoadingContent.SetActive(isLoading);
            ErrorContent.SetActive(false);
        }

        internal void ShowError(string errorText = "")
        {
            if (errorText.IsEmpty())
                errorText = "An error occurred!";

            _errorText.text = errorText;
            LoadingContent.SetActive(false);
            ErrorContent.SetActive(true);
        }
    }
}
