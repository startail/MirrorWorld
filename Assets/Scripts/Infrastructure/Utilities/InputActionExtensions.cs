using System;
using UnityEngine.InputSystem;
using R3;

namespace Infrastructure.Utilities
{
    // R3は純粋C#動作なので、Unity関連の処理を直接Observableに変換する機能は入ってないため、その拡張を自作する必要がある
    public static class InputActionExtensions
    {
        public static Observable<InputAction.CallbackContext> OnPerformedAsObservable(this InputAction action)
        {
            return Observable.FromEvent<Action<InputAction.CallbackContext>, InputAction.CallbackContext>(
                h => (InputAction.CallbackContext context) => h(context),
                h => action.performed += h,
                h => action.performed -= h
            );
        }
    }
}
