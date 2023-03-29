using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager3D : BaseGridManager
{
    SteamAPIManager steamAPI;

    private int difficulty;

    private bool saving;

    public GameObject[] layers;

    private int startBombAmount = 12;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 3;

        Helpers.NestedChildToGob<Tile3D>(transform, tiles);
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

        DS = gameObject.GetComponent<DataSerializer>();
        SetText();
    }

    protected override void OnEnable()
    {
        EventSystem.eventCollectionParam[EventType.PLANT_FLAG] += ActivateFlag;
        EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE] += AddGoodTile;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] += ReturnFlag;
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
        EventSystem.eventCollectionParam[EventType.PLANT_FLAG] -= ActivateFlag;
        EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE] -= AddGoodTile;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] -= ReturnFlag;
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

    private void AddLayer()
    {
        EventSystem.eventCollection[EventType.ADD_LAYER]();
        bombAmount += 3;
        EventSystem.eventCollectionParam[EventType.BOMB_UPDATE](bombAmount);
    }

    // activate a flag and place it above the tile
    protected override void ActivateFlag(object value)
    {
        Vector3[] vectors = value as Vector3[];
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
    public override void ReturnFlag(object value)
    {
        GameObject flag = value as GameObject;
        flag.transform.position = Vector3.up * 5000;
        flag.transform.parent = null;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
        RemoveFlag();
    }

    // place 3 bombs per layer
    protected override IEnumerator RandomizeGrid()
    {
        int curTile = 0;
        int spawnChance = 0;
        int bombCount = 0;
        int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
        int curTileCount = 0; // used for optimalization
        bombAmount = startBombAmount;

        foreach (GameObject layer in layers)
        {
            for (int i = 0; i < layer.transform.childCount; i++)
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    // 5 = amount of layers
                    spawnChance = (layer.transform.childCount * 5 - curTile) / (bombAmount - bombCount);
                }

                GameObject newTile = layer.transform.GetChild(i).transform.gameObject;
                
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

                    // Less than 26 neighbours means that it is a tile on the edge
                    Collider[] hits = Physics.OverlapBox(newTile.transform.position, Vector3.one * 0.75f);
                    if (hits.Length < 26)
                    {
                        emptyTiles.Add(newTile);
                    }
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

        // Build new list based on fully empty tiles, select random as start, or use random from first list of no true empty
        List<GameObject> trueEmpty = new List<GameObject>();
        foreach(GameObject tile in emptyTiles)
        {
            Collider[] hits = Physics.OverlapBox(tile.transform.position, Vector3.one * 0.75f);
            bool hadBomb = false;
            foreach (Collider col in hits)
            {
                if (col.gameObject.layer == 11)
                {
                    hadBomb = true;
                }
            }
            if (!hadBomb)
            {
                trueEmpty.Add(tile);
            }

            yield return new WaitForEndOfFrame();
        }
        if (trueEmpty.Count > 0)
        {
            emptyTiles = trueEmpty;
        }

        yield return new WaitForEndOfFrame();

        EventSystem.eventCollection[EventType.PREPARE_GAME]();
        yield return new WaitForEndOfFrame();
        StartGame();
        yield return new WaitForEndOfFrame();
    }

    protected override void CheckForVictory()
    {
        progress = goodTiles / (tiles.Count - startBombAmount);
        if (goodTiles == (tiles.Count - startBombAmount))
        {
            wonGame = true;
            EventSystem.eventCollection[EventType.WIN_GAME]();
        }
    }

    protected override void LoseGame()
    {
        loseGame = true;

        if (tileClicks <= 1) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);
    }

    protected override void SaveData()
    {
        if (saving) return;
        saving = true;

        float efficiency = 1f * tileClicks / (tileClicks + otherClicks) * 100f;
        efficiency = Mathf.Clamp(efficiency, 0, 100);
        uiManager.SetEfficiency(efficiency);

        float timer = Helpers.RoundToThreeDecimals(this.timer);
        AccountData AD = DS.GetUserData();
        AD.totalClicks = AD.totalClicks + tileClicks;
        AD.gamesPlayed = AD.gamesPlayed + 1;
        AD.totalTimePlayed = AD.totalTimePlayed + timer;
        AD.galaxyTotalClicks = AD.galaxyTotalClicks + tileClicks;
        AD.galaxyGamesPlayed = AD.galaxyGamesPlayed + 1;

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

            AD.galaxyVictories = AD.galaxyVictories + 1;
            AD.galaxyVictories1 = AD.galaxyVictories1 + 1;
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

        steamAPI.SetStatInt(UserStats.galaxy1GamesPlayed, AD.galaxyGamesPlayed);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.galaxyGamesPlayed, AD.galaxyGamesPlayed);

        DS.UpdateAccountData(AD);

        wonGame = false;
        saving = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (data == null) data = DS.GetUserData();

        stars.text = "" + difficulty;
        infoText.text =
            "Time: " + Helpers.RoundToThreeDecimals(data.galaxyTime1) + "s\n" +
            "Skill: " + Helpers.RoundToThreeDecimals(data.galaxyEfficiency1) + "%\n" +
            "Victories: " + data.galaxyVictories1 + "\n";
    }
}
