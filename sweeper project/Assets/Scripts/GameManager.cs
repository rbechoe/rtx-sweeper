using UnityEngine;
using TMPro;

public class GameManager : Base
{
    [Header("Prefabs")]
    public GameObject tile;

    [Header("Settings")]
    public GameObject target;
    public int gridSize;
    public int bombAmount = 10;

    [SerializeField]
    private Spawner spawner;
    [SerializeField]
    private DifficultSpawner spawner3D;

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

    public void SetupEasy3D()
    {
        gridSize = 4;
        bombAmount = 10;
        Setup3DGame(gridSize, bombAmount);
    }

    public void SetupMedium3D()
    {
        gridSize = 6;
        bombAmount = 48;
        Setup3DGame(gridSize, bombAmount);
    }

    public void SetupHard3D()
    {
        gridSize = 10;
        bombAmount = 300;
        Setup3DGame(gridSize, bombAmount);
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
        Parameters param = new Parameters();
        param.vector3s.Add(new Vector3(_x / 2f, (_x + _z / 2f) * 0.5f, _z / 2f));
        EventSystem<Parameters>.InvokeEvent(EventType.START_POS, param);
        spawner.CreateGrid(_x, _z, _bombCount, this);
    }

    private void Setup3DGame(int _gridSize, int _bombCount)
    {
        spawner3D.CreateGrid(_gridSize, _bombCount, this);
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
        EventSystem<Parameters>.InvokeEvent(EventType.PICK_TILE, new Parameters());
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