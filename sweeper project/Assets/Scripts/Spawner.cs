using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Base
{
    [Header("Prefabs")]
    public GameObject tile;

    [Header("Statistics")]
    public bool isDone;
    public int bombCount = 0;

    private int gridSize;
    private int bombAmount = 10;

    GameManager gameManager;

    public void CreateGrid(int _gridSize, int _bombAmount, GameManager _gameManager)
    {
        gridSize = _gridSize;
        bombAmount = _bombAmount;
        gameManager = _gameManager;
        StartCoroutine(Grid());
    }

    private IEnumerator Grid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    tilesLeft = (int)Mathf.Pow(gridSize, 2) - curTile;
                    spawnChance = tilesLeft / (bombAmount - bombCount);
                }    

                GameObject newTile = Instantiate(tile, new Vector3(x, 0, z), Quaternion.identity);
                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.AddComponent<Bomb>();
                    bombCount++;
                }
                else
                {
                    newTile.AddComponent<Empty>();
                }

                curTile++;
                yield return new WaitForEndOfFrame();
            }
        }

        isDone = true;
        gameManager.SetCheckers();
    }
}