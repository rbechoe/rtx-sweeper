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
    private GameObject[] tiles;

    [Header("Debug")]
    public bool forceCheck;

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
        tiles = new GameObject[(int)Mathf.Pow(gridSize, 2)];
    }

    public void SetTiles(GameObject[] _tiles)
    {
        tiles = _tiles;
    }

    public void SetCheckers()
    {
        foreach(GameObject _tile in tiles)
        {
            _tile.GetComponent<Checker>().CheckBombs();
        }
    }
}
