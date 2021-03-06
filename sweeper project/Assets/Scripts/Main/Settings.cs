using UnityEngine;

public class Settings : MonoBehaviour
{
    private static Settings instance;

    public static Settings Instance { get { return instance; } }

    private float SFXVolume = 0.1f, BGMVolume = 0.1f, mainSFXVolume = 1f;

    private bool rtxEnabled = false;

    SteamAPIManager steamAPI;

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

    private void Start()
    {
        int firstTime = PlayerPrefs.GetInt("First_Time");
        if (firstTime == 0)
        {
            PlayerPrefs.SetInt("First_Time", 1);
        }
        else
        {
            SFXVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SFX_Volume"));
            BGMVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("BGM_Volume"));
            mainSFXVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("Main_SFX_Volume"));
            rtxEnabled = (PlayerPrefs.GetInt("RTX") == 0) ? false : true;
        }

        steamAPI = SteamAPIManager.Instance;
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKey(KeyCode.F4))
        {
            steamAPI.SetAchievement(UserAchievements.ragequit);
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
        PlayerPrefs.SetFloat("SFX_Volume", SFXVolume);
    }

    public float GetSFXVolume()
    {
        return SFXVolume;
    }

    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
        PlayerPrefs.SetFloat("BGM_Volume", BGMVolume);
    }

    public float GetBGMVolume()
    {
        return BGMVolume;
    }

    public void SetMainSFXVolume(float value)
    {
        mainSFXVolume = value;
        PlayerPrefs.SetFloat("Main_SFX_Volume", mainSFXVolume);
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
        PlayerPrefs.SetInt("RTX", 1);
    }

    public void DisableRTX()
    {
        rtxEnabled = false;
        PlayerPrefs.SetInt("RTX", 0);
    }
}
