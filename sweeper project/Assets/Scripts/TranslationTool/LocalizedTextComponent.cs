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

    private Language language;

    public void Awake()
    {
        EventSystem.AddListener(EventType.UPDATE_LANGUAGE, RefreshLanguage);
    }

    public void OnDestroy()
    {
        EventSystem.RemoveListener(EventType.UPDATE_LANGUAGE, RefreshLanguage);
    }

    public void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        language = Settings.Instance.GetLanguage();

        if (TextIdentifier == string.Empty)
            return;

        if (LANGUAGE_DATABASE)
        {
            textComponent.text = LANGUAGE_DATABASE.GetSentence(TextIdentifier, language);
        }
    }

    public void RefreshLanguage()
    {
        language = Settings.Instance.GetLanguage();
        textComponent.text = LANGUAGE_DATABASE.GetSentence(TextIdentifier, language);
        // TODO update font of language as well so that it wont go blank
    }
}
