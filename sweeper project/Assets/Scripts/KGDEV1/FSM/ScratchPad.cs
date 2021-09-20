using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchPad
{
    public Dictionary<string, object> data = new Dictionary<string, object>();

    public T Get<T>(string key)
    {
        if ( data.ContainsKey( key ) )
        {
            return (T)data[key];
        }
        return default(T);
    }

    public void Set<T>( string key, T value )
    {
        data[key] = value;
    }
}
