using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventListenerBase : MonoBehaviour
{
    protected abstract void Register();
    protected abstract void Unregister();

    private void OnEnable() => Register();
    private void OnDisable() => Unregister();
}