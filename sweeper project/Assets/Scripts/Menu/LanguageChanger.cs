using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

public class LanguageChanger : MonoBehaviour
{
    TMP_Dropdown myDrownDown;

    void Start()
    {
        // TODO if no language has been set in playerprefs then check system language or switch to english
        Debug.Log(CultureInfo.InstalledUICulture.TwoLetterISOLanguageName);

        myDrownDown = GetComponent<TMP_Dropdown>();
    }

    public void UpdateLanguage()
    {
        int option = myDrownDown.value;
        Settings.Instance.SetLanguage((Language)option);
    }
}
