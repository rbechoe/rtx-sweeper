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
        EventSystem<Parameters>.AddListener(EventType.WIN_GAME, GameWin);
        EventSystem<Parameters>.AddListener(EventType.GAME_LOSE, GameOver);
        EventSystem<Parameters>.AddListener(EventType.PLAY_CLICK, ClickSound);
        EventSystem<Parameters>.AddListener(EventType.PLAY_FLAG, FlagPlantSound);
        EventSystem<Parameters>.AddListener(EventType.REMOVE_FLAG, RemoveFlagSound);
    }

    private void OnDisable()
    {
        // unlisten
        EventSystem<Parameters>.RemoveListener(EventType.WIN_GAME, GameWin);
        EventSystem<Parameters>.RemoveListener(EventType.GAME_LOSE, GameOver);
        EventSystem<Parameters>.RemoveListener(EventType.PLAY_CLICK, ClickSound);
        EventSystem<Parameters>.RemoveListener(EventType.PLAY_FLAG, FlagPlantSound);
        EventSystem<Parameters>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlagSound);
    }

    private void GameOver(Parameters parameters)
    {
        audioSource.PlayOneShot(lose);
        audioSource.PlayOneShot(explosion);
    }

    private void GameWin(Parameters parameters)
    {
        audioSource.PlayOneShot(win);
    }

    private void ClickSound(Parameters parameters)
    {
        audioSource.PlayOneShot(tileClick);
    }

    private void FlagPlantSound(Parameters parameters)
    {
        audioSource.PlayOneShot(flagPlant);
    }

    private void RemoveFlagSound(Parameters parameters)
    {
        audioSource.PlayOneShot(flagRemove);
    }
}
