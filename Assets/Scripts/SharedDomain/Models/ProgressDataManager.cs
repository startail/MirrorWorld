using DefaultNamespace;
using SharedDomain.Repositories;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class ProgressDataManager
{
    [Inject] private readonly IProgressRepository progressRepository;
    
    public GameMainProgress gameProgress = GameMainProgress.Initial;

    [System.Flags]
    public enum GameMainProgress
    {
        Initial = 0,
        AfterInitial = 1 << 0,
    } 
    
    public void SaveProgress()
    {
        var data = new GameProgressData
        {
            gameMainProgressFlags = (int)gameProgress,
            lastSaveTime = System.DateTime.Now
        };
        progressRepository.Save(data);
    }

    public void LoadProgress()
    {
        GameProgressData data = progressRepository.Load();
        if (data != null)
        {
            gameProgress = (GameMainProgress)data.gameMainProgressFlags;
        }
        
        // 初回ロードでデータがInitialの場合、追加の初期化が必要ならここで実施
        if (gameProgress == GameMainProgress.Initial)
        {
            Initialize();
        }
        
        Debug.Log($"Progress loaded: {gameProgress}");
    }
    
    // 進行状況の更新ロジック
    public void Initialize()
    {
        gameProgress = GameMainProgress.Initial;
    }
    
    public void SetInitializeCompleted()
    {
        gameProgress |= GameMainProgress.AfterInitial;
    }
}
