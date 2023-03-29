using UnityEngine;
using System.Collections.Generic;
using System;

public static class EventSystem
{
    public static Dictionary<EventType, Action> eventCollection = new Dictionary<EventType, Action>();
    public static Dictionary<EventType, Action<object>> eventCollectionParam = new Dictionary<EventType, Action<object>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void FillDictionary()
    {
        eventCollection.Clear();
        eventCollectionParam.Clear();

        foreach (EventType type in Enum.GetValues(typeof(EventType)))
        {
            if (!eventCollection.ContainsKey(type))
            {
                eventCollection.Add(type, Empty);
            }
            if (!eventCollectionParam.ContainsKey(type))
            {
                eventCollectionParam.Add(type, Empty);
            }
        }
    }

    private static void Empty() { }

    private static void Empty(object value) { }
}
