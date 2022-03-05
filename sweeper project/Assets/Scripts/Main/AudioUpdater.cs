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
        EventSystem<float>.AddListener(EventType.UPDATE_BGM, ApplyBGMSettings);
        EventSystem<float>.AddListener(EventType.UPDATE_SFX, ApplySFXSettings);
        EventSystem<float>.AddListener(EventType.UPDATE_SFX_MAIN, ApplyMainSFXSettings);
    }

    private void OnDisable()
    {
        EventSystem<float>.RemoveListener(EventType.UPDATE_BGM, ApplyBGMSettings);
        EventSystem<float>.RemoveListener(EventType.UPDATE_SFX, ApplySFXSettings);
        EventSystem<float>.RemoveListener(EventType.UPDATE_SFX_MAIN, ApplyMainSFXSettings);
    }

    private void ApplyBGMSettings(float value)
    {
        foreach(AudioSource bgm in bgmSources)
        {
            bgm.volume = value;
        }
    }

    private void ApplySFXSettings(float value)
    {
        foreach (AudioSource sfx in sfxSources)
        {
            sfx.volume = value;
        }
    }

    private void ApplyMainSFXSettings(float value)
    {
        foreach (AudioSource sfx in mainSfxSources)
        {
            sfx.volume = value;
        }
    }
}
