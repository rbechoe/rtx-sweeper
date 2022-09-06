using System;
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
    public string GetSentence(string ID, Language language)
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

    public List<string> GetSentences(string ID)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].ID == ID)
            {
                Translate translate = translations[i];
                return translate.sentence;
            }
        }

        return new List<string>();
    }

    public void AddEntry(string textInput)
    {
        List<string> temp = new List<string>();
        int count = Enum.GetNames(typeof(Language)).Length;

        for (int i = 0; i < count; i++)
        {
            temp.Add("");
        }

        Translate translate = new Translate(textInput, temp);
        translations.Add(translate);
    }

    public void DeleteEntry(string ID)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].ID == ID)
            {
                translations.Remove(GetTranslateObject(ID));
            }
        }
    }

    public Translate GetTranslateObject(string ID)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].ID == ID)
            {
                return translations[i];
            }
        }

        return null;
    }
}
