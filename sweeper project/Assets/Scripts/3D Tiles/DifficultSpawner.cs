using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultSpawner : Base
{
    [Header("Prefabs")]
    public GameObject tile;
    public GameObject flag;
    public GameObject parentTile;
    [SerializeField]
    private GameObject target;

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
        Vector3 position = new Vector3(gridSize / 2f, gridSize, gridSize / 2f);
        EventSystem<Vector3>.InvokeEvent(EventType.START_POS, position);
        CreateGrid(gridSize, bombAmount);
    }

    private void OnEnable()
    {
        EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem.AddListener(EventType.RESET_GAME, ResetGame);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
        EventSystem.AddListener(EventType.PICK_TILE, PickStartingTile);
        EventSystem<GameObject>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
    }

    private void OnDisable()
    {
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
        EventSystem.RemoveListener(EventType.RESET_GAME, ResetGame);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
        EventSystem.RemoveListener(EventType.PICK_TILE, PickStartingTile);
        EventSystem<GameObject>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
    }

    public void CreateGrid(int _gridSize, int _bombAmount)
    {
        gridSize = _gridSize;
        bombAmount = _bombAmount;
        bombs = _bombAmount;
        // magical numbers to fix set 3D grid with camera orientation
        target.transform.position = new Vector3(0, -9 + gridSize * 2, 1 - gridSize / 10f);
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
        EventSystem.InvokeEvent(EventType.PREPARE_GAME);
        yield return new WaitForEndOfFrame();
    }

    // add flag to pool
    private void AddNewFlag()
    {
        GameObject _flag = Instantiate(flag, Vector3.up * 5000, Quaternion.identity);
        _flag.transform.localScale = Vector3.one;
        inactiveFlags.Add(_flag);
    }

    // activate a flag and place it above the tile
    private void ActivateFlag(Vector3[] vectors)
    {
        if (inactiveFlags.Count > 0 && bombs > 0)
        {
            inactiveFlags[0].transform.position = vectors[0];
            inactiveFlags[0].transform.eulerAngles = vectors[1];
            inactiveFlags[0].transform.parent = parentTile.transform;
            activeFlags.Add(inactiveFlags[0]);
            inactiveFlags.RemoveAt(0);
        }
    }

    // remove a flag from the tile
    public void ReturnFlag(GameObject flag)
    {
        flag.transform.position = Vector3.up * 5000;
        flag.transform.parent = null;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
    }

    private void AddEmptyTile(GameObject gameobject)
    {
        emptyTiles.Add(gameobject);
    }

    private void PickStartingTile()
    {
        if (emptyTiles.Count > 0)
        {
            firstTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
            firstTile.GetComponent<Tile3D>().FirstTile();
        }
    }

    private void ResetGame()
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

    private void AddFlag(Vector3[] vectors)
    {
        bombs--;
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombs);
    }

    private void RemoveFlag(object value)
    {
        bombs++;
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombs);
    }
}
