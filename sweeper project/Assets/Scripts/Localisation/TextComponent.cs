using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextComponent : MonoBehaviour
{
    public static LanguageDatabase LANGUAGE_DATABASE
    {
        get => Resources.Load<LanguageDatabase>("Localisation/LanguageDatabase");

        set => LANGUAGE_DATABASE = value;
    }

    private TextMeshProUGUI textComponent;
    [SerializeField]
    private string textIdentifier;

    public static Language LANGUAGE
    {
        get;
        set;
    }

    public void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (textIdentifier == string.Empty)
            return;

        if (LANGUAGE_DATABASE)
        {
            textComponent.text = LANGUAGE_DATABASE.GetSentence(this.textIdentifier, LANGUAGE);
        }
    }

    public void RefreshLanguage()
    {
        textComponent.text = LANGUAGE_DATABASE.GetSentence(this.textIdentifier, LANGUAGE);
    }
}
