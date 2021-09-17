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
        EventSystem<Vector3>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem.AddListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Vector3>.AddListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
    }

    private void OnDisable()
    {
        EventSystem<Vector3>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Vector3>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
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
        if (inactiveFlags.Count > 0 && bombs > 0)
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

    private void ResetGame()
    {
        isDone = false;
        bombCount = 0;
        StartCoroutine(ResetLogic());        
    }

    // super efficient system......not
    IEnumerator ResetLogic()
    {
        EventSystem.InvokeEvent(EventType.END_GAME);

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

    private void AddFlag(Vector3 empty)
    {
        bombs--;
    }

    private void RemoveFlag(GameObject empty)
    {
        bombs++;
    }
}