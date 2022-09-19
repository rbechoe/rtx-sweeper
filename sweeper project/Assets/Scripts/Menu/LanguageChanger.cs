using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using System;

public class LanguageChanger : MonoBehaviour
{
    TMP_Dropdown myDrownDown;

    void Start()
    {
        myDrownDown = GetComponent<TMP_Dropdown>();

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
