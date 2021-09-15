using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    COUNT_BOMBS     = 0,    
    SHOW_BOMBS      = 1,
    END_GAME        = 2,
    ENABLE_GRID     = 3,
    START_GAME      = 4,
    PREPARE_GAME    = 5,
    PLANT_FLAG      = 6,
    REMOVE_FLAG     = 7,
}

// event system that takes 0 arguments
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
        eventDictionary[type]?.Invoke();
    }
}

// event system that takes 1 argument
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
    public static void InvokeEvent(EventType type, T param)
    {
        eventDictionary[type]?.Invoke(param);
    }
}