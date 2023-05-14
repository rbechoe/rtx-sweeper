using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager2DGarden : BaseGridManager
{
    SteamAPIManager steamAPI;

    private int difficulty = 8;
    private int reduction = 0;

    public Slider difficultySlider;
    public bool inEditMode = true;

    public Color playableColor = Color.green;
    public Color unPlayableColor = Color.red;
    public GameObject flagPrefab;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        Helpers.NestedChildToGob<Tile2DGarden>(transform, tiles);
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

        DS = gameObject.GetComponent<DataSerializer>();
    }

    protected override void OnEnable()
    {
        EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE] += AddGoodTile;
        EventSystem.eventCollectionParam[EventType.PLANT_FLAG] += ActivateFlag;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] += ReturnFlag;
        EventSystem.eventCollectionParam[EventType.ADD_EMPTY] += AddEmptyTile;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] += FlagClick;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += ResetGame;
        EventSystem.eventCollection[EventType.WIN_GAME] += StopTimer;
        EventSystem.eventCollection[EventType.END_GAME] += StopTimer;
        EventSystem.eventCollection[EventType.GAME_LOSE] += LoseGame;
        EventSystem.eventCollection[EventType.GAME_LOSE] += StopTimer;
        EventSystem.eventCollection[EventType.REVEAL_TILE] += TileClick;
        EventSystem.eventCollection[EventType.OTHER_CLICK] += OtherClick;
        EventSystem.eventCollection[EventType.PLAY_FLAG] += PlantFlag;
    }

    protected override void OnDisable()
    {
        EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE] -= AddGoodTile;
        EventSystem.eventCollectionParam[EventType.PLANT_FLAG] -= ActivateFlag;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] -= ReturnFlag;
        EventSystem.eventCollectionParam[EventType.ADD_EMPTY] -= AddEmptyTile;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] -= FlagClick;
        EventSystem.eventCollection[EventType.RANDOM_GRID] -= ResetGame;
        EventSystem.eventCollection[EventType.WIN_GAME] -= StopTimer;
        EventSystem.eventCollection[EventType.END_GAME] -= StopTimer;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= LoseGame;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= StopTimer;
        EventSystem.eventCollection[EventType.REVEAL_TILE] -= TileClick;
        EventSystem.eventCollection[EventType.OTHER_CLICK] -= OtherClick;
        EventSystem.eventCollection[EventType.PLAY_FLAG] -= PlantFlag;
    }

    protected override void LoseGame()
    {
        loseGame = true;

        if (tileClicks <= 1) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);
    }

    protected override void SaveData()
    {
        if (area == 4) return;

        float efficiency = 1f * tileClicks / (tileClicks + otherClicks) * 100f;
        efficiency = Mathf.Clamp(efficiency, 0, 100);
        uiManager.SetEfficiency(efficiency);

        AccountData AD = DS.GetUserData();
        AD.totalClicks = AD.totalClicks + tileClicks;
        AD.gamesPlayed = AD.gamesPlayed + 1;
        float timer = Helpers.RoundToThreeDecimals(this.timer);
        AD.totalTimePlayed = AD.totalTimePlayed + timer;

        steamAPI.SetStatInt(UserStats.totalGamesPlayed, AD.gamesPlayed);
        steamAPI.SetStatInt(UserStats.totalClicks, AD.totalClicks);

        steamAPI.UpdateLeaderBoard(LeaderboardStats.clicks, AD.totalClicks);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesPlayed, AD.gamesPlayed);

        if (wonGame)
        {
            AD.gamesWon = AD.gamesWon + 1;

            if (timer < 10) steamAPI.SetAchievement(UserAchievements.speedrunPro);
            if (timer < 20) steamAPI.SetAchievement(UserAchievements.speedrun);

            steamAPI.SetStatInt(UserStats.totalGamesWon, AD.gamesWon);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.noFlags);
            if (!usedFlag && difficulty >= 5) steamAPI.SetAchievement(UserAchievements.noFlagsPlus);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesWon, AD.gamesWon);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.timePlayed, (int)(AD.totalTimePlayed / 60f));
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }

        DS.UpdateAccountData(AD);
            
        wonGame = false;
    }

    protected override IEnumerator RandomizeGrid()
    {
        int curTile = 0;
        int spawnChance = 0;
        int bombCount = 0;
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;
        reduction = 0;

        // remove flags
        foreach (GameObject flag in activeFlags)
        {
            Destroy(flag);
        }
        foreach (GameObject flag in inactiveFlags)
        {
            Destroy(flag);
        }
        activeFlags.Clear();
        inactiveFlags.Clear();

        for (int tileId = 0; tileId < tiles.Count; tileId++)
        {
            GameObject newTile = tiles[tileId];
            Tile2DGarden tileData = newTile.GetComponent<Tile2DGarden>();
            if (tileData.unplayable)
            {
                newTile.tag = "Untagged";
                newTile.layer = 0;
                tileData.state = TileStates.Empty;
                newTile.name = "Unplayable " + curTile;
                reduction++;
            }
            else
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    spawnChance = (tiles.Count - curTile) / (bombAmount - bombCount);
                }

                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.tag = "Bomb";
                    newTile.layer = 11;
                    tileData.state = TileStates.Bomb;
                    bombCount++;
                    AddNewFlag();
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
        }
        yield return new WaitForEndOfFrame();

        EventSystem.eventCollection[EventType.PREPARE_GAME]();
        yield return new WaitForEndOfFrame();
        StartGame();
        yield return new WaitForEndOfFrame();
    }

    // add flag to pool
    private void AddNewFlag()
    {
        GameObject _flag = Instantiate(flagPrefab, Vector3.up * 5000, Quaternion.identity);
        _flag.transform.parent = flagParent.transform;
        inactiveFlags.Add(_flag);
    }

    protected override void CheckForVictory()
    {
        progress = (float)goodTiles / (tiles.Count - reduction - initialBombAmount);
        if (goodTiles == (tiles.Count - reduction - initialBombAmount))
        {
            wonGame = true;
            EventSystem.eventCollection[EventType.WIN_GAME]();
        }
    }

    protected override void ResetGame()
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
            bombAmount = tiles.Count / difficulty;
            initialBombAmount = bombAmount;
            emptyTiles = new List<GameObject>();
            StartCoroutine(ResetLogic());
        }
    }

    protected override void SetText(AccountData data = null)
    {
        // not used
    }

    protected override void AddEmptyTile(object value)
    {
        GameObject gameobject = value as GameObject;
        if (gameobject.GetComponent<Tile2DGarden>().unplayable)
        {
            return;
        }

        emptyTiles.Add(gameobject);
    }

    public void UpdateDifficulty()
    {
        difficulty = 13 - (int)difficultySlider.value;
    }

    public void EnterEditMode()
    {
        inEditMode = true;
    }

    public void ExitEditMode()
    {
        inEditMode = false;
    }
}
