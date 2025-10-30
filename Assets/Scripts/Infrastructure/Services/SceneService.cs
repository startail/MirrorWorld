using System;
using System.Collections;
using System.Collections.Generic;
using AnnulusGames.SceneSystem;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Services
{
    public enum SceneKey
    {
        None,
        Base,
        Title,
        Settings,
        Credit,
        MainMenu,
        Ingame,
        StoryTelling,
        Gallery,
    }
    
    public class SceneService : ITickable
    {
        public bool isBuildDone = false;
        private SceneContainer container;
        private LoadSceneOperationHandle pushHandle;
        private MainLoadingScreen loadingScreenPrefab;
        private List<SceneKey> directSceneList = new List<SceneKey>();
        private SceneKey startSceneKey = SceneKey.None;
        public SceneKey peekSceneKey = SceneKey.None;
        private SceneKey beforeDirectPeekSceneKey = SceneKey.None;
        private UnityEvent afterCreateContainer = new UnityEvent();
        private bool isPushCalling = false;
        private bool isDirect = false;
        private SceneKey pushKey = SceneKey.None;
        private bool needLoadingScreen = true;
        private bool isPopCalling = false;
        private SceneController controller;
        
        // DI
        [Inject]
        public SceneService(MainLoadingScreen loadingScreenPrefab, SceneController controller)
        {
            this.loadingScreenPrefab = loadingScreenPrefab;
            this.controller = controller;
            controller.StartCoroutine(CreateContainer());
        }
        
        public void Tick()
        {
            if (isBuildDone && isPushCalling)
            {
                if (isDirect)
                {
                    controller.StartCoroutine(PushSceneToDirect(pushKey, needLoadingScreen) );
                    isDirect = false;
                }
                else
                {
                    controller.StartCoroutine(PushSceneToContainer(pushKey, needLoadingScreen));
                }
                isPushCalling = false;
                pushKey = SceneKey.None;
            }
            if (isBuildDone && isPopCalling)
            {
                controller.StartCoroutine(PopSceneFromContainer());
                isPopCalling = false;
            }
        }
        
        public void PushScene(SceneKey key, bool isDirect = false, bool needLoadingScreen = false)
        {
            if (isPushCalling) return;
            isPushCalling = true;
            this.isDirect = isDirect;
            this.needLoadingScreen = needLoadingScreen;
            pushKey = key;
        }
        
        public void PopScene()
        {
            if (isPopCalling) return;
            isPopCalling = true;
        }
        
        private IEnumerator CreateContainer()
        {
            container = SceneContainer.Create();
            container.Register(SceneKey.Base.ToString(), 0, 0);
            container.Register(SceneKey.Title.ToString(), 1, 1);
            container.Register(SceneKey.Settings.ToString(), 2, 2);
            container.Register(SceneKey.Credit.ToString(), 3,2);
            container.Register(SceneKey.MainMenu.ToString(), 4,3);
            container.Register(SceneKey.Ingame.ToString(), 5,4);
            container.Register(SceneKey.StoryTelling.ToString(), 6,5);
            container.Register(SceneKey.Gallery.ToString(), 7,4);
            
            container.OnAfterPop += (newPeek,poped) =>
            {
                Debug.Log(poped+" Poped > New Peek is "+newPeek);
                if (newPeek == null)
                {
                    peekSceneKey = startSceneKey;
                }
                else
                {
                    peekSceneKey = (SceneKey)Enum.Parse(typeof(SceneKey),newPeek);
                }
            };
            
            isBuildDone = true;
            var containerBuildHandle = container.Build();
            
            if (SceneManager.GetSceneByName("BaseScene").IsValid())
            {
                startSceneKey = SceneKey.Base;
                peekSceneKey = SceneKey.Title;
            }
            else
            {
                startSceneKey = (SceneKey)Enum.Parse(typeof(SceneKey),SceneManager.GetActiveScene().name.Split("Scene")[0]);
                peekSceneKey = startSceneKey;
                Scenes.LoadSceneAsync("BaseScene");
            }
            
            afterCreateContainer.Invoke();
            yield return containerBuildHandle.ToYieldInteraction();
        }
        
        public float GetProgress()
        {
            if (pushHandle.IsDone) return 1.0f;
            return pushHandle.Progress;
        }

        private IEnumerator PushSceneToContainer(SceneKey key, bool needLoadingScreen = true)
        {
            if( SceneKey.None == key) yield break;
            peekSceneKey = key;

            if (needLoadingScreen)
            {
                var loadingScreen = GameObject.Instantiate(loadingScreenPrefab);
                GameObject.DontDestroyOnLoad(loadingScreen);
                pushHandle = container.Push(key.ToString()).WithLoadingScreen(loadingScreen.GetComponent<MainLoadingScreen>().loadingScreen);
                yield return pushHandle.ToYieldInteraction();
            }
            else
            {
                pushHandle = container.Push(key.ToString());
                yield return pushHandle.ToYieldInteraction();
            }
        }
        
        public IEnumerator PushSceneToDirect(SceneKey key, bool needLoadingScreen = true)
        {
            directSceneList.Add(key);
            beforeDirectPeekSceneKey = peekSceneKey;
            peekSceneKey = key;
            
            if( SceneKey.None == key) yield break;

            if (needLoadingScreen)
            {
                var loadingScreen = GameObject.Instantiate(loadingScreenPrefab);
                GameObject.DontDestroyOnLoad(loadingScreen);
                pushHandle = Scenes.LoadSceneAsync(key+"Scene", LoadSceneMode.Additive ).WithLoadingScreen(loadingScreen.GetComponent<MainLoadingScreen>().loadingScreen);
                yield return pushHandle.ToYieldInteraction();
            }
            else
            {
                pushHandle = Scenes.LoadSceneAsync(key+"Scene", LoadSceneMode.Additive );
                yield return pushHandle.ToYieldInteraction();
            }
        }
        
        private IEnumerator PopSceneFromContainer()
        {
            if( directSceneList.Count > 0)
            {
                if (directSceneList.Count == 1)
                {
                    peekSceneKey = beforeDirectPeekSceneKey;
                }
                else
                {
                    peekSceneKey = directSceneList[directSceneList.Count - 2];
                }
                yield return PopSceneFromDirect(directSceneList[directSceneList.Count-1]);
                yield break;
            }
            var handle = container.Pop();
            yield return handle.ToYieldInteraction();
        }
        
        private IEnumerator PopSceneFromDirect(SceneKey key)
        {
            if (!directSceneList.Contains(key) || !SceneManager.GetSceneByName(key+"Scene").IsValid() ) yield break;
            directSceneList.Remove(key);
            var handle = Scenes.UnloadSceneAsync(key+"Scene");
            yield return handle.ToYieldInteraction();
        }
    }
}