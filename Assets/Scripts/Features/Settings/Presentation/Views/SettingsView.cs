using DefaultNamespace;
using Features.Credit.Presentation.Interfaces;
using Features.Credit.Presentation.Views;
using Features.Settings.Presentation.Interfaces;
using Features.Title.Presentation.Interfaces;
using SharedPresentation.Interfaces;
using UnityEngine;

namespace Features.Settings.Presentation.Views
{
    public class SettingsView : MonoBehaviour, ISettingsView, IGenericView
    {
        [SerializeField] public GenericButton backButton;

        public GenericButton BackButton => backButton;

        public void Show()
        {
            backButton.gameObject.SetActive(true);
        }

        public void Hide()
        {
            backButton.gameObject.SetActive(false);
        }

    }

    [System.Serializable]
    public class SettingsViewSet
    {
        public RuntimePlatform platform;
        public SettingsView settingsView;
    }
}