using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dialogues : MonoBehaviour
{
    public List<string> lines;
    public List<int> waitTimes;

    public Text text;
    public GameObject dialogueBar;

    public float startDelay, endDelay;
    private float totalWaitTime;

    private Language language;

    public static LanguageDatabase LANGUAGE_DATABASE
    {
        get => Resources.Load<LanguageDatabase>("Languages");

        set => LANGUAGE_DATABASE = value;
    }

    void Start()
    {
        language = Settings.Instance.GetLanguage();

        DisableBar();

        StartCoroutine(DelayedMethods.FireMethod(EnableBar, startDelay));

        totalWaitTime += startDelay - waitTimes[0];

        for (int i = 0; i < lines.Count; i++)
        {
            totalWaitTime += waitTimes[i];
            StartCoroutine(DelayedMethods<int>.FireMethod(Dialogue, i, totalWaitTime));
        }

        StartCoroutine(DelayedMethods.FireMethod(DisableBar, totalWaitTime + endDelay));
    }

    void EnableBar()
    {
        dialogueBar.SetActive(true);
    }

    void DisableBar()
    {
        dialogueBar.SetActive(false);
    }

    void Dialogue(int line)
    {
        text.text = LANGUAGE_DATABASE.GetSentence(lines[line], language);
    }
}
