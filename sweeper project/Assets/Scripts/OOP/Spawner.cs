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
    private int xSize = 0;
    private int zSize = 0;
    private int bombs = 0;

    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> activeFlags = new List<GameObject>();
    private List<GameObject> inactiveFlags = new List<GameObject>();

    GameManager gameManager;

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<Parameters>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<Parameters>.AddListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Parameters>.AddListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<Parameters>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<Parameters>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<Parameters>.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Parameters>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<Parameters>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
    }

    public void CreateGrid(int _x, int _z, int _bombAmount, GameManager _gameManager)
    {
        gridSize = _x * _z;
        bombAmount = _bombAmount;
        bombs = _bombAmount;
        gameManager = _gameManager;
        xSize = _x;
        zSize = _z;
        StartCoroutine(Grid());
    }

    private IEnumerator Grid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;
        GameObject newTile = null;

        // TODO track empty tile for no guess start and highlight it during start event
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    tilesLeft = gridSize - curTile;
                    spawnChance = tilesLeft / (bombAmount - bombCount);
                }

                newTile = Instantiate(tile, new Vector3(x, 0, z), Quaternion.identity);
                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.AddComponent<Bomb>();
                    newTile.GetComponent<Bomb>().SetGameManager(gameManager);
                    bombCount++;

                    // create flag for the pool, 1 flag per bomb
                    if (inactiveFlags.Count < curTile)
                    {
                        AddNewFlag();
                    }
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
        EventSystem<Parameters>.InvokeEvent(EventType.PREPARE_GAME, new Parameters());
    }

    // add flag to pool
    private void AddNewFlag()
    {
        GameObject _flag = Instantiate(flag, Vector3.up * 5000, Quaternion.identity);
        inactiveFlags.Add(_flag);
    }

    // activate a flag and place it above the tile
    private void ActivateFlag(Parameters param)
    {
        if (inactiveFlags.Count > 0 && bombs > 0)
        {
            inactiveFlags[0].transform.position = param.vector3s[0];
            activeFlags.Add(inactiveFlags[0]);
            inactiveFlags.RemoveAt(0);
        }
    }

    // remove a flag from the tile
    public void ReturnFlag(Parameters param)
    {
        GameObject _flag = param.gameObjects[0];
        _flag.transform.position = Vector3.up * 5000;
        activeFlags.Remove(_flag);
        inactiveFlags.Add(_flag);
    }

    private void ResetGame(object value)
    {
        isDone = false;
        bombCount = 0;
        StartCoroutine(ResetLogic());        
    }

    // super efficient system......not
    IEnumerator ResetLogic()
    {
        EventSystem<Parameters>.InvokeEvent(EventType.END_GAME, new Parameters());

        // remove all flags and tiles
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        tiles = new List<GameObject>();
        yield return new WaitForEndOfFrame();

        foreach (GameObject flag in activeFlags)
        {
            Destroy(flag);
        }
        activeFlags = new List<GameObject>();
        yield return new WaitForEndOfFrame();

        foreach (GameObject flag in inactiveFlags)
        {
            Destroy(flag);
        }
        inactiveFlags = new List<GameObject>();
        yield return new WaitForEndOfFrame();

        // start spawner with current settings
        StartCoroutine(Grid());
    }

    private void AddFlag(object value)
    {
        bombs--;
    }

    private void RemoveFlag(object value)
    {
        bombs++;
    }
}