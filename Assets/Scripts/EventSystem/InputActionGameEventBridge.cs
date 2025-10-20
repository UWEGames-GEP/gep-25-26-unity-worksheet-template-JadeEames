using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionGameEventMediator : MonoBehaviour
{
    public InputActionReference pauseAction;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.started += context => EventManager.Raise(new PauseInputEvent { triggered = true });
        }
    }

    private void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= _ => EventManager.Raise(new PauseInputEvent { triggered = true });
        }
    }
}
