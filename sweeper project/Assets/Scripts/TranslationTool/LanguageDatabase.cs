using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (translations[i].id == ID)
            {
                Translate translate = translations[i];
                return translate.translation[(int)language];
            }
        }

        return "Not Localized Yet.";
    }

    public List<string> GetSentences(string ID)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].id == ID)
            {
                Translate translate = translations[i];
                return translate.translation;
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
            if (translations[i].id == ID)
            {
                translations.Remove(GetTranslateObject(ID));
            }
        }
    }

    public Translate GetTranslateObject(string ID)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].id == ID)
            {
                return translations[i];
            }
        }

        return null;
    }
}
