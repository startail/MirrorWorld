using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIFocusGroup : MonoBehaviour
    {
        [Header("Focus Settings")]
        [SerializeField] private Selectable defaultFocusTarget;

        private List<Selectable> selectables = new List<Selectable>();
        private bool _isActive = false;

        public bool IsActive => _isActive;

        private void Awake()
        {
            selectables.AddRange(GetComponentsInChildren<Selectable>(true));
        }

        public void Activate()
        {
            _isActive = true;

            foreach (var selectable in selectables)
            {
                if (selectable != null)
                    selectable.interactable = true;
            }

            var token = this.GetCancellationTokenOnDestroy();
            if (defaultFocusTarget != null)
                SetFocusAsync(defaultFocusTarget, token).Forget();
            else if (selectables.Count > 0)
                SetFocusAsync(selectables[0], token).Forget();
        }

        public void Deactivate()
        {
            _isActive = false;

            foreach (var selectable in selectables)
            {
                if (selectable != null)
                    selectable.interactable = false;
            }
        }

        public void SetDefaultFocusTarget(Selectable target)
        {
            defaultFocusTarget = target;
        }

        // 子要素のSelectableリストを更新（動的にボタンを追加した場合に呼ぶ）
        public void RefreshSelectables()
        {
            selectables.Clear();
            selectables.AddRange(GetComponentsInChildren<Selectable>(true));
        }

        private async UniTaskVoid SetFocusAsync(Selectable target, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            }
            catch (System.OperationCanceledException)
            {
                return;
            }

            if (target != null && target.gameObject.activeInHierarchy && target.interactable)
                target.Select();
        }
    }
}
