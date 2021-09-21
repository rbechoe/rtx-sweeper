using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    // gameplay related
    COUNT_BOMBS     = 0,    
    SHOW_BOMBS      = 1,
    END_GAME        = 2,
    PICK_TILE       = 3,
    START_GAME      = 4,
    PREPARE_GAME    = 5,
    PLANT_FLAG      = 6,
    REMOVE_FLAG     = 7,
    RESET_GAME      = 8,
    FIRST_CLICK     = 10,
    ADD_EMPTY       = 11,
    START_POS       = 12,
    WIN_GAME        = 13,
    BOMB_UPDATE     = 14,
    // input related
    INPUT_LEFT      = 15,
    INPUT_RIGHT     = 16,
    INPUT_UP        = 17,
    INPUT_DOWN      = 18,
    INPUT_FORWARD   = 19,
    INPUT_BACK      = 20,
}

// event system that takes multiple arguments through an array
public static class EventSystem<Parameters>
{
    private static Dictionary<EventType, System.Action<Parameters>> eventDictionary = new Dictionary<EventType, System.Action<Parameters>>();

    public static void AddListener(EventType type, System.Action<Parameters> function)
    {
        if (!eventDictionary.ContainsKey(type))
        {
            eventDictionary.Add(type, null);
        }

        eventDictionary[type] += (function);
    }

    public static void RemoveListener(EventType type, System.Action<Parameters> function)
    {
        if (eventDictionary.ContainsKey(type))
        {
            eventDictionary[type] -= (function);
        }
    }

    // execute event for all those listening
    public static void InvokeEvent(EventType type, Parameters param)
    {
        eventDictionary[type]?.Invoke(param);
    }
}

// all possible parameters
public class Parameters
{
    public List<int> integers = new List<int>();
    public List<float> floats = new List<float>();
    public List<bool> booleans = new List<bool>();
    public List<Vector3> vector3s = new List<Vector3>();
    public List<GameObject> gameObjects = new List<GameObject>();
}