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
        DisableUIElements();
        StartCoroutine(DelayedMethods.FireMethod(EnableBar, startDelay));
        StartCoroutine(DelayedMethods.FireMethod(RandomizeGrid, 10));
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

    // TODO detach all below this comment and place it in a tutorial manager
    void DisableUIElements()
    {
        foreach (GameObject ui in uiObjects)
        {
            ui.SetActive(false);
        }
    }

    void EnableUIElements()
    {
        foreach (GameObject ui in uiObjects)
        {
            ui.SetActive(true);
        }
    }

    public void RandomizeGrid()
    {
        EventSystem.InvokeEvent(EventType.RANDOM_GRID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
