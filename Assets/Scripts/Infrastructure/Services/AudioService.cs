using UnityEngine;
using KanKikuchi.AudioManager;
using VContainer;

namespace Infrastructure.Services
{
    public class AudioService
    {
        [Inject] private SceneService sceneService;
        
        public void PlayBGM(string bgmPath, bool isLoop = true)
        {
            var path = bgmPath.Replace("BGM/", "");
            if (!sceneService.isBuildDone) return;
            if (BGMManager.Instance.GetCurrentAudioNames().Contains(path)) return;
            BGMManager.Instance.Play(bgmPath, isLoop: isLoop);
        }
        
        public void StopBGM()
        {
            BGMManager.Instance.Stop();
        }
        
        public void FadeOutBGM(float fadeTime = 1f)
        {
            BGMManager.Instance.FadeOut(fadeTime);
        }
        
        public void PauseBGM()
        {
            BGMManager.Instance.Pause();
        }
        
        public void ResumeBGM()
        {
            BGMManager.Instance.UnPause();
        }

        public bool IsBGMPlaying()
        {
            return BGMManager.Instance.IsPlaying();
        }
        
        public bool IsBGMPassExist(string bgmPath)
        {
            var path = bgmPath.Replace("BGM/", "");
            return BGMManager.Instance.GetCurrentAudioNames().Contains(path);
        }
        
        public bool IsSEPassExist(string sePath)
        {
            var path = sePath.Replace("SE/", "");
            return SEManager.Instance.GetCurrentAudioNames().Contains(path);
        }
        
        public void PlaySE(string sePath, float pitch = 1.0f)
        {
            if (pitch > 1.0f) pitch = Mathf.Pow(pitch, 0.3f); // pitchの調整
            if (!sceneService.isBuildDone) return;
            SEManager.Instance.Play(sePath, pitch:pitch);
        }

        public void StopSE(string sePath)
        {
            SEManager.Instance.Stop(sePath);
        }
        
        public void ChangeBGMVolume(float volume)
        {
            BGMManager.Instance.ChangeBaseVolume(volume);
        }
        
        public void ChangeSEVolume(float volume)
        {
            SEManager.Instance.ChangeBaseVolume(volume);
        }
    }
}