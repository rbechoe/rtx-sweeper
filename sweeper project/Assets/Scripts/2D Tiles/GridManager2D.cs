using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager2D : Base
{
    public List<GameObject> tiles = new List<GameObject>();
    public List<GameObject> activeFlags = new List<GameObject>();
    public List<GameObject> inactiveFlags = new List<GameObject>();

    private GameObject firstTile;
    private List<GameObject> emptyTiles = new List<GameObject>();
    private int bombAmount;
    private int goodTiles = 0;

    [Header("Settings")]
    [Tooltip("1:X where 1 is the bomb and X is the amount of non bomb tiles")]
    [SerializeField] private int bombDensity = 6;
    [SerializeField] private GameObject flagParent;

    protected override void Start()
    {
        base.Start();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Checker>())
            {
                tiles.Add(child.gameObject);
            }
        }

        foreach (Transform child in flagParent.transform)
        {
            if (child.GetComponent<Flag>())
            {
                inactiveFlags.Add(child.gameObject);
            }
        }

        EventSystem<Parameters>.InvokeEvent(EventType.RANDOM_GRID, new Parameters());
    }

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem<Parameters>.AddListener(EventType.ADD_GOOD_TILE, AddGoodTile);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem<Parameters>.RemoveListener(EventType.ADD_GOOD_TILE, AddGoodTile);
    }

    private IEnumerator RandomizeGrid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;
        int bombCount = 0;
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;

        for (int tileId = 0; tileId < tiles.Count; tileId++)
        {
            // formula: based on tiles and bombs left increase chance for next tile to be bomb
            if (bombCount < bombAmount)
            {
                tilesLeft = tiles.Count - curTile;
                spawnChance = tilesLeft / (bombAmount - bombCount);
            }

            GameObject newTile = tiles[tileId];
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
            curTileCount++;
            newTile.name = "tile " + curTile;

            // continue next frame
            if (curTileCount >= tilesPerFrame)
            {
                curTileCount = 0;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();

        EventSystem<Parameters>.InvokeEvent(EventType.PREPARE_GAME, new Parameters());
        StartGame();
        yield return new WaitForEndOfFrame();
    }

    // activate a flag and place it above the tile
    private void ActivateFlag(Parameters param)
    {
        if (inactiveFlags.Count > 0 && bombAmount > 0)
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

    private void PickStartingTile()
    {
        if (emptyTiles.Count > 0)
        {
            firstTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
            firstTile.GetComponent<Tile>().FirstTile();
        }
    }

    private void ResetGame(object value)
    {
        firstTile = null;
        StartCoroutine(ResetLogic());
    }
    
    IEnumerator ResetLogic()
    {
        goodTiles = 0;
        bombAmount = tiles.Count / bombDensity;
        emptyTiles = new List<GameObject>();
        EventSystem<Parameters>.InvokeEvent(EventType.END_GAME, new Parameters());

        // remove all bomb and empty components
        foreach (GameObject tile in tiles)
        {
            // TODO can be improved!!
            if (tile.GetComponent<Empty>())
            {
                Destroy(tile.GetComponent<Empty>());
            }
            if (tile.GetComponent<Bomb>())
            {
                Destroy(tile.GetComponent<Bomb>());
            }
        }
        yield return new WaitForEndOfFrame();

        // move all active flags to inactive
        foreach (GameObject flag in activeFlags)
        {
            flag.transform.parent = flagParent.transform;
            flag.transform.localPosition = Vector3.zero;
            inactiveFlags.Add(flag);
        }
        activeFlags = new List<GameObject>();
        yield return new WaitForEndOfFrame();

        // reset the grid
        StartCoroutine(RandomizeGrid());
    }

    private void AddFlag(object value)
    {
        bombAmount--;
        Parameters param = new Parameters();
        param.integers.Add(bombAmount);
        EventSystem<Parameters>.InvokeEvent(EventType.BOMB_UPDATE, param);
    }

    private void RemoveFlag(object value)
    {
        bombAmount++;
        Parameters param = new Parameters();
        param.integers.Add(bombAmount);
        EventSystem<Parameters>.InvokeEvent(EventType.BOMB_UPDATE, param);
    }

    private void StartGame()
    {
        Parameters param = new Parameters();
        param.integers.Add(bombAmount);
        EventSystem<Parameters>.InvokeEvent(EventType.COUNT_BOMBS, new Parameters());
        PickStartingTile();
        EventSystem<Parameters>.InvokeEvent(EventType.START_GAME, new Parameters());
        EventSystem<Parameters>.InvokeEvent(EventType.BOMB_UPDATE, param); // TODO fix
    }

    private void AddGoodTile(object value)
    {
        goodTiles++;
        CheckForVictory();
    }

    private void CheckForVictory()
    {
        if (goodTiles == tiles.Count - bombAmount)
        {
            //TODO: EndGame();
            EventSystem<Parameters>.InvokeEvent(EventType.WIN_GAME, new Parameters());
        }
    }
}
