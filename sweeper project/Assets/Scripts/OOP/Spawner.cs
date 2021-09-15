using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Base
{
    [Header("Prefabs")]
    public GameObject tile;
    public GameObject flag;

    [Header("Statistics")]
    public bool isDone;
    public int bombCount = 0;

    private int gridSize;
    private int bombAmount = 10;

    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> activeFlags = new List<GameObject>();
    private List<GameObject> inactiveFlags = new List<GameObject>();

    GameManager gameManager;

    public void CreateGrid(int _gridSize, int _bombAmount, GameManager _gameManager)
    {
        gridSize = _gridSize;
        bombAmount = _bombAmount;
        gameManager = _gameManager;
        StartCoroutine(Grid());
    }

    private void OnEnable()
    {
        EventSystem<Vector3>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
    }

    private void OnDisable()
    {
        EventSystem<Vector3>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
    }

    private IEnumerator Grid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;
        GameObject newTile = null;

        // TODO track empty tile for no guess start
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;
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

                newTile = Instantiate(tile, new Vector3(x, 0, z), Quaternion.identity);
                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.AddComponent<Bomb>();
                    newTile.GetComponent<Bomb>().SetGameManager(gameManager);
                    bombCount++;
                }
                else
                {
                    newTile.AddComponent<Empty>();
                    newTile.GetComponent<Empty>().SetGameManager(gameManager);
                }

                curTile++;
                curTileCount++;
                newTile.name = "tile " + curTile;
                tiles.Add(newTile);

                // create flag for the pool
                if (inactiveFlags.Count < curTile)
                {
                    AddNewFlag();
                }

                // continue next frame
                if (curTileCount >= tilesPerFrame)
                {
                    curTileCount = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        yield return new WaitForEndOfFrame();
        isDone = true;
        EventSystem.InvokeEvent(EventType.PREPARE_GAME);
    }

    // add flag to pool
    private void AddNewFlag()
    {
        GameObject _flag = Instantiate(flag, Vector3.up * 5000, Quaternion.identity);
        inactiveFlags.Add(_flag);
    }

    // activate a flag and place it above the tile
    private void ActivateFlag(Vector3 position)
    {
        if (inactiveFlags.Count > 0)
        {
            inactiveFlags[0].transform.position = position;
            activeFlags.Add(inactiveFlags[0]);
            inactiveFlags.RemoveAt(0);
        }
    }

    // remove a flag from the tile
    public void ReturnFlag(GameObject flag)
    {
        flag.transform.position = Vector3.up * 5000;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
    }
}