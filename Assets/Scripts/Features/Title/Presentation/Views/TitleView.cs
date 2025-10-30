using DefaultNamespace;
using Features.Title.Presentation.Interfaces;
using SharedPresentation.Interfaces;
using UnityEngine;

namespace Features.Title.Presentation.Views
{
    public class TitleView : MonoBehaviour, ITitleView, IGenericView
    {
        [SerializeField] public GenericButton toSettingsButton;
        [SerializeField] public GenericButton toCreditButton;
        [SerializeField] public GenericButton toMainMenuButton;

        public GenericButton ToSettingsButton => toSettingsButton;
        public GenericButton ToCreditButton => toCreditButton;
        public GenericButton ToMainMenuButton => toMainMenuButton;

        public void Show()
        {
            toSettingsButton.gameObject.SetActive(true);
            toCreditButton.gameObject.SetActive(true);
            toMainMenuButton.gameObject.SetActive(true);
        }

        public void Hide()
        {
            toSettingsButton.gameObject.SetActive(false);
            toCreditButton.gameObject.SetActive(false);
            toMainMenuButton.gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class TitleViewSet
    {
        public RuntimePlatform platform;
        public TitleView titleView;
    }
}