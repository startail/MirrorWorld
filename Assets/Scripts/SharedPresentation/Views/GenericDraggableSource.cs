using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GenericDraggableSource : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public event Action onBeginDrag;
    public event Action onDrag;
    public event Action onEndDrag;
    public event Action onPointerEnter;
    public event Action onPointerExit;
    public event Action onSuccessed;
    public event Action onFailed;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        eventData.pointerDrag = gameObject;
        onBeginDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
    }
    
    public void NotifyDropResult(bool success)
    {
        if (success)
        {
            onSuccessed?.Invoke();
        }
        else
        {
            onFailed?.Invoke();
        }
    }
}

/*
// PresenterX.cs
public class PresenterX : MonoBehaviour
{
    // Model Dを保持（Bに紐づくデータ）
    [SerializeField] private MyModelD targetModelD; 
    
    // View Bを初期化時に購読
    public void Initialize(DroppableSource viewB, MyModelD modelD)
    {
        viewB.onDrop += HandleDrop;
        targetModelD = modelD;
        // ... 他の初期化処理 ...
    }

    private void HandleDrop(GameObject droppedObjectA)
    {
        // 1. View A（DraggableSource）とModel C（AssociatedModel）を取得
        if (!droppedObjectA.TryGetComponent(out DraggableSource viewA))
        {
            Debug.LogError("Dropped object does not have DraggableSource.");
            return;
        }

        MyModelC sourceModelC = viewA.AssociatedModel;
        
        // 2. データに基づいたドロップの成否を判断 (ビジネスロジック)
        bool isDropValid = IsValidDrop(sourceModelC, targetModelD); // 👈 このメソッド内でCとDのデータを比較
        
        if (isDropValid)
        {
            // 3. 成功ロジック: データ反映
            targetModelD.ReflectDataFrom(sourceModelC); 

            // 4. View Aに成功を通知
            viewA.NotifyDropResult(true); 
            // 例えば、この通知を受けたView Aは自身を非表示/破棄する
            
            Debug.Log($"ドロップ成功: {sourceModelC.DataName} のデータを {targetModelD.DataName} に反映しました。");
        }
        else
        {
            // 5. View Aに失敗を通知
            viewA.NotifyDropResult(false);
            // 例えば、この通知を受けたView Aは元の位置に戻る
            
            Debug.Log("ドロップ失敗: データ制約により反映できません。");
        }
    }

    // Model CとDのデータのみに基づいて成否を判断する純粋なロジック
    private bool IsValidDrop(MyModelC source, MyModelD target)
    {
        // 例: CのデータがDの特定の条件を満たしているか
        return source.DataType == target.RequiredType && source.Value > target.MinValue;
    }
}
*/