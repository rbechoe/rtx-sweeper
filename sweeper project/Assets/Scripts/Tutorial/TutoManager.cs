using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutoManager : MonoBehaviour
{
    public GameObject victoryText;
    public GameObject[] uiObjects;
    public TextMeshProUGUI bombCount, timer;

    private void Start()
    {
        DisableUIElements();
        DisableText();
        StartCoroutine(DelayedMethods.FireMethod(RandomizeGrid, 10));
    }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.WIN_GAME, EnableText);
        EventSystem.AddListener(EventType.RANDOM_GRID, DisableText);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.WIN_GAME, EnableText);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, DisableText);
    }

    void EnableText()
    {
        victoryText.SetActive(true);
    }

    void DisableText()
    {
        victoryText.SetActive(false);
    }

    void DisableUIElements()
    {
        foreach (GameObject ui in uiObjects)
        {
            ui.SetActive(false);
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
