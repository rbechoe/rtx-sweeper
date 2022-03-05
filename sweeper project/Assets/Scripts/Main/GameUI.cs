using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject victoryText;
    public TextMeshProUGUI bombText;
    public TextMeshProUGUI timeText;

    public Slider BGMSlider, SFXSlider, mainSFXSlider;

    private int bombAmount;

    private void Start()
    {
        victoryText.SetActive(false);
        bombText.text = bombAmount.ToString();

        Settings settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();
        BGMSlider.value = settings.GetBGMVolume();
        SFXSlider.value = settings.GetSFXVolume();
        mainSFXSlider.value = settings.GetMainSFXVolume();
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

    public void UpdateBGMVolume()
    {
        EventSystem<float>.InvokeEvent(EventType.UPDATE_BGM, BGMSlider.value);
    }

    public void UpdateSFXVolume()
    {
        EventSystem<float>.InvokeEvent(EventType.UPDATE_SFX, SFXSlider.value);
    }

    public void UpdateMainSFXVolume()
    {
        EventSystem<float>.InvokeEvent(EventType.UPDATE_SFX_MAIN, mainSFXSlider.value);
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
        timeText.text = "<mspace=mspace=21>" + Math.Round(time, 3).ToString("N3");
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

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}