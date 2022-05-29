using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Translate
{
    public string id;
    [HideInInspector]
    public List<string> sentences = new List<string>();

    public Translate(string id, List<string> translations)
    {
        this.id = id;
        this.sentences = translations;
    }
}
