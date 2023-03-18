using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Assignables")]
    public Locker tutorial;
    public Locker arctic, asia, desert, islands, galaxy;
    public GameObject rtxButtons, rtxOffText;

    [Header("Buttons")]
    public Button playBtn;
    public Button asiaBtn, desertBtn, bossBtn, galaxyBtn, graphicsBtn, rtxOnBtn, rtxOffBtn, gardenBtn, skinsBtn;

    [Header("Sliders")]
    public Slider BGMSlider;
    public Slider mainSFXSlider, SFXSlider;

    // color picker settings
    [Header("Customization")]
    public RawImage flagColor;
    public Slider flagR, flagG, flagB;
    public TMP_InputField inputFlagR, inputFlagG, inputFlagB;
    public ColorPicker flagPicker;

    private int xSize;
    private int zSize;
    private int gridSize;
    private int bombAmount;

    private DataSerializer dataSerializer;

    private SteamAPIManager steamAPI;

    private void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        BGMSlider.value = Settings.Instance.GetBGMVolume();
        SFXSlider.value = Settings.Instance.GetSFXVolume();
        mainSFXSlider.value = Settings.Instance.GetMainSFXVolume();

        // enable disable correct rtx buttons
        if (Settings.Instance.GetRTX())
        {
            rtxOnBtn.gameObject.SetActive(false);
            rtxOffBtn.gameObject.SetActive(true);
        }
        else
        {
            rtxOnBtn.gameObject.SetActive(true);
            rtxOffBtn.gameObject.SetActive(false);
        }

        // if version is 12 then enable graphics button otherwise disable it
        string dxVersion = SystemInfo.graphicsDeviceVersion;
        if (dxVersion.Contains("Direct3D 12"))
        {
            rtxButtons.SetActive(true);
            rtxOffText.SetActive(false);
        }
        else
        {
            rtxButtons.SetActive(false);
            rtxOffText.SetActive(true);
        }

        // disable interactables based on savedata
        dataSerializer = gameObject.GetComponent<DataSerializer>();
        AccountData accountData = dataSerializer.GetUserData();
        if (accountData.tutorialVictories > 0) playBtn.gameObject.SetActive(true);
        else playBtn.gameObject.SetActive(false);
        if (accountData.arcticVictories > 0) asiaBtn.gameObject.SetActive(true);
        else asiaBtn.gameObject.SetActive(false);
        if (accountData.asiaVictories > 0) desertBtn.gameObject.SetActive(true);
        else desertBtn.gameObject.SetActive(false);
        if (accountData.desertVictories > 0) bossBtn.gameObject.SetActive(true);
        else bossBtn.gameObject.SetActive(false);
        if (accountData.bossVictories > 0) galaxyBtn.gameObject.SetActive(true);
        else galaxyBtn.gameObject.SetActive(false);

        // enable pretty environments based on progression
        if (accountData.tutorialVictories > 0) tutorial.UnlockAreas();
        if (accountData.arcticVictories > 0) arctic.UnlockAreas();
        if (accountData.asiaVictories > 0) asia.UnlockAreas();
        if (accountData.desertVictories > 0) desert.UnlockAreas();
        if (accountData.bossVictories > 0) islands.UnlockAreas();
        if (accountData.galaxyVictories > 0) galaxy.UnlockAreas();

        // dlc's
        if (accountData.hasCosmetics == 0 && steamAPI.CheckDLC((AppId_t)2166670))
        {
            accountData.hasCosmetics = 1;
            dataSerializer.UpdateAccountData(accountData);
        }
        if (accountData.hasCosmetics > 0)
        {
            gardenBtn.gameObject.SetActive(true);
            skinsBtn.gameObject.SetActive(true);
            Settings.Instance.HasSkins();

            Color flagColor = Settings.Instance.GetFlagColor();
            flagR.value = flagColor.r * 255;
            flagG.value = flagColor.g * 255;
            flagB.value = flagColor.b * 255;
            inputFlagR.text = "" + flagColor.r * 255;
            inputFlagG.text = "" + flagColor.g * 255;
            inputFlagB.text = "" + flagColor.b * 255;
            flagPicker.SetRV(flagColor.r * 255);
            flagPicker.SetGV(flagColor.g * 255);
            flagPicker.SetBV(flagColor.b * 255);
            flagPicker.SetA(255, true);
        }
        else
        {
            skinsBtn.gameObject.SetActive(false);
        }
    }

    public void Easy2D()
    {
        gridSize = 9;
        xSize = 9;
        zSize = 9;
        bombAmount = 10;
        NewGame2D();
    }

    public void Medium2D()
    {
        gridSize = 16;
        xSize = 16;
        zSize = 16;
        bombAmount = 40;
        NewGame2D();
    }

    public void Hard2D()
    {
        gridSize = 30;
        xSize = 30;
        zSize = 30;
        bombAmount = 130;
        NewGame2D();
    }

    public void Custom2D()
    {
        xSize = 10;
        zSize = 10;
        bombAmount = 10;
        NewGame2D();
    }

    public void Medium3D()
    {
        NewGame3D();
    }

    public void UpdateColors()
    {
        Settings.Instance.SetFlagColor(flagColor.color);
    }

    private void NewGame2D()
    {
        TheCreator.Instance.xSize = xSize;
        TheCreator.Instance.zSize = zSize;
        TheCreator.Instance.gridSize = gridSize;
        TheCreator.Instance.bombAmount = bombAmount;
        SceneManager.LoadScene("Garden");
    }

    private void NewGame3D()
    {
        SceneManager.LoadScene("Universe");
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

    public void SaveSettings()
    {
        // implement by writing to settings file
    }

    public void EnableRTX()
    {
        EventSystem.InvokeEvent(EventType.ENABLE_RTX);
    }

    public void DisableRTX()
    {
        EventSystem.InvokeEvent(EventType.DISABLE_RTX);
    }

    public void LoadAsia()
    {
        SceneManager.LoadScene("Asia");
    }

    public void LoadBoss()
    {
        SceneManager.LoadScene("Boss Scene");
    }

    public void LoadDesert()
    {
        SceneManager.LoadScene("Desert");
    }

    public void LoadArctic()
    {
        SceneManager.LoadScene("Arctic");
    }

    public void LoadGarden()
    {
        SceneManager.LoadScene("Garden");
    }

    public void LoadGardenPlus()
    {
        SceneManager.LoadScene("Garden Unlimited");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void LoadAnomaly()
    {
        SceneManager.LoadScene("Anomaly");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSkinsDLC()
    {
        Application.OpenURL("https://store.steampowered.com/app/2166670");
    }
}