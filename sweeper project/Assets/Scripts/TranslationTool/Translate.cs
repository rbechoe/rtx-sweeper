using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Translate : MonoBehaviour
{
    public string ID;
    [HideInInspector]
    public List<string> sentence;

    public Translate(string id, List<string> translations)
    {
        ID = id;
        sentence = translations;
    }
}
