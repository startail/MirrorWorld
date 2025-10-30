using System.IO;
using UnityEngine;

namespace SharedDomain.Repositories
{
    // IProgressRepository の具体的なファイル入出力実装
    public class FileProgressRepository : IProgressRepository
    {
        private const string FileName = "save_progress.json";
        private readonly string _savePath;

        public FileProgressRepository()
        {
            // Unityの永続データパスとファイル名を結合
            _savePath = Path.Combine(Application.persistentDataPath, FileName);
            Debug.Log($"Save file path set to: {_savePath}");
        }

        public void Save(GameProgressData data)
        {
            try
            {
                // データをJSON形式にシリアライズ
                string json = JsonUtility.ToJson(data, true);
                
                // ファイルに書き込み
                File.WriteAllText(_savePath, json);
                Debug.Log("Game progress successfully saved.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save progress data: {e.Message}");
            }
        }

        public GameProgressData Load()
        {
            if (!File.Exists(_savePath))
            {
                Debug.Log("Save file not found. Returning new default data.");
                // ファイルが存在しない場合は、新しい初期データを返す
                return new GameProgressData(); 
            }

            try
            {
                // ファイルからJSONデータを読み込み
                string json = File.ReadAllText(_savePath);
                
                // JSONを GameProgressData オブジェクトにデシリアライズ
                GameProgressData loadedData = JsonUtility.FromJson<GameProgressData>(json);
                Debug.Log("Game progress successfully loaded.");
                return loadedData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load progress data: {e.Message}. Returning new default data.");
                // 読み込みエラーの場合は、新しい初期データを返す（データ破損対応）
                return new GameProgressData();
            }
        }
    }
}