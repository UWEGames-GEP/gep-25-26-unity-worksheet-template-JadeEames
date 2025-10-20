
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PauseInputEventUnityEvent : UnityEvent<PauseInputEvent> { }

public class PauseInputEventEventListener : EventListener<PauseInputEvent, PauseInputEventUnityEvent> { }
