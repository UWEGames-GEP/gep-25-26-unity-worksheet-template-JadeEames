using UnityEngine;
using System;

/// <summary>
/// Class <c>GameEvent</c> is an abstract class declared for polymorphic purposes. May be expanded later to include shared functionality.
/// </summary>
public abstract class GameEvent : ScriptableObject { }

/// <summary>
/// Class <c>PayloadGameEvent</c> defines a concrete GameEvent containing a payload of type T.
/// </summary>
/// <typeparam name="T">Generic parameter T is a placeholder & will be replaced with a concrete type at compilation. <see></see>>https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics</typeparam>
[CreateAssetMenu(fileName = "PayloadGameEvent", menuName = "Scriptable Objects/PayloadGameEvent")]
public class PayloadGameEvent<T> : GameEvent
{
    /// <summary>
    /// Invocation list of method pointers.
    /// </summary>
    private Action<T> _listeners;

    /// <summary>
    /// Triggers the event and calls all registered listeners with the payload as arg. 
    /// </summary>
    /// <param name="payload"></param>
    public void Raise(T payload)
    {
        // Invokes event if _listeners is not null using null conditional operator. 
        _listeners?.Invoke(payload);
    }

    /// <summary>
    /// Function <c>Register</c> adds a method pointer to the <c>Action T</c> invocation list. 
    /// </summary>
    /// <param name="listener">Method pointer to add to the invocation list.</param>
    public void Register(Action<T> listener) => _listeners += listener;

    /// <summary>
    /// Function <c>Register</c> removes a method pointer from the <c>Action T</c> invocation list. 
    /// </summary>
    /// <param name="listener">Method pointer to remove from the invocation list.</param>
    public void Unregister(Action<T> listener) => _listeners -= listener;
    // => token, in this use case, is used for an expression body definition - other use is as the lambda operator. 
}
