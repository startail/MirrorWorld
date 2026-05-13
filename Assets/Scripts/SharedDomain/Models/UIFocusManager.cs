using R3;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class UIFocusManager
    {
        private readonly Stack<string> _focusStack = new Stack<string>();
        private readonly ReactiveProperty<string> _currentFocusGroup = new ReactiveProperty<string>(null);

        public ReadOnlyReactiveProperty<string> CurrentFocusGroup => _currentFocusGroup;
        public bool HasActiveUI => _focusStack.Count > 0;

        public void Push(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                UnityEngine.Debug.LogWarning("UIFocusManager.Push: groupId is null or empty");
                return;
            }

            _focusStack.Push(groupId);
            _currentFocusGroup.Value = groupId;
        }

        public string Pop()
        {
            if (_focusStack.Count > 0)
            {
                string poppedGroup = _focusStack.Pop();
                _currentFocusGroup.Value = _focusStack.Count > 0 ? _focusStack.Peek() : null;
                return poppedGroup;
            }

            return null;
        }

        public void Clear()
        {
            _focusStack.Clear();
            _currentFocusGroup.Value = null;
        }

        public bool IsActive(string groupId)
        {
            return _currentFocusGroup.Value == groupId;
        }

        public int StackDepth => _focusStack.Count;
    }
}
