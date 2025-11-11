using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionGameEventMediator : MonoBehaviour
{
    public InputActionReference pauseAction;
    public InputActionReference toggleInputAction;


    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.started += context => EventManager.Raise(new PauseInputEvent { triggered = true });
        }

        if (toggleInputAction != null)
        {
            toggleInputAction.action.Enable();
            toggleInputAction.action.started += context => EventManager.Raise(new ToggleInventory { enable = true });
        }
    }

    private void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= _ => EventManager.Raise(new PauseInputEvent { triggered = true });
        }

        if (toggleInputAction != null)
        {
            toggleInputAction.action.Enable();
            toggleInputAction.action.started -= _ => EventManager.Raise(new ToggleInventory { enable = true });
        }
    }
}
