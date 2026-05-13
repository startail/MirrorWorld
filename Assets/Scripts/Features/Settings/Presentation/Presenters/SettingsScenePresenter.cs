using System;
using AnnulusGames.SceneSystem;
using Features.Settings.Presentation.Interfaces;
using Infrastructure.Services;
using R3;
using VContainer;
using VContainer.Unity;

namespace Features.Settings.Presentation.Presenters
{
    public class SettingsScenePresenter : IPostInitializable, IDisposable
    {
        [Inject] private readonly SceneService sceneService;
        [Inject] private readonly AudioService audioService;
        [Inject] private readonly SettingsDataManager settingsDataManager;
        [Inject] private readonly ISettingsView settingsView;

        private readonly CompositeDisposable _disposables = new();

        public void PostInitialize()
        {
            Observable.FromEvent(
                    h => settingsView.BackButton.onPointerUp += h,
                    h => settingsView.BackButton.onPointerUp -= h)
                .Subscribe(_ => sceneService.PopScene())
                .AddTo(_disposables);
        }

        public void ChangeToNextLanguage()
        {
            for (int index = 0; index < settingsDataManager.languageSets.Count; index++)
            {
                if (settingsDataManager.languageSets[index].languageCode == settingsDataManager.currentLanguageSet.languageCode)
                {
                    int next = index == settingsDataManager.languageSets.Count - 1 ? 0 : index + 1;
                    settingsDataManager.SetCurrentLanguageSet(settingsDataManager.languageSets[next]);
                    break;
                }
            }
        }

        public void Dispose() => _disposables.Dispose();
    }
}
