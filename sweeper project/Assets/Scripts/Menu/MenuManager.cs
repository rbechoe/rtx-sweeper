using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField xTMP;
    [SerializeField]
    private TMP_InputField zTMP;
    [SerializeField]
    private TMP_InputField bombTMP;

    public Button playBtn, asiaBtn, desertBtn, bossBtn, graphicsBtn, rtxOnBtn, rtxOffBtn;
    public Slider BGMSlider, SFXSlider, mainSFXSlider;

    public Locker tutorial, arctic, asia, desert, islands;

    private int xSize;
    private int zSize;
    private int gridSize;
    private int bombAmount;

    private DataSerializer dataSerializer;

    private void Start()
    {
        Settings settings = GameObject.FindGameObjectWithTag("Settings").GetComponent<Settings>();
        BGMSlider.value = settings.GetBGMVolume();
        SFXSlider.value = settings.GetSFXVolume();
        mainSFXSlider.value = settings.GetMainSFXVolume();

        // enable disable correct rtx buttons
        if (settings.GetRTX())
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
            graphicsBtn.interactable = true;
        }
        else
        {
            graphicsBtn.interactable = false;
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

        // enable pretty environments based on progression
        if (accountData.tutorialVictories > 0) tutorial.UnlockAreas();
        if (accountData.arcticVictories > 0) arctic.UnlockAreas();
        if (accountData.asiaVictories > 0) asia.UnlockAreas();
        if (accountData.desertVictories > 0) desert.UnlockAreas();
        if (accountData.bossVictories > 0) islands.UnlockAreas();
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
        xSize = int.Parse(xTMP.GetComponent<TMP_InputField>().text);
        zSize = int.Parse(zTMP.GetComponent<TMP_InputField>().text);
        bombAmount = int.Parse(bombTMP.GetComponent<TMP_InputField>().text);
        NewGame2D();
    }

    public void Easy3D()
    {
        gridSize = 4;
        bombAmount = 8;
        NewGame3D();
    }

    public void Medium3D()
    {
        gridSize = 6;
        bombAmount = 30;
        NewGame3D();
    }

    public void Hard3D()
    {
        gridSize = 10;
        bombAmount = 80;
        NewGame3D();
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
        TheCreator.Instance.gridSize = gridSize;
        TheCreator.Instance.bombAmount = bombAmount;
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

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}