using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "localisation", menuName = "Scriptable/")]
public class LanguageDatabase : ScriptableObject
{
    public List<Translate> translations;


    /// <summary>
    /// Getting the sentence.
    /// </summary>
    /// <param name="id">ID of the sentence</param>
    /// <param name="language">Language needed</param>
    /// <returns></returns>
    public string GetSentence(string id, Language language)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].id == id)
            {
                Translate translation = translations[i];
                return translation.sentences[(int)language];
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Getting the sentences
    /// </summary>
    /// <param name="id">ID of sentence</param>
    /// <returns></returns>
    public List<string> GetSentences(string id)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].id == id)
            {
                return translations[i].sentences;
            }
        }

        return null;
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

    public void DeleteEntry(string id)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].id == id)
            {
                translations.Remove(GetTranslateObject(id));
            }
        }
    }

    public Translate GetTranslateObject(string id)
    {
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].id == id)
            {
                return translations[i];
            }
        }

        return null;
    }
}
