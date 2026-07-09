using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class EventListener<T, TU> : EventListenerBase where TU : UnityEvent<T>, new()
{
    [SerializeField] private TU response = new();

    protected override void Register() => EventManager.Register<T>(OnEvent);
    protected override void Unregister() => EventManager.Unregister<T>(OnEvent);

    private void OnEvent(T payload) => response?.Invoke(payload);
}

