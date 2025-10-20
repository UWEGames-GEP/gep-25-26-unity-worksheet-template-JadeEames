using System;
using UnityEngine;
using System.Collections.Generic;

public static class EventManager
{
    /// <summary>
    /// Events is a dictionary mapping event payload type to registered method pointers.
    /// </summary>
    private static Dictionary<Type, Delegate> events = new();

    /// <summary>
    /// <c>Register</c> adds a method pointer to an event types registered methods. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="callback"></param>
    public static void Register<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var registeredMethods))
        {
            events[typeof(T)] = (Action<T>)registeredMethods + callback;
        }
        else
        {
            events[typeof(T)] = callback;
        }
    }

    /// <summary>
    /// <c>Unregister</c> removes a method pointer from an event types registered methods. 
    /// </summary>
    /// <typeparam name="T">Placeholder for event payload type</typeparam>
    /// <param name="callback">Method pointer associated with event type</param>
    public static void Unregister<T>(Action<T> callback)
    {
        if (events.TryGetValue(typeof(T), out var registeredMethods))
        {
            var current = (Action<T>)registeredMethods - callback;

            if (current == null) { events.Remove(typeof(T)); }
            else { events[typeof(T)] = current; }
        }
    }
    /// <summary>
    /// <c>Raise</c> triggers provided event and its registered methods, passing event payload through to those methods. 
    /// </summary>
    /// <typeparam name="T">Event payload type</typeparam>
    /// <param name="eventPayload">Event payload data</param>
    public static void Raise<T>(T eventPayload)
    {
        if (events.TryGetValue(typeof(T), out var registeredMethods))
        {
            ((Action<T>)registeredMethods)?.Invoke(eventPayload);
        }
    }
}
