using UnityEngine;

namespace SharedDomain.Repositories
{
    // IProgressRepository の PlayerPrefs 実装
    public class PlayerPrefsProgressRepository : IProgressRepository
    {
        // PlayerPrefsに保存するキー
        private const string SaveDataKey = "GameSaveData_V1"; 

        public void Save(GameProgressData data)
        {
            try
            {
                // データをJSON形式にシリアライズ
                string json = JsonUtility.ToJson(data);
                
                // PlayerPrefsに文字列として保存
                PlayerPrefs.SetString(SaveDataKey, json);
                
                // 確実に書き込みを完了させる
                PlayerPrefs.Save(); 
                
                Debug.Log("Game progress successfully saved to PlayerPrefs.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save progress data to PlayerPrefs: {e.Message}");
            }
        }

        public GameProgressData Load()
        {
            // 指定されたキーのデータが存在するか確認
            if (!PlayerPrefs.HasKey(SaveDataKey))
            {
                Debug.Log("Save data not found in PlayerPrefs. Returning new default data.");
                // データが存在しない場合は、新しい初期データを返す
                return new GameProgressData(); 
            }

            try
            {
                // PlayerPrefsからJSON文字列を読み込み
                string json = PlayerPrefs.GetString(SaveDataKey);
                
                // JSONを GameProgressData オブジェクトにデシリアライズ
                GameProgressData loadedData = JsonUtility.FromJson<GameProgressData>(json);
                
                // デシリアライズが null になった場合（データ破損）のチェック
                if (loadedData == null)
                {
                    Debug.LogError("PlayerPrefs data corrupted or invalid. Returning new default data.");
                    return new GameProgressData();
                }
                
                Debug.Log("Game progress successfully loaded from PlayerPrefs.");
                return loadedData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load progress data from PlayerPrefs: {e.Message}. Returning new default data.");
                // 読み込みエラーの場合は、新しい初期データを返す
                return new GameProgressData();
            }
        }
    }
}