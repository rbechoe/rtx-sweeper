using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Debug")]
    public bool forceCheck;

    private int goodTiles;

    public float timer { get; private set; }
    public bool gameActive { get; set; }

    protected override void Update()
    {
        base.Update();
        if (forceCheck)
        {
            forceCheck = false;
            StartGame();
        }

        if (gameActive)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.PREPARE_GAME, StartGame);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.PREPARE_GAME, StartGame);
    }

    public void SetupEasy()
    {
        gridSize = 9;
        bombAmount = 10;
        SetupGame();
    }

    public void SetupMedium()
    {
        gridSize = 16;
        bombAmount = 40;
        SetupGame();
    }

    public void SetupHard()
    {
        gridSize = 30;
        bombAmount = 130;
        SetupGame();
    }

    private void SetupGame()
    {
        mainCam.transform.position = new Vector3(gridSize / 2f * 0.9f, gridSize * 1.1f, (gridSize / 2f - 0.5f) * 1.2f);
        spawner.CreateGrid(gridSize, bombAmount, this);
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

    private void StartGame()
    {
        EventSystem.InvokeEvent(EventType.COUNT_BOMBS, gameObject.name);
        EventSystem.InvokeEvent(EventType.START_GAME, gameObject.name);
        timer = 0;
        gameObject.GetComponent<UIManager>().bombs = bombAmount;
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
        EventSystem.InvokeEvent(EventType.END_GAME, gameObject.name);
    }
}