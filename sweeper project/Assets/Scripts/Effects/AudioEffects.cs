using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffects : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip explosion, win, lose, tileClick, flagPlant, flagRemove;

    private void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.WIN_GAME, GameWin);
        EventSystem.AddListener(EventType.GAME_LOSE, GameOver);
        EventSystem.AddListener(EventType.PLAY_CLICK, ClickSound);
        EventSystem.AddListener(EventType.PLAY_FLAG, FlagPlantSound);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, RemoveFlagSound);
    }

    private void OnDisable()
    {
        // unlisten
        EventSystem.RemoveListener(EventType.WIN_GAME, GameWin);
        EventSystem.RemoveListener(EventType.GAME_LOSE, GameOver);
        EventSystem.RemoveListener(EventType.PLAY_CLICK, ClickSound);
        EventSystem.RemoveListener(EventType.PLAY_FLAG, FlagPlantSound);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlagSound);
    }

    private void GameOver()
    {
        audioSource.PlayOneShot(lose);
        audioSource.PlayOneShot(explosion);
    }

    private void GameWin()
    {
        audioSource.PlayOneShot(win);
    }

    private void ClickSound()
    {
        audioSource.PlayOneShot(tileClick);
    }

    private void FlagPlantSound()
    {
        audioSource.PlayOneShot(flagPlant);
    }

    private void RemoveFlagSound(GameObject flag)
    {
        audioSource.PlayOneShot(flagRemove);
    }

    public void UIClick()
    {
        audioSource.PlayOneShot(tileClick);
    }
}
