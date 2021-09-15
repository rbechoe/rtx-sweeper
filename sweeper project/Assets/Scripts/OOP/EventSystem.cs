using System.Collections;
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
}

public delegate void EventCallback(EventType evt, object value);

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
        Debug.Log("Invoked: " + type.ToString());
        eventDictionary[type]?.Invoke();
    }
}
