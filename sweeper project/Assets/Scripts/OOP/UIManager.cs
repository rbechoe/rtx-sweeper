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

    protected override void Update()
    {
        base.Update();
        UpdateUI();
    }
    private void OnEnable()
    {
        EventSystem<Vector3>.AddListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
    }

    private void OnDisable()
    {
        EventSystem<Vector3>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
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
        EventSystem.InvokeEvent(EventType.RESET_GAME);
    }

    private void AddFlag(Vector3 empty)
    {
        if (bombs > 0)
        {
            bombs--;
        }
    }

    private void RemoveFlag(GameObject empty)
    {
        bombs++;
    }
}