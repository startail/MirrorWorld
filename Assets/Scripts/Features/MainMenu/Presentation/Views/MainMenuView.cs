using DefaultNamespace;
using Features.MainMenu.Presentation.Interfaces;
using SharedPresentation.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.MainMenu.Presentation.Views
{
    public class MainMenuView : MonoBehaviour, IMainMenuView, IGenericView
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
    public class MainMenuViewSet
    {
        public RuntimePlatform platform;
        public MainMenuView mainMenuView;
    }
}