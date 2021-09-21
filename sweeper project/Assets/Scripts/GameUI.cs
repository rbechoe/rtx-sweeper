using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : Base
{
    public GameObject victoryText;
    public TextMeshProUGUI bombText;
    public TextMeshProUGUI timeText;

    [SerializeField]
    private GameManager gameManager;

    private int bombAmount;

    protected override void Start()
    {
        victoryText.SetActive(false);
    }

    protected override void Update()
    {
        bombText.text = bombAmount.ToString();
        timeText.text = Mathf.FloorToInt(gameManager.timer).ToString();
    }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.RESET_GAME, ResetGame);    
        EventSystem<Parameters>.AddListener(EventType.WIN_GAME, WinGame);    
        EventSystem<Parameters>.AddListener(EventType.BOMB_UPDATE, SetBombsLeft);    
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Parameters>.RemoveListener(EventType.WIN_GAME, WinGame);
        EventSystem<Parameters>.RemoveListener(EventType.BOMB_UPDATE, SetBombsLeft);
    }

    private void WinGame(object value)
    {
        victoryText.SetActive(true);
    }

    private void ResetGame(object value)
    {
        victoryText.SetActive(false);
        bombAmount = 0;
    }

    private void SetBombsLeft(Parameters param)
    {
        bombAmount = param.integers[0];
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGame()
    {
        EventSystem<Parameters>.InvokeEvent(EventType.RESET_GAME, new Parameters());
    }
}
