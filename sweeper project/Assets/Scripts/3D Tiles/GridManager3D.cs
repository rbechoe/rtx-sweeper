using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager3D : BaseGridManager
{
    SteamAPIManager steamAPI;

    private int difficulty;

    private bool saving;
    private int layerClick;

    public GameObject[] layers;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 3;

        Helpers.NestedChildToGob<Tile3D>(transform, tiles);
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

        difficultyStars = "Difficulty: "; 
        for (int i = 0; i < difficulty; i++)
        {
            difficultyStars += "*";
        }

        DS = gameObject.GetComponent<DataSerializer>();
        SetText();
    }

    protected override void OnEnable()
    {
        EventSystem<GameObject>.AddListener(EventType.ADD_GOOD_TILE, AddGoodTile);
        EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem.AddListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.AddListener(EventType.WIN_GAME, StopTimer);
        EventSystem.AddListener(EventType.END_GAME, StopTimer);
        EventSystem.AddListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.AddListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.AddListener(EventType.PLAY_CLICK, LayerUnlock);
        EventSystem.AddListener(EventType.REVEAL_TILE, TileClick);
        EventSystem.AddListener(EventType.OTHER_CLICK, OtherClick);
        EventSystem.AddListener(EventType.PLAY_FLAG, PlantFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, FlagClick);
    }

    protected override void OnDisable()
    {
        EventSystem<GameObject>.RemoveListener(EventType.ADD_GOOD_TILE, AddGoodTile);
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.RemoveListener(EventType.WIN_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.END_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.RemoveListener(EventType.PLAY_CLICK, LayerUnlock);
        EventSystem.RemoveListener(EventType.REVEAL_TILE, TileClick);
        EventSystem.RemoveListener(EventType.OTHER_CLICK, OtherClick);
        EventSystem.RemoveListener(EventType.PLAY_FLAG, PlantFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
    }

    // unlock a layer
    private void LayerUnlock()
    {
        layerClick++;
        if (layerClick >= 2 && !layers[1].activeSelf)
        {
            layers[1].SetActive(true);
            AddLayer();
        }
        if (layerClick >= 4 && !layers[2].activeSelf)
        {
            layers[2].SetActive(true);
            AddLayer();
        }
        if (layerClick >= 6 && !layers[3].activeSelf)
        {
            layers[3].SetActive(true);
            AddLayer();
        }
        if (layerClick >= 8 && !layers[4].activeSelf)
        {
            layers[4].SetActive(true);
            AddLayer();
        }
    }

    private void AddLayer()
    {
        EventSystem.InvokeEvent(EventType.ADD_LAYER);
        bombAmount += 3;
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
    }

    // activate a flag and place it above the tile
    protected override void ActivateFlag(Vector3[] vectors)
    {
        if (inactiveFlags.Count > 0 && bombAmount > 0)
        {
            inactiveFlags[0].transform.position = vectors[0];
            inactiveFlags[0].transform.eulerAngles = Vector3.zero;
            inactiveFlags[0].transform.parent = transform;
            activeFlags.Add(inactiveFlags[0]);
            inactiveFlags.RemoveAt(0);
            AddFlag();
        }
    }

    // remove a flag from the tile
    public override void ReturnFlag(GameObject flag)
    {
        flag.transform.position = Vector3.up * 5000;
        flag.transform.parent = null;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
        RemoveFlag();
    }

    // place 4 bombs per layer
    protected override IEnumerator RandomizeGrid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;
        int bombCount = 0;
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0;
        layerClick = 0;
        bombAmount = 3;

        // TODO fix magical numbers
        layers[1].SetActive(true);
        layers[2].SetActive(true);
        layers[3].SetActive(true);
        layers[4].SetActive(true);

        foreach (GameObject layer in layers)
        {
            curTile = 0;
            tilesLeft = 0;
            spawnChance = 0;
            bombCount = 0;

            for (int i = 0; i < layer.transform.childCount; i++)
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    tilesLeft = layer.transform.childCount - curTile;
                    spawnChance = tilesLeft / (bombAmount - bombCount);
                }

                GameObject newTile = layer.transform.GetChild(i).transform.gameObject;
                if (newTile.GetComponent<StartingMarker>() != null)
                {
                    newTile.tag = "Empty";
                    newTile.layer = 12;
                    newTile.GetComponent<BaseTile>().state = TileStates.Empty;
                    firstTile = newTile;
                }
                else if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
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
        }
        yield return new WaitForEndOfFrame();

        // TODO fix magical numbers
        layers[1].SetActive(false);
        layers[2].SetActive(false);
        layers[3].SetActive(false);
        layers[4].SetActive(false);

        EventSystem.InvokeEvent(EventType.PREPARE_GAME);
        yield return new WaitForEndOfFrame();
        StartGame();
        yield return new WaitForEndOfFrame();
    }

    protected override void CheckForVictory()
    {
        // always 15 bombs total
        progress = goodTiles / (tiles.Count - 15);
        if (goodTiles == (tiles.Count - 15))
        {
            wonGame = true;
            EventSystem.InvokeEvent(EventType.WIN_GAME);
        }
    }

    protected override void SaveData()
    {
        if (saving) return;
        saving = true;

        float efficiency = 1f * tileClicks / (tileClicks + otherClicks) * 100f;
        efficiency = Mathf.Clamp(efficiency, 0, 100);
        uiManager.SetEfficiency(efficiency);

        AccountData AD = DS.GetUserData();
        AD.totalClicks = AD.totalClicks + tileClicks;
        AD.gamesPlayed = AD.gamesPlayed + 1;
        float timer = Helpers.RoundToThreeDecimals(this.timer);
        AD.totalTimePlayed = AD.totalTimePlayed + timer;

        if (timer < 10) steamAPI.SetAchievement(UserAchievements.speedrunPro);
        if (timer < 20) steamAPI.SetAchievement(UserAchievements.speedrun);

        steamAPI.SetStatInt(UserStats.totalGamesPlayed, AD.gamesPlayed);
        steamAPI.SetStatInt(UserStats.totalClicks, AD.totalClicks);

        steamAPI.UpdateLeaderBoard(LeaderboardStats.clicks, AD.totalClicks);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesPlayed, AD.gamesPlayed);

        if (tileClicks == 1 && loseGame) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);

        if (wonGame)
        {
            AD.gamesWon = AD.gamesWon + 1;

            steamAPI.SetStatInt(UserStats.totalGamesWon, AD.gamesWon);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.noFlags);
            if (!usedFlag && difficulty >= 5) steamAPI.SetAchievement(UserAchievements.noFlagsPlus);

            AD.galaxyVictories += 1;
            if (timer < AD.galaxyTime1 || (timer == AD.galaxyTime1 && efficiency > AD.galaxyEfficiency1) || AD.galaxyTime1 == 0)
            {
                AD.galaxyTime1 = timer;
                AD.galaxyEfficiency1 = efficiency;
                AD.galaxyClicks1 = tileClicks;

                steamAPI.SetStatFloat(UserStats.galaxy1BestTime, timer);
            }
            steamAPI.SetStatInt(UserStats.galaxy1Victories, AD.galaxyVictories1);
            steamAPI.SetStatInt(UserStats.galaxyGamesWon, AD.galaxyVictories);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesWon, AD.gamesWon);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.timePlayed, (int)(AD.totalTimePlayed / 60f));

            steamAPI.UpdateLeaderBoard(LeaderboardStats.galaxy1BestTime, (int)(AD.galaxyTime1 * 1000));
            steamAPI.UpdateLeaderBoard(LeaderboardStats.galaxyGamesWon, AD.galaxyVictories);
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }

        steamAPI.SetStatInt(UserStats.galaxy1GamesPlayed, 1);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.galaxyGamesPlayed, AD.galaxyGamesPlayed);

        DS.UpdateAccountData(AD);

        wonGame = false;
        saving = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (data == null) data = DS.GetUserData();

        infoText.text = difficultyStars + "\n" +
            "Time: " + Helpers.RoundToThreeDecimals(data.galaxyTime1) + "s\n" +
            "Skill: " + Helpers.RoundToThreeDecimals(data.galaxyEfficiency1) + "%\n" +
            "Victories: " + data.galaxyVictories1 + "\n";
    }

    protected override void PickStartingTile()
    {
        firstTile.GetComponent<BaseTile>().FirstTile();
    }
}
