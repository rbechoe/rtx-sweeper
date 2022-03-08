using UnityEngine;

public class Settings : MonoBehaviour
{
    private static Settings instance;

    public static Settings Instance { get { return instance; } }

    private float SFXVolume = 0.1f, BGMVolume = 0.1f, mainSFXVolume = 1f;

    private bool rtxEnabled = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        EventSystem<float>.AddListener(EventType.UPDATE_SFX, SetSFXVolume);
        EventSystem<float>.AddListener(EventType.UPDATE_BGM, SetBGMVolume);
        EventSystem<float>.AddListener(EventType.UPDATE_SFX_MAIN, SetMainSFXVolume);
        EventSystem.AddListener(EventType.ENABLE_RTX, EnableRTX);
        EventSystem.AddListener(EventType.DISABLE_RTX, DisableRTX);
    }

    private void OnDisable()
    {
        EventSystem<float>.RemoveListener(EventType.UPDATE_SFX, SetSFXVolume);
        EventSystem<float>.RemoveListener(EventType.UPDATE_BGM, SetBGMVolume);
        EventSystem<float>.RemoveListener(EventType.UPDATE_SFX_MAIN, SetMainSFXVolume);
        EventSystem.RemoveListener(EventType.ENABLE_RTX, EnableRTX);
        EventSystem.RemoveListener(EventType.DISABLE_RTX, DisableRTX);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
    }

    public float GetSFXVolume()
    {
        return SFXVolume;
    }

    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
    }

    public float GetBGMVolume()
    {
        return BGMVolume;
    }

    public void SetMainSFXVolume(float value)
    {
        mainSFXVolume = value;
    }

    public float GetMainSFXVolume()
    {
        return mainSFXVolume;
    }

    public bool GetRTX()
    {
        return rtxEnabled;
    }

    public void EnableRTX()
    {
        rtxEnabled = true;
    }

    public void DisableRTX()
    {
        rtxEnabled = false;
    }
}
