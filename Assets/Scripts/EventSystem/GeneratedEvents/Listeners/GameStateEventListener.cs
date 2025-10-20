
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameStateUnityEvent : UnityEvent<GameStateEvent> { }

public class GameStateEventListener : EventListener<GameStateEvent, GameStateUnityEvent> { }
