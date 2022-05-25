using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseGridManager : MonoBehaviour
{
    protected List<GameObject> tiles = new List<GameObject>();
    protected List<GameObject> activeFlags = new List<GameObject>();
    protected List<GameObject> inactiveFlags = new List<GameObject>();

    protected GameObject firstTile;
    protected List<GameObject> emptyTiles = new List<GameObject>();
    protected int bombAmount;
    protected int initialBombAmount;
    protected int goodTiles = 0;

    [Header("Settings")]
    [Tooltip("1:X where 1 is the bomb and X is the amount of non bomb tiles")]
    [SerializeField] protected int bombDensity = 6;
    [SerializeField] protected GameObject flagParent;

    protected bool timeStarted;
    protected bool inReset;
    protected bool wonGame;
    protected bool usedFlag;
    protected bool loseGame;
    protected int tileClicks;
    protected float timer;
    public float progress;

    protected DataSerializer DS;
    public GameUI uiManager;

    public TextMeshProUGUI infoText;

    [Header("Level specific")]
    [Tooltip("1 = arctic, 2 = asia, 3 = desert, 4 = tutorial")]
    public int area;
    public int level;
    public Color emptyTileColor = Color.black;
    public Color startColor = Color.blue;
    public Color defaultColor = Color.grey;
    public Color selectColor = Color.green;

    protected string difficultyStars;
    protected bool firstTime = true; // used to avoid bug, clean solution needs to be fixed!

    protected abstract void Start();

    protected virtual void Update()
    {
        if (timeStarted) timer += Time.deltaTime;
        EventSystem<float>.InvokeEvent(EventType.UPDATE_TIME, timer);
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    protected virtual IEnumerator RandomizeGrid()
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
                newTile.tag = "Bomb";
                newTile.layer = 11;
                newTile.GetComponent<BaseTile>().state = TileStates.Bomb;
                bombCount++;
            }
            else
            {
                newTile.tag = "Empty";
                newTile.layer = 12;
                newTile.GetComponent<BaseTile>().state = TileStates.Empty;
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

        EventSystem.InvokeEvent(EventType.PREPARE_GAME);
        yield return new WaitForEndOfFrame();
        StartGame();
        yield return new WaitForEndOfFrame();
    }

    // activate a flag and place it above the tile
    protected virtual void ActivateFlag(Vector3[] vectors)
    {
        if (inactiveFlags.Count > 0 && bombAmount > 0)
        {
            inactiveFlags[0].transform.position = vectors[0];
            inactiveFlags[0].transform.eulerAngles = vectors[1];
            activeFlags.Add(inactiveFlags[0]);
            inactiveFlags.RemoveAt(0);
            AddFlag();
        }
    }

    // remove a flag from the tile
    public virtual void ReturnFlag(GameObject flag)
    {
        flag.transform.position = Vector3.up * 5000;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
        RemoveFlag();
    }

    protected virtual void AddFlag()
    {
        bombAmount--;
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
    }

    protected virtual void RemoveFlag()
    {
        bombAmount++;
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
    }

    protected virtual void LoseGame()
    {
        loseGame = true;
    }

    protected virtual void AddEmptyTile(GameObject gameobject)
    {
        emptyTiles.Add(gameobject);
    }

    protected virtual void PickStartingTile()
    {
        if (emptyTiles.Count > 0)
        {
            firstTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
            firstTile.GetComponent<BaseTile>().FirstTile();
        }
    }

    protected virtual void ResetGame()
    {
        if (inReset)
        {
            return;
        }
        else
        {
            inReset = true;
            firstTile = null;
            timeStarted = false;
            usedFlag = false;
            goodTiles = 0;
            timer = 0;
            tileClicks = 0;
            bombAmount = tiles.Count / bombDensity;
            initialBombAmount = bombAmount;
            emptyTiles = new List<GameObject>();
            StartCoroutine(ResetLogic());
        }
    }

    IEnumerator ResetLogic()
    {
        EventSystem.InvokeEvent(EventType.END_GAME);
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

    protected virtual void StartGame()
    {
        EventSystem.InvokeEvent(EventType.COUNT_BOMBS);
        PickStartingTile();
        EventSystem.InvokeEvent(EventType.START_GAME);
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
        inReset = false;
        loseGame = false;
    }

    protected virtual void AddGoodTile(GameObject tile)
    {
        if (!timeStarted)
        {
            timeStarted = true;
        }
        goodTiles++;
        CheckForVictory();
    }

    protected virtual void CheckForVictory()
    {
        progress = goodTiles / (tiles.Count - initialBombAmount);
        if (goodTiles == (tiles.Count - initialBombAmount))
        {
            wonGame = true;
            EventSystem.InvokeEvent(EventType.WIN_GAME);
        }
    }

    protected virtual void TileClick()
    {
        tileClicks++;
    }

    protected virtual void TileClick(Vector3[] vectors)
    {
        tileClicks++;
    }

    protected virtual void FlagClick(GameObject flag)
    {
        tileClicks++;
        usedFlag = true;
    }

    protected virtual void StopTimer()
    {
        if (firstTime || tileClicks == 0)
        {
            firstTime = false;
            return;
        }
        timeStarted = false;

        SaveData();
    }

    protected abstract void SaveData();

    protected abstract void SetText(AccountData data = null);
}
