using UnityEngine;

public class AudioEffects : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip explosion, win, lose, tileClick, flagPlant, flagRemove;

    private void OnEnable()
    {
        // listen
        EventSystem.eventCollection[EventType.WIN_GAME] += GameWin;
        EventSystem.eventCollection[EventType.GAME_LOSE] += GameOver;
        EventSystem.eventCollection[EventType.PLAY_CLICK] += ClickSound;
        EventSystem.eventCollection[EventType.PLAY_CLICK_SFX] += UIClick;
        EventSystem.eventCollection[EventType.PLAY_FLAG] += FlagPlantSound;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] += RemoveFlagSound;
    }

    private void OnDisable()
    {
        // unlisten
        EventSystem.eventCollection[EventType.WIN_GAME] -= GameWin;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= GameOver;
        EventSystem.eventCollection[EventType.PLAY_CLICK] -= ClickSound;
        EventSystem.eventCollection[EventType.PLAY_CLICK_SFX] -= UIClick;
        EventSystem.eventCollection[EventType.PLAY_FLAG] -= FlagPlantSound;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] -= RemoveFlagSound;
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

    private void RemoveFlagSound(object value)
    {
        audioSource.PlayOneShot(flagRemove);
    }

    public void UIClick()
    {
        audioSource.PlayOneShot(tileClick);
    }
}
