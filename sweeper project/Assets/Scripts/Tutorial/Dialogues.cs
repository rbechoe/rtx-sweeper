using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogues : MonoBehaviour
{
    public List<string> lines;
    public List<int> waitTimes;

    public TextMeshProUGUI text;
    public GameObject dialogueBar;
    public GameObject[] uiObjects;

    public float startDelay, endDelay;
    private float totalWaitTime;

    void Start()
    {
        DisableBar();

        StartCoroutine(DelayedMethods.FireMethod(EnableBar, startDelay));

        totalWaitTime += startDelay - waitTimes[0];

        for (int i = 0; i < lines.Count; i++)
        {
            totalWaitTime += waitTimes[i];
            StartCoroutine(DelayedMethods<int>.FireMethod(Dialogue, i, totalWaitTime));
        }

        StartCoroutine(DelayedMethods.FireMethod(DisableBar, totalWaitTime + endDelay));
        StartCoroutine(DelayedMethods.FireMethod(EnableUIElements, totalWaitTime));
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
        text.text = lines[line];
    }

    void EnableUIElements()
    {
        foreach (GameObject ui in uiObjects)
        {
            ui.SetActive(true);
        }
    }
}
