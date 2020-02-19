using System;
using System.Collections.Generic;
using System.Linq;

public static class EventEmitter
{
    private static Dictionary<GameEvent, List<Action<IEvent>>> listeners = new Dictionary<GameEvent, List<Action<IEvent>>>();

    public static void Reset()
    {
        listeners.Clear();
    }

    public static void Add(GameEvent eventName, Action<IEvent> action)
    {
        if (listeners.ContainsKey(eventName))
        {
            listeners[eventName].Add(action);
        }
        else
        {
            listeners[eventName] = new List<Action<IEvent>>
            {
                action
            };
        }
    }

    public static void Remove(GameEvent eventName, Action<IEvent> action)
    {
        if (listeners.ContainsKey(eventName))
        {
            listeners[eventName].Remove(action);
            if (!listeners[eventName].Any())
            {
                listeners.Remove(eventName);
            }
        }
    }

    public static void Emit(GameEvent eventName, IEvent eventParams = null)
    {
        if (listeners.ContainsKey(eventName))
        {
            foreach (var action in listeners[eventName])
            {
                action?.Invoke(eventParams);
            }
        }
    }
}