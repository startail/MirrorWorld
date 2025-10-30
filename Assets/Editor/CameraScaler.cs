using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public static class CameraScaler
{
    // メニューパスを定義します。Tools/Camera/Apply Game View Scale として登録されます。
    [MenuItem("Tools/Camera/Apply Game View Scale &C", false, 50)]
    private static void ApplyGameViewScaleToCamera()
    {
        // 1. MainCameraの取得
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MainCameraが見つかりません。シーンに'MainCamera'タグの付いたカメラが存在することを確認してください。");
            return;
        }

        // 2. ProjectionがOrthographic（正投影）であることの確認
        if (mainCamera.orthographic == false)
        {
            Debug.LogError("MainCameraのProjectionがOrthographicではありません。Sizeプロパティは正投影でのみ有効です。");
            return;
        }

        // 3. GameViewの解像度情報を取得
        // GameViewのWindowインスタンス自体は内部クラスのため、反射(Reflection)を利用してサイズを取得します。
        Vector2 gameViewSize = GetMainGameViewSize();

        if (gameViewSize.x <= 0 || gameViewSize.y <= 0)
        {
            Debug.LogError("Game Viewのサイズを取得できませんでした。Game Viewウィンドウが開いていることを確認してください。");
            return;
        }

        // 4. 現在のカメラ設定に基づいて新しいorthographicSizeを計算
        // ターゲットとするゲーム内の「縦幅のサイズ」（Height）を求め、orthographicSizeを決定します。
        
        // orthographicSizeの計算基準とするワールドの幅（例: 16単位）を定義
        // この値を基準に、Game Viewのアスペクト比に合わせてSizeを調整します。
        const float TargetWorldLongSide = 480f;
        const float TargetWorldShortSide = 270f;
        
        // Game Viewの縦横比 (アスペクト比)
        float gameViewAspect = gameViewSize.x / gameViewSize.y;
        float targetAspect = (gameViewAspect >= 1.0f) ? TargetWorldLongSide / TargetWorldShortSide : TargetWorldShortSide / TargetWorldLongSide;
        float targetWidth = (gameViewAspect >= 1.0f) ? TargetWorldLongSide : TargetWorldShortSide;
        
        
        // ターゲットワールド幅に基づいて、新しいワールドの縦幅を計算
        // NewWorldHeight = TargetWorldWidth / GameViewAspect
        float newWorldHeight = targetWidth / targetAspect;//gameViewAspect;
        
        // orthographicSize はワールドの縦幅の「半分」
        float newOrthographicSize = newWorldHeight / 2.0f;


        // 5. MainCameraに適用
        // Undoシステムに登録し、操作を元に戻せるようにします。
        Undo.RecordObject(mainCamera, "Apply Game View Scale to Camera Size");
        
        mainCamera.orthographicSize = newOrthographicSize;

        /*Debug.Log($"MainCamera Sizeを調整しました。\n" +
                  $"Game View Resolution: {gameViewSize.x}x{gameViewSize.y}\n" +
                  $"Aspect Ratio: {gameViewAspect:F2}\n" +
                  $"New Orthographic Size: {newOrthographicSize:F4}");*/

        // 6. CanvasScalerにも同様のスケールを適用
        ApplyGameViewScaleToCanvasScaler(targetWidth, newWorldHeight);
    }

    /// <summary>
    /// 反射を使用して開いているGame Viewのピクセルサイズを取得します。
    /// </summary>
    private static Vector2 GetMainGameViewSize()
    {
        // UnityEditor.GameView 型のウィンドウを探す
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        
        // シーン内にGameViewウィンドウが存在するか確認
        EditorWindow gameView = EditorWindow.GetWindow(T, false, "Game");
        
        if (gameView == null) return Vector2.zero;

        // Game Viewの内部プロパティ（m_Pos）からサイズを取得
        // Unityバージョンによって内部クラス名やフィールド名が変わる可能性があるため注意が必要です。
        
        // 現在のサイズを直接取得できるプロパティ
        return gameView.rootVisualElement.contentRect.size;
    }
    
    // ターゲットとするGameObjectのタグ名
    private const string TargetTag = "MainCanvas";
    
    private static void ApplyGameViewScaleToCanvasScaler(float worldWidth,float worldHeight)
    {
        // 1. 指定されたタグを持つ全てのGameObjectを取得
        // FindGameObjectsWithTag()はアクティブなGameObjectのみを検索します
        GameObject[] canvasObjects = GameObject.FindGameObjectsWithTag(TargetTag);

        if (canvasObjects.Length == 0)
        {
            Debug.LogError($"タグ '{TargetTag}' を持つGameObjectが見つかりませんでした。");
            return;
        }

        int changesCount = 0;
        
        // 2. 各GameObjectをループ処理
        foreach (GameObject canvasObj in canvasObjects)
        {
            // CanvasScalerコンポーネントを取得
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();

            if (scaler != null)
            {
                // 3. Undoシステムに登録し、操作を元に戻せるようにする
                Undo.RecordObject(scaler, $"Change Reference Resolution on {canvasObj.name}");
                
                // 4. Reference Resolutionを新しい値に設定
                Vector2 NewReferenceResolution = new Vector2(worldWidth, worldHeight);
                scaler.referenceResolution = NewReferenceResolution;

                // 5. Editorにこのアセットが変更されたことを通知し、保存が必要であることを示す
                EditorUtility.SetDirty(scaler);
                
                Debug.Log($"'{canvasObj.name}' の Reference Resolution を {NewReferenceResolution.x}x{NewReferenceResolution.y} に変更しました。");
                changesCount++;
            }
            else
            {
                Debug.LogWarning($"タグ '{TargetTag}' を持つGameObject '{canvasObj.name}' に CanvasScaler コンポーネントが見つかりませんでした。");
            }
        }

        if (changesCount > 0)
        {
            Debug.Log($"処理が完了しました。{changesCount}個のCanvasScalerが更新されました。");
        }    
    }

}