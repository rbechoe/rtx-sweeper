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
        EventSystem.eventCollection[EventType.WIN_GAME] += EnableText;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += DisableText;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.WIN_GAME] -= EnableText;
        EventSystem.eventCollection[EventType.RANDOM_GRID] -= DisableText;
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
        EventSystem.eventCollection[EventType.RANDOM_GRID]();
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
