using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject victoryText;
    public TextMeshProUGUI bombText;
    public TextMeshProUGUI timeText;

    private int bombAmount;

    private void Start()
    {
        victoryText.SetActive(false);
        bombText.text = bombAmount.ToString();
    }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.RESET_GAME, ResetGame);
        EventSystem.AddListener(EventType.RANDOM_GRID, ResetGrid);
        EventSystem.AddListener(EventType.WIN_GAME, WinGame);    
        EventSystem<int>.AddListener(EventType.BOMB_UPDATE, SetBombsLeft);
        EventSystem<float>.AddListener(EventType.UPDATE_TIME, SetTimer);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, ResetGrid);
        EventSystem.RemoveListener(EventType.WIN_GAME, WinGame);
        EventSystem<int>.RemoveListener(EventType.BOMB_UPDATE, SetBombsLeft);
        EventSystem<float>.RemoveListener(EventType.UPDATE_TIME, SetTimer);
    }

    private void WinGame()
    {
        victoryText.SetActive(true);
    }

    private void ResetGame()
    {
        victoryText.SetActive(false);
        bombAmount = 0;
        timeText.text = "0";
        bombText.text = "0";
    }

    private void SetTimer(float time)
    {
        timeText.text = "" + Mathf.Round(time * 1000.0f) / 1000.0f;
    }

    private void SetBombsLeft(int amount)
    {
        bombAmount = amount;
        bombText.text = bombAmount.ToString();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGrid()
    {
        EventSystem.InvokeEvent(EventType.RESET_GAME);
    }
}