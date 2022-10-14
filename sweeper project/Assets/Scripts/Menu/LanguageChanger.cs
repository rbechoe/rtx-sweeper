using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;

public class LanguageChanger : MonoBehaviour
{
    int curLanguage;
    public LanguagePiece[] pieces;

    void Start()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            curLanguage = PlayerPrefs.GetInt("Language");
        }
        else
        {
            string localLang = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            curLanguage = (int)Enum.Parse(typeof(Language), localLang);
        }

        UpdateLanguage(curLanguage);
    }

    public void UpdateLanguage(int option)
    {
        Settings.Instance.SetLanguage((Language)option);
        curLanguage = option;

        for (int i = 0; i < pieces.Length; i++)
        {
            if (i == curLanguage)
            {
                pieces[i].Select();
            }
            else
            {
                pieces[i].Deselect();
            }
        }
    }
}
