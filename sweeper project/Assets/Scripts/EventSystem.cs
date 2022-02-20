using UnityEngine;
using System.Collections.Generic;

public enum EventType
{
    // gameplay related
    COUNT_BOMBS     = 0,    // passes nothing    
    SHOW_BOMBS      = 1,    // not used
    END_GAME        = 2,    // passes nothing
    PICK_TILE       = 3,    // passes nothing
    START_GAME      = 4,    // passes nothing
    PREPARE_GAME    = 5,    // passes nothing
    PLANT_FLAG      = 6,    // passes vector 3[]
    REMOVE_FLAG     = 7,    // passes gameobject
    RESET_GAME      = 8,    // passes nothing
    FIRST_CLICK     = 10,   // not used
    ADD_EMPTY       = 11,   // passes gameobject
    START_POS       = 12,   // passes vector 3
    WIN_GAME        = 13,   // passes nothing
    BOMB_UPDATE     = 14,   // passes int
    // input related
    INPUT_LEFT      = 15,   // passes nothing
    INPUT_RIGHT     = 16,   // passes nothing
    INPUT_UP        = 17,   // passes nothing
    INPUT_DOWN      = 18,   // passes nothing
    INPUT_FORWARD   = 19,   // passes nothing
    INPUT_BACK      = 20,   // passes nothing
    // new stuff
    RANDOM_GRID     = 21,   // passes nothing
    GAME_LOSE       = 22,   // passes nothing
    ADD_GOOD_TILE   = 23,   // passes gameobject
    UPDATE_TIME     = 24,   // passes float
    PLAY_CLICK      = 25,   // passes nothing
    PLAY_FLAG       = 26,   // passes nothing
    TILE_CLICK      = 27,   // passes nothing
}

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
