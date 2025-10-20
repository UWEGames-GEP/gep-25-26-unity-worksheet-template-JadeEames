using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct EventBinding
{
    public UnityEvent subscribe;
    public UnityEvent unsubscribe;
    public UnityEvent handle_event;
}

[CreateAssetMenu(fileName = "EventBindingsConfig", menuName = "Scriptable Objects/EventBindingsConfig")]
public class EventBindingsConfig : ScriptableObject
{
    public List<EventBinding> config;
    public PayloadGameEvent<int> test;
}