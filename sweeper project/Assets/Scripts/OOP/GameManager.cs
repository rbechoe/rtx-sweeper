using UnityEngine;
using TMPro;

public class GameManager : Base
{
    [Header("Prefabs")]
    public GameObject tile;

    [Header("Settings")]
    public GameObject mainCam;
    public int gridSize;
    public int bombAmount = 10;

    [SerializeField]
    private Spawner spawner;

    [SerializeField]
    private TMP_InputField xTMP;
    [SerializeField]
    private TMP_InputField zTMP;
    [SerializeField]
    private TMP_InputField bombTMP;

    private int goodTiles;

    public float timer { get; private set; }
    public bool gameActive { get; set; }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.PREPARE_GAME, StartGame);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.PREPARE_GAME, StartGame);
    }

    protected override void Update()
    {
        base.Update();

        if (gameActive)
        {
            timer += Time.deltaTime;
        }
    }

    public void SetupEasy()
    {
        gridSize = 9;
        bombAmount = 10;
        SetupGame(gridSize, gridSize, bombAmount);
    }

    public void SetupMedium()
    {
        gridSize = 16;
        bombAmount = 40;
        SetupGame(gridSize, gridSize, bombAmount);
    }

    public void SetupHard()
    {
        gridSize = 30;
        bombAmount = 130;
        SetupGame(gridSize, gridSize, bombAmount);
    }

    public void SetupCustom()
    {
        gridSize = int.Parse(xTMP.text) * int.Parse(zTMP.text);
        bombAmount = int.Parse(bombTMP.text);
        bombAmount = Mathf.Clamp(bombAmount, 0, gridSize - 1);
        SetupGame(int.Parse(xTMP.text), int.Parse(zTMP.text), bombAmount);
    }

    private void SetupGame(int _x, int _z, int _bombCount)
    {
        mainCam.transform.position = new Vector3(_x / 2f * 0.9f, (_x + _z / 2f) * 1.1f, (_z / 2f - 0.5f) * 1.2f);
        spawner.CreateGrid(_x, _z, _bombCount, this);
    }

    public void AddGoodTile()
    {
        goodTiles++;
        CheckForVictory();
    }

    private void CheckForVictory()
    {
        if (goodTiles == Mathf.Pow(gridSize, 2) - bombAmount)
        {
            EndGame();
            gameObject.GetComponent<UIManager>().ShowVictory();
        }
    }

    private void StartGame(object value)
    {
        EventSystem<Parameters>.InvokeEvent(EventType.COUNT_BOMBS, new Parameters());
        EventSystem<Parameters>.InvokeEvent(EventType.START_GAME, new Parameters());
        timer = 0;
        goodTiles = 0;
        gameObject.GetComponent<UIManager>().bombs = bombAmount;
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
        EventSystem<Parameters>.InvokeEvent(EventType.END_GAME, new Parameters());
    }
}