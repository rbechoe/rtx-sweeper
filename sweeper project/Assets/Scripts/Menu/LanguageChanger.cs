using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Dropdown))]
public class LanguageChanger : MonoBehaviour
{
    Dropdown myDrownDown;

    void Start()
    {
        myDrownDown = GetComponent<Dropdown>();

        if (PlayerPrefs.HasKey("Language"))
        {
            myDrownDown.value = PlayerPrefs.GetInt("Language");
        }
        else
        {
            string localLang = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            myDrownDown.value = (int)Enum.Parse(typeof(Language), localLang);
        }
    }

    public void UpdateLanguage()
    {
        int option = myDrownDown.value;
        Settings.Instance.SetLanguage((Language)option);
    }
}
