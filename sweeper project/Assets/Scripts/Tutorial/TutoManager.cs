using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoManager : MonoBehaviour
{
    public GameObject victoryText;
    public Text bombCount, timer;

    private DataSerializer dataSerializer;
    public GameUI uiManager;

    private void Start()
    {
        dataSerializer = gameObject.GetComponent<DataSerializer>();

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
        AccountData userData = dataSerializer.GetUserData();
        uiManager.SetEfficiency(100);
        userData.tutorialVictories += 1;
        dataSerializer.UpdateAccountData(userData);

        victoryText.SetActive(true);
    }

    void DisableText()
    {
        victoryText.SetActive(false);
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
