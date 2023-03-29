using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject victoryText, loserText;
    public Text bombText;
    public Text timeText;
    public Text victoryTimeText;
    public Text victorySkillText;
    private CameraManager cameraManager;

    public Slider BGMSlider, SFXSlider, mainSFXSlider;

    private int bombAmount;
    private float currentTime;

    private void Start()
    {
        victoryText.SetActive(false);
        loserText.SetActive(false);
        bombText.text = bombAmount.ToString();

        BGMSlider.value = Settings.Instance.GetBGMVolume();
        SFXSlider.value = Settings.Instance.GetSFXVolume();
        mainSFXSlider.value = Settings.Instance.GetMainSFXVolume();

        cameraManager = gameObject.GetComponent<CameraManager>();
    }

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.RESET_GAME] += ResetGame;
        EventSystem.eventCollection[EventType.GAME_LOSE] += LoseGame;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += ResetGrid;
        EventSystem.eventCollection[EventType.WIN_GAME] += WinGame;    
        EventSystem.eventCollectionParam[EventType.BOMB_UPDATE] += SetBombsLeft;
        EventSystem.eventCollectionParam[EventType.UPDATE_TIME] += SetTimer;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.RESET_GAME] -= ResetGame;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= LoseGame;
        EventSystem.eventCollection[EventType.RANDOM_GRID] -= ResetGrid;
        EventSystem.eventCollection[EventType.WIN_GAME] -= WinGame;
        EventSystem.eventCollectionParam[EventType.BOMB_UPDATE] -= SetBombsLeft;
        EventSystem.eventCollectionParam[EventType.UPDATE_TIME] -= SetTimer;
    }

    public void UpdateBGMVolume()
    {
        EventSystem.eventCollectionParam[EventType.UPDATE_BGM](BGMSlider.value);
    }

    public void UpdateSFXVolume()
    {
        EventSystem.eventCollectionParam[EventType.UPDATE_SFX](SFXSlider.value);
    }

    public void UpdateMainSFXVolume()
    {
        EventSystem.eventCollectionParam[EventType.UPDATE_SFX_MAIN](mainSFXSlider.value);
    }

    private void WinGame()
    {
        victoryTimeText.text = "" + Math.Round(currentTime, 3).ToString("N3");
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

    private void SetTimer(object value)
    {
        currentTime = (float)value;
        timeText.text = "" + (int)currentTime;
    }

    private void SetBombsLeft(object value)
    {
        bombAmount = (int)value;
        bombText.text = bombAmount.ToString();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGrid()
    {
        EventSystem.eventCollection[EventType.RESET_GAME]();
    }

    public void EnableRTX()
    {
        EventSystem.eventCollection[EventType.ENABLE_RTX]();
    }

    public void DisableRTX()
    {
        EventSystem.eventCollection[EventType.DISABLE_RTX]();
    }

    public void ReloadLevel()
    {
        cameraManager.ResetToStart();
    }

    public void SlowReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetEfficiency(float value)
    {
        victorySkillText.text = "" + Math.Round(value, 3) + "%";
    }
}