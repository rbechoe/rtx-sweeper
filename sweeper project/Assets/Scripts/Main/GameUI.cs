using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject victoryText, loserText;
    public TextMeshProUGUI bombText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI victoryTimeText;
    public TextMeshProUGUI victorySkillText;
    private CameraManager cameraManager;

    public Slider BGMSlider, SFXSlider, mainSFXSlider;

    private int bombAmount;
    private float currentTime;

    private void Start()
    {
        victoryText.SetActive(false);
        loserText.SetActive(false);
        bombText.text = bombAmount.ToString();

        Settings settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();
        BGMSlider.value = settings.GetBGMVolume();
        SFXSlider.value = settings.GetSFXVolume();
        mainSFXSlider.value = settings.GetMainSFXVolume();
        cameraManager = gameObject.GetComponent<CameraManager>();
    }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.RESET_GAME, ResetGame);
        EventSystem.AddListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.AddListener(EventType.RANDOM_GRID, ResetGrid);
        EventSystem.AddListener(EventType.WIN_GAME, WinGame);    
        EventSystem<int>.AddListener(EventType.BOMB_UPDATE, SetBombsLeft);
        EventSystem<float>.AddListener(EventType.UPDATE_TIME, SetTimer);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, LoseGame);
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
        victoryTimeText.text = "<mspace=mspace=21>" + Math.Round(currentTime, 3).ToString("N3");
        victoryText.SetActive(true);
    }

    private void LoseGame()
    {
        loserText.SetActive(true);
    }

    private void ResetGame()
    {
        victoryText.SetActive(false);
        loserText.SetActive(false);
        bombAmount = 0;
        timeText.text = "0";
        bombText.text = "0";
        victorySkillText.text = "0%";
        victoryTimeText.text = "0";
    }

    private void SetTimer(float time)
    {
        currentTime = time;
        timeText.text = "<mspace=mspace=21>" + (int)currentTime;
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
        cameraManager.ResetToStart();
    }

    public void SetEfficiency(float value)
    {
        victorySkillText.text = "<mspace=mspace=21>" + Math.Round(value, 3) + "%";
    }
}