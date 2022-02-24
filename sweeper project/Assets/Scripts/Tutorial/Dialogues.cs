using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogues : MonoBehaviour
{
    public List<string> lines;
    public List<int> waitTimes;

    public TextMeshProUGUI text;
    public GameObject dialogueBar;

    private float totalWaitTime;

    void Start()
    {
        DisableBar();
        StartCoroutine(DelayedMethods.FireMethod(EnableBar, 2));

        for (int i = 0; i < lines.Count; i++)
        {
            totalWaitTime += waitTimes[i];
            StartCoroutine(DelayedMethods<int>.FireMethod(Dialogue, i, totalWaitTime));
        }

        StartCoroutine(DelayedMethods.FireMethod(DisableBar, totalWaitTime + 4));
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
}
