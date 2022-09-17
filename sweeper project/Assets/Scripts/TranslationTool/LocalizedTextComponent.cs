using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextComponent : MonoBehaviour
{
    public static LanguageDatabase LANGUAGE_DATABASE
    {
        get => Resources.Load<LanguageDatabase>("Languages");

        set => LANGUAGE_DATABASE = value;
    }

    private TextMeshProUGUI textComponent;
    [SerializeField]
    private string TextIdentifier;

    public static Language LANGUAGE
    {
        get;
        set;
    }

    public void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (TextIdentifier == string.Empty)
            return;

        if (LANGUAGE_DATABASE)
        {
            // todo language manager
            //Debug.Log(CultureInfo.InstalledUICulture.TwoLetterISOLanguageName);
            textComponent.text = LANGUAGE_DATABASE.GetSentence(TextIdentifier, LANGUAGE);
        }
    }

    public void RefreshLanguage()
    {
        textComponent.text = LANGUAGE_DATABASE.GetSentence(TextIdentifier, LANGUAGE);
    }
}
