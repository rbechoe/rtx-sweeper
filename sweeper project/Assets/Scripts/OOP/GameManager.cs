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

    protected override void Start()
    {
        base.Start();
        SetupGame();
    }

    protected override void Update()
    {
        base.Update();
        if (forceCheck)
        {
            forceCheck = false;
            SetCheckers();
        }
    }

    private void SetupGame()
    {
        mainCam.transform.position = new Vector3(gridSize / 2f, gridSize, gridSize / 2f - 0.5f);
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
            print("Victory!!");
        }
    }

    IEnumerator CheckBombs()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].GetComponent<Checker>().CheckBombs();
        }
        yield return new WaitForEndOfFrame();
    }
}