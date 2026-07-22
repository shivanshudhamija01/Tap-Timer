using System;
public static class EventBus<T> where T : IEvent
{
    private static event Action<T> OnEvent;

    public static void Subscribe(Action<T> listener) => OnEvent += listener;
    public static void Unsubscribe(Action<T> listener) => OnEvent -= listener;
    public static void Raise(T eventData) => OnEvent?.Invoke(eventData);

    /// <summary>Drops all subscribers for this event type. Useful as a safety net on scene load if stale references from Editor domain reloads are ever a concern.</summary>
    public static void Clear() => OnEvent = null;
}