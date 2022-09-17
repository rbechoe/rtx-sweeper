using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Translate
{
    public string id;
    public List<string> translation;

    public Translate(string id, List<string> translations)
    {
        this.id = id;
        this.translation = translations;
    }
}
