using UnityEngine;

public class Settings : MonoBehaviour
{
    private static Settings instance;

    public static Settings Instance { get { return instance; } }

    private float SFXVolume = 0.1f, BGMVolume = 0.1f, mainSFXVolume = 1f;

    private bool rtxEnabled = false;
    private bool hasSkins = false;

    public Language activeLanguage = Language.en;

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

    public void SetLanguage(Language language)
    {
        activeLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
        EventSystem.InvokeEvent(EventType.UPDATE_LANGUAGE);
    }

    public void SetFlagColor(Color color)
    {
        PlayerPrefs.SetFloat("Flag_R", color.r);
        PlayerPrefs.SetFloat("Flag_G", color.g);
        PlayerPrefs.SetFloat("Flag_B", color.b);
    }

    public Color GetFlagColor()
    {
        if (PlayerPrefs.HasKey("Flag_R") && hasSkins)
        {
            return new Color(PlayerPrefs.GetFloat("Flag_R"), PlayerPrefs.GetFloat("Flag_G"), PlayerPrefs.GetFloat("Flag_B"));
        }
        else
        {
            // default blue color for flags
            return new Color(0f / 255f, 123f / 255f, 255f / 255f);
        }
    }

    public Language GetLanguage()
    {
        return activeLanguage;
    }

    public void HasSkins()
    {
        hasSkins = true;
    }

    public int GetFlagIndex()
    {
        if (PlayerPrefs.HasKey("Flag_Type") && hasSkins)
        {
            return PlayerPrefs.GetInt("Flag_Type");
        }
        else
        {
            // default flag
            return 0;
        }
    }

    public void SetFlagIndex(int flagIndex)
    {
        PlayerPrefs.SetInt("Flag_Type", flagIndex);
    }
}
