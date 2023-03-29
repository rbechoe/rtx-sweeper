using UnityEngine;

public class AudioUpdater : MonoBehaviour
{
    public AudioSource[] bgmSources, sfxSources, mainSfxSources;

    private Settings settings;

    private void Start()
    {
        settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();

        ApplyBGMSettings(settings.GetBGMVolume());
        ApplySFXSettings(settings.GetSFXVolume());
        ApplyMainSFXSettings(settings.GetMainSFXVolume());
    }

    private void OnEnable()
    {
        EventSystem.eventCollectionParam[EventType.UPDATE_BGM] += ApplyBGMSettings;
        EventSystem.eventCollectionParam[EventType.UPDATE_SFX] += ApplySFXSettings;
        EventSystem.eventCollectionParam[EventType.UPDATE_SFX_MAIN] += ApplyMainSFXSettings;
    }

    private void OnDisable()
    {
        EventSystem.eventCollectionParam[EventType.UPDATE_BGM] -= ApplyBGMSettings;
        EventSystem.eventCollectionParam[EventType.UPDATE_SFX] -= ApplySFXSettings;
        EventSystem.eventCollectionParam[EventType.UPDATE_SFX_MAIN] -= ApplyMainSFXSettings;
    }

    private void ApplyBGMSettings(object value)
    {
        foreach(AudioSource bgm in bgmSources)
        {
            bgm.volume = (float)value;
        }
    }

    private void ApplySFXSettings(object value)
    {
        foreach (AudioSource sfx in sfxSources)
        {
            sfx.volume = (float)value;
        }
    }

    private void ApplyMainSFXSettings(object value)
    {
        foreach (AudioSource sfx in mainSfxSources)
        {
            sfx.volume = (float)value;
        }
    }
}
