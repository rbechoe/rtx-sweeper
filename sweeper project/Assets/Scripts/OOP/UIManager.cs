using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Base
{
    public TextMeshProUGUI timerTMP;
    public TextMeshProUGUI bombsTMP;

    GameManager gameManager;

    public int bombs { get; set; }
    public int flags { get; set; }

    public GameObject victory;

    protected override void Start()
    {
        base.Start();
        gameManager = gameObject.GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<Parameters>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<Parameters>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
    }

    protected override void Update()
    {
        base.Update();
        UpdateUI();
    }

    private void UpdateUI()
    {
        timerTMP.text = "" + Mathf.FloorToInt(gameManager.timer);
        bombsTMP.text = "" + bombs;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("sweeper rtx");
    }

    public void ShowVictory()
    {
        victory.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGame()
    {
        victory.SetActive(false);
        EventSystem<Parameters>.InvokeEvent(EventType.RESET_GAME, new Parameters());
    }

    private void AddFlag(object value)
    {
        if (bombs > 0)
        {
            bombs--;
        }
    }

    private void RemoveFlag(object value)
    {
        bombs++;
    }
}