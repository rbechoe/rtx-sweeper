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

    protected override void Start()
    {
        base.Start();
        SetupGame();
    }

    private void SetupGame()
    {
        mainCam.transform.position = new Vector3(gridSize / 2f, gridSize, gridSize / 2f - 0.5f);
        spawner.CreateGrid(gridSize, bombAmount, this);
    }

    public void SetCheckers()
    {

    }
}
