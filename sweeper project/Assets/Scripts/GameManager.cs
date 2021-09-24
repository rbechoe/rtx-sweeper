using UnityEngine;

public class GameManager : Base
{
    [Header("Settings")]
    public int gridSize;
    public int bombAmount = 10;

    private int goodTiles;

    public float timer { get; private set; }
    public bool gameActive { get; set; }

    protected override void Start()
    {
        base.Start();
        gridSize = TheCreator.Instance.gridSize;
        bombAmount = TheCreator.Instance.bombAmount;
    }

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

    public void AddGoodTile3D()
    {
        goodTiles++;
        CheckForVictory3D();
    }

    private void CheckForVictory3D()
    {
        if (goodTiles == Mathf.Pow(gridSize, 3) - bombAmount)
        {
            EndGame();
            EventSystem<Parameters>.InvokeEvent(EventType.WIN_GAME, new Parameters());
        }
    }

    public void AddGoodTile2D()
    {
        goodTiles++;
        CheckForVictory2D();
    }

    private void CheckForVictory2D()
    {
        if (goodTiles == Mathf.Pow(gridSize, 2) - bombAmount)
        {
            EndGame();
            EventSystem<Parameters>.InvokeEvent(EventType.WIN_GAME, new Parameters());
        }
    }

    private void StartGame(object value)
    {
        Parameters param = new Parameters();
        param.integers.Add(bombAmount);
        EventSystem<Parameters>.InvokeEvent(EventType.COUNT_BOMBS, new Parameters());
        EventSystem<Parameters>.InvokeEvent(EventType.PICK_TILE, new Parameters());
        EventSystem<Parameters>.InvokeEvent(EventType.START_GAME, new Parameters());
        EventSystem<Parameters>.InvokeEvent(EventType.BOMB_UPDATE, param);
        timer = 0;
        goodTiles = 0;
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
        EventSystem<Parameters>.InvokeEvent(EventType.END_GAME, new Parameters());
    }
}