using DefaultNamespace;
using Features.Credit.Presentation.Interfaces;
using Features.Title.Presentation.Interfaces;
using SharedPresentation.Interfaces;
using UnityEngine;

namespace Features.Credit.Presentation.Views
{
    public class CreditView : MonoBehaviour, ICreditView, IGenericView
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
    public class CreditViewSet
    {
        public RuntimePlatform platform;
        public CreditView creditView;
    }
}