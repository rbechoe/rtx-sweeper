using UnityEngine;
using System.Collections.Generic;

// event system without arguments
public static class EventSystem
{
    private static Dictionary<EventType, System.Action> eventDictionary = new Dictionary<EventType, System.Action>();

    public static void AddListener(EventType type, System.Action function)
    {
        if (!eventDictionary.ContainsKey(type))
        {
            eventDictionary.Add(type, null);
        }

        eventDictionary[type] += (function);
    }

    public static void RemoveListener(EventType type, System.Action function)
    {
        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type] -= (function);
        }
    }

    // execute event for all those listening
    public static void InvokeEvent(EventType type)
    {
        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type]?.Invoke();
        }
    }
}

// event system that takes arguments
public static class EventSystem<T>
{
    private static Dictionary<EventType, System.Action<T>> eventDictionary = new Dictionary<EventType, System.Action<T>>();

    public static void AddListener(EventType type, System.Action<T> function)
    {
        if (!eventDictionary.ContainsKey(type))
        {
            eventDictionary.Add(type, null);
        }

        eventDictionary[type] += (function);
    }

    public static void RemoveListener(EventType type, System.Action<T> function)
    {
        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type] -= (function);
        }
    }

    // execute event for all those listening
    public static void InvokeEvent(EventType type, T arg)
    {
        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type]?.Invoke(arg);
        }
    }
}
