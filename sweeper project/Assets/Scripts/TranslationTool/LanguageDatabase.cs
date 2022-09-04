using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Localisation", menuName = "Scriptable/")]
public class LanguageDatabase : ScriptableObject
{
    public List<Translate> translations;

    /// <summary>
    /// Getting the sentence
    /// </summary>
    /// <param name="ID">ID of the sentence</param>
    /// <param name="language">Language needed</param>
    /// <returns>localized string</returns>
    public string GetSentence(string ID, int language)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].ID == ID)
            {
                Translate translate = translations[i];
                return translate.sentence[(int)language];
            }
        }

        return string.Empty;
    }
}
