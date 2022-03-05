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

    public Button playBtn, asiaBtn, desertBtn, bossBtn;
    public Slider BGMSlider, SFXSlider, mainSFXSlider;

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

        // disable interactables based on savedata
        dataSerializer = gameObject.GetComponent<DataSerializer>();
        AccountData accountData = dataSerializer.GetUserData();
        if (accountData.tutorialVictories > 0) playBtn.interactable = true;
        if (accountData.arcticVictories > 0) asiaBtn.interactable = true;
        if (accountData.asiaVictories > 0) desertBtn.interactable = true;
        if (accountData.desertVictories > 0) bossBtn.gameObject.SetActive(true);
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