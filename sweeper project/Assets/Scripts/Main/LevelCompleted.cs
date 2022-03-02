using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelCompleted : MonoBehaviour
{
    public List<string> lines;
    public List<int> waitTimes;

    public TextMeshProUGUI text;
    public GameObject dialogueBar;

    public float endDelay;
    private float totalWaitTime;

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.WIN_GAME, FireDialogue);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.WIN_GAME, FireDialogue);
    }

    void FireDialogue()
    {
        StartCoroutine(DelayedMethods.FireMethod(EnableBar));

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
        text.text = lines[line];
    }
}
