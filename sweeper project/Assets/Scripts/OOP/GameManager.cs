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

    [SerializeField]
    private List<GameObject> tiles;

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
            SetCheckers();
        }

        if (gameActive)
        {
            timer += Time.deltaTime;
        }
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

    public void SetTiles(List<GameObject> _tiles)
    {
        tiles = _tiles;
    }

    public void SetCheckers()
    {
        StartCoroutine(CheckBombs());
    }

    public void AddGoodTile()
    {
        goodTiles++;
        CheckForVictory();
    }

    void CheckForVictory()
    {
        if (goodTiles == Mathf.Pow(gridSize, 2) - bombAmount)
        {
            EndGame();
            gameObject.GetComponent<UIManager>().ShowVictory();
            print("Victory!!");
        }
    }

    IEnumerator CheckBombs()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponent<Checker>().CheckBombs();
            tiles[i].GetComponent<Tile>().Clickable();
        }
        yield return new WaitForEndOfFrame();
        StartGame();
    }

    public void StartGame()
    {
        gameObject.GetComponent<UIManager>().bombs = bombAmount;
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
        StartCoroutine(DisableTiles());
    }

    IEnumerator DisableTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponent<Tile>().Unclickable();
        }
        yield return new WaitForEndOfFrame();
    }
}