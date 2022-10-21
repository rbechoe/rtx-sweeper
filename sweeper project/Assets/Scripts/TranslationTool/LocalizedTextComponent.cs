using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedTextComponent : MonoBehaviour
{
    public static LanguageDatabase LANGUAGE_DATABASE
    {
        get => Resources.Load<LanguageDatabase>("Languages");

        set => LANGUAGE_DATABASE = value;
    }

    private Text textComponent;
    [SerializeField]
    private string TextIdentifier;

    private Language language;

    public void Awake()
    {
        textComponent = GetComponent<Text>();
        EventSystem.AddListener(EventType.UPDATE_LANGUAGE, RefreshLanguage);
    }

    public void OnDestroy()
    {
        EventSystem.RemoveListener(EventType.UPDATE_LANGUAGE, RefreshLanguage);
    }

    public void Start()
    {
        language = Settings.Instance.GetLanguage();

        if (TextIdentifier == string.Empty)
        {
            return;
        }

        if (LANGUAGE_DATABASE)
        {
            textComponent.text = LANGUAGE_DATABASE.GetSentence(TextIdentifier, language);
        }
    }

    public void RefreshLanguage()
    {
        language = Settings.Instance.GetLanguage();
        textComponent.text = LANGUAGE_DATABASE.GetSentence(TextIdentifier, language);
    }
}
