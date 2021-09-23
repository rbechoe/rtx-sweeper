using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultSpawner : Base
{
    [Header("Prefabs")]
    public GameObject tile;
    public GameObject flag;
    public GameObject parentTile;

    [Header("Statistics")]
    public bool isDone;
    public int bombCount = 0;

    private int gridSize = 4;
    private int bombAmount = 10;
    private int bombs = 0;

    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> activeFlags = new List<GameObject>();
    private List<GameObject> inactiveFlags = new List<GameObject>();

    private GameObject firstTile;
    private List<GameObject> emptyTiles = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        gridSize = TheCreator.Instance.gridSize;
        bombAmount = TheCreator.Instance.bombAmount;
        Parameters param = new Parameters();
        param.vector3s.Add(new Vector3(gridSize / 2f, gridSize, gridSize / 2f));
        EventSystem<Parameters>.InvokeEvent(EventType.START_POS, param);
        CreateGrid(gridSize, bombAmount);
    }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<Parameters>.AddListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<Parameters>.AddListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Parameters>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<Parameters>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
        EventSystem<Parameters>.AddListener(EventType.PICK_TILE, PickStartingTile);
        EventSystem<Parameters>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<Parameters>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem<Parameters>.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem<Parameters>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<Parameters>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
        EventSystem<Parameters>.RemoveListener(EventType.PICK_TILE, PickStartingTile);
        EventSystem<Parameters>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
    }

    public void CreateGrid(int _gridSize, int _bombAmount)
    {
        gridSize = _gridSize;
        bombAmount = _bombAmount;
        bombs = _bombAmount;
        StartCoroutine(Grid());
    }

    private IEnumerator Grid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;
        GameObject newTile = null;

        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;
        for (float x = -(gridSize / 2f); x < (gridSize / 2f); x++)
        {
            for (float z = -(gridSize / 2f); z < (gridSize / 2f); z++)
            {
                for (float y = -(gridSize / 2f); y < (gridSize / 2f); y++)
                {
                    // formula: based on tiles and bombs left increase chance for next tile to be bomb
                    if (bombCount < bombAmount)
                    {
                        tilesLeft = int.Parse(Mathf.Pow(gridSize, 3).ToString()) - curTile;
                        spawnChance = tilesLeft / (bombAmount - bombCount);
                    }

                    newTile = Instantiate(tile, new Vector3(x + .5f, y + .5f, z + .5f), Quaternion.identity);
                    newTile.transform.localScale = Vector3.one * 0.9f;

                    if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                    {
                        newTile.AddComponent<Bomb3D>();
                        bombCount++;

                        // create flag for the pool, 1 flag per bomb
                        if (inactiveFlags.Count < curTile)
                        {
                            AddNewFlag();
                        }
                    }
                    else
                    {
                        newTile.AddComponent<Empty3D>();
                    }

                    curTile++;
                    curTileCount++;
                    newTile.name = "tile " + curTile;
                    tiles.Add(newTile);
                    newTile.transform.parent = parentTile.transform;

                    // continue next frame
                    if (curTileCount >= tilesPerFrame)
                    {
                        curTileCount = 0;
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();

        isDone = true;
        EventSystem<Parameters>.InvokeEvent(EventType.PREPARE_GAME, new Parameters());
        yield return new WaitForEndOfFrame();
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

    private void AddEmptyTile(Parameters param)
    {
        emptyTiles.Add(param.gameObjects[0]);
    }

    private void PickStartingTile(object value)
    {
        if (emptyTiles.Count > 0)
        {
            firstTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
            firstTile.GetComponent<Tile3D>().FirstTile();
        }
    }

    private void ResetGame(object value)
    {
        isDone = false;
        bombCount = 0;
        firstTile = null;
        StartCoroutine(ResetLogic());
    }

    // super efficient system......not
    IEnumerator ResetLogic()
    {
        emptyTiles = new List<GameObject>();
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;

        // remove all flags and tiles
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
            curTileCount++;
            if (curTileCount >= tilesPerFrame)
            {
                curTileCount = 0;
                yield return new WaitForEndOfFrame();
            }
        }
        tiles = new List<GameObject>();
        yield return new WaitForEndOfFrame();

        foreach (GameObject flag in activeFlags)
        {
            Destroy(flag);
            curTileCount++;
            if (curTileCount >= tilesPerFrame)
            {
                curTileCount = 0;
                yield return new WaitForEndOfFrame();
            }
        }
        activeFlags = new List<GameObject>();
        yield return new WaitForEndOfFrame();

        foreach (GameObject flag in inactiveFlags)
        {
            Destroy(flag);
            curTileCount++;
            if (curTileCount >= tilesPerFrame)
            {
                curTileCount = 0;
                yield return new WaitForEndOfFrame();
            }
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
