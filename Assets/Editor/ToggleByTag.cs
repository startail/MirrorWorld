using UnityEditor;
using UnityEngine;
using System.Linq; // LINQを使用するために追加

public static class ToggleByTag
{
    private const string TagA = "Windows";
    private const string TagB = "Android";
    private const string TagC = "WebGL";

    [MenuItem("Tools/Toggle/Windows &1", false, 10)]
    private static void ToggleTagA() => ToggleObjectsByTag(TagA);

    [MenuItem("Tools/Toggle/Android &2", false, 11)]
    private static void ToggleTagB() => ToggleObjectsByTag(TagB);
    
    [MenuItem("Tools/Toggle/WebGL &3", false, 12)]
    private static void ToggleTagC() => ToggleObjectsByTag(TagC);

    private static void ToggleObjectsByTag(string targetTag)
    {
        // 1. 【変更点】シーン内の全GameObject（非アクティブも含む）を検索
        //    FindObjectsOfTypeAllはEditorOnlyであり、非アクティブなオブジェクトやPrefabも含めて検索可能
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
        // 2. ターゲットのタグを持つオブジェクトにフィルタリング
        GameObject[] objectsToToggle = allObjects
            .Where(obj => obj.CompareTag(targetTag) && 
                          // ヒエラルキー内のオブジェクトに限定（Prefabアセットなどは除く）
                          obj.hideFlags == HideFlags.None) 
            .ToArray();

        if (objectsToToggle.Length == 0)
        {
            Debug.LogWarning($"[ToggleByTag] No active or inactive GameObjects found with tag: {targetTag}.");
            return;
        }

        // 3. 最初のオブジェクトのアクティブ状態を基準として取得
        //    FindObjectsOfTypeAllは無関係なオブジェクトも含めるため、ヒエラルキー内のオブジェクトかどうかを判定するロジックが必要です
        
        // 処理の対象となるオブジェクトの中から、現在の状態を判断
        bool currentState = objectsToToggle.First().activeSelf;
        bool newState = !currentState;

        // 4. 全てのオブジェクトのSetActiveを切り替えます。
        foreach (GameObject obj in objectsToToggle)
        {
            // Undoの登録
            Undo.RecordObject(obj, $"Toggle Active State of {targetTag} to {newState}");
            
            // 状態を切り替えます。
            obj.SetActive(newState);
        }

        Debug.Log($"[ToggleByTag] Switched {objectsToToggle.Length} objects with tag '{targetTag}' to {(newState ? "ON (Active)" : "OFF (Inactive)")}.");
    }
}