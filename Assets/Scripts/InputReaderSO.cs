using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReaderSO", menuName = "Scriptable Objects/InputReaderSO")]
public class InputReaderSO : ScriptableObject, MyInputActions.IGameplayActions
{
    public event Action OnJumpStarted, OnJumpPerformed, OnJumpCanceled;
    public event Action<Vector2> OnMoveEvent;
    //Entidad que se encarga de leer los inputs y lanzar eventos
    
    private MyInputActions _InputActionMap;

    private void OnEnable()
    {
        _InputActionMap = new MyInputActions();
        _InputActionMap.Gameplay.Enable();  
        _InputActionMap.UI.Disable();
        _InputActionMap.Gameplay.AddCallbacks(this);
    }

    private void OnDisable()
    {
        _InputActionMap.Gameplay.Disable();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) OnJumpStarted?.Invoke();
        if (context.canceled) OnJumpCanceled?.Invoke();
        if (context.performed) OnJumpPerformed?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
}
