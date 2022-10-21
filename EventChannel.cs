using UnityEngine;
using UnityEngine.Events;

public abstract class EventChannel<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T type) =>
        OnEventRaised?.Invoke(type);
}