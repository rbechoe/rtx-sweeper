using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    protected int otherClicks;
    protected int shuffleCount;
    protected float timer;
    public float progress;
    public bool gridActive = true;

    protected DataSerializer DS;
    public GameUI uiManager;

    public Text infoText, stars;

    [Header("Level specific")]
    [Tooltip("1 = arctic, 2 = asia, 3 = desert, 4 = tutorial")]
    public int area;
    public int level;
    public Color emptyTileColor = Color.black;
    public Color startColor = Color.blue;
    public Color defaultColor = Color.grey;
    public Color selectColor = Color.green;

    protected bool firstTime = true; // used to avoid bug, clean solution needs to be fixed!

    protected abstract void Start();

    protected virtual void Update()
    {
        if (timeStarted) timer += Time.deltaTime;
        EventSystem.eventCollectionParam[EventType.UPDATE_TIME](timer);
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    protected virtual IEnumerator RandomizeGrid()
    {
        int curTile = 0;
        int spawnChance = 0;
        int bombCount = 0;
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;

        for (int tileId = 0; tileId < tiles.Count; tileId++)
        {
            // formula: based on tiles and bombs left increase chance for next tile to be bomb
            if (bombCount < bombAmount)
            {
                spawnChance = (tiles.Count - curTile) / (bombAmount - bombCount);
            }

            GameObject newTile = tiles[tileId];
            BaseTile tileData = newTile.GetComponent<BaseTile>();
            if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
            {
                newTile.tag = "Bomb";
                newTile.layer = 11;
                tileData.state = TileStates.Bomb;
                bombCount++;
            }
            else
            {
                newTile.tag = "Empty";
                newTile.layer = 12;
                tileData.state = TileStates.Empty;
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

        EventSystem.eventCollection[EventType.PREPARE_GAME]();
        yield return new WaitForEndOfFrame();
        StartGame();
        yield return new WaitForEndOfFrame();
    }

    // activate a flag and place it above the tile
    protected virtual void ActivateFlag(object value)
    {
        if (!gridActive)
        {
            return;
        }

        Vector3[] vectors = value as Vector3[];
        if (inactiveFlags.Count > 0 && bombAmount > 0)
        {
            inactiveFlags[0].transform.position = vectors[0];
            inactiveFlags[0].transform.eulerAngles = vectors[1];
            inactiveFlags[0].transform.parent = transform;
            activeFlags.Add(inactiveFlags[0]);
            inactiveFlags.RemoveAt(0);
            AddFlag();
        }
    }

    // remove a flag from the tile
    public virtual void ReturnFlag(object value)
    {
        GameObject flag = value as GameObject;
        flag.transform.position = Vector3.up * 5000;
        flag.transform.parent = flagParent.transform;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
        RemoveFlag();
    }

    protected virtual void AddFlag()
    {
        bombAmount--;
        EventSystem.eventCollectionParam[EventType.BOMB_UPDATE](bombAmount);
    }

    protected virtual void RemoveFlag()
    {
        bombAmount++;
        EventSystem.eventCollectionParam[EventType.BOMB_UPDATE](bombAmount);
    }

    protected virtual void LoseGame()
    {
        loseGame = true;
    }

    protected virtual void AddEmptyTile(object value)
    {
        emptyTiles.Add(value as GameObject);
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
            otherClicks = 0;
            shuffleCount = 0;
            bombAmount = tiles.Count / bombDensity;
            initialBombAmount = bombAmount;
            emptyTiles = new List<GameObject>();
            StartCoroutine(ResetLogic());
        }
    }

    protected IEnumerator ResetLogic()
    {
        EventSystem.eventCollection[EventType.END_GAME]();
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
        EventSystem.eventCollection[EventType.COUNT_BOMBS]();
        PickStartingTile();
        EventSystem.eventCollection[EventType.START_GAME]();
        EventSystem.eventCollectionParam[EventType.BOMB_UPDATE](bombAmount);
        inReset = false;
        loseGame = false;
    }

    protected virtual void AddGoodTile(object value)
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
            EventSystem.eventCollection[EventType.WIN_GAME]();
        }
    }

    protected virtual void TileClick()
    {
        tileClicks++;
    }

    protected virtual void FlagClick(object value)
    {
        otherClicks++;
    }

    protected virtual void PlantFlag()
    {
        usedFlag = true;
    }

    protected virtual void OtherClick()
    {
        otherClicks++;
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
