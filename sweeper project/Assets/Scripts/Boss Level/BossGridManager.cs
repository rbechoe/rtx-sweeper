using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGridManager : BaseGridManager
{
    public List<bool> checks = new List<bool>(); // used to check if each tile has performed required logic
    public int busyTiles = 0; // when not zero tiles are busy getting revealed

    private int difficulty;

    private LayerMask flagMask;

    private bool gameActive;
    private bool canShuffle;

    SteamAPIManager steamAPI;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        flagMask = LayerMask.GetMask("Flag");
        canShuffle = true;

        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BaseTile>())
            {
                tiles.Add(child.gameObject);
                child.GetComponent<BossTile>().myId = count;
                checks.Add(false);
                count++;
            }
        }

        foreach (Transform child in flagParent.transform)
        {
            if (child.GetComponent<BaseFlag>())
            {
                inactiveFlags.Add(child.gameObject);
            }
        }

        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 3; // base +3 due to boss stage

        DS = gameObject.GetComponent<DataSerializer>();
        SetText();
    }

    protected override void OnEnable()
    {
        EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE] += AddGoodTile;
        EventSystem.eventCollectionParam[EventType.PLANT_FLAG] += ActivateFlag;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] += ReturnFlag;
        EventSystem.eventCollectionParam[EventType.ADD_EMPTY] += AddEmptyTile;
        EventSystem.eventCollectionParam[EventType.REMOVE_FLAG] += FlagClick;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += ResetGame;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += GameActive;
        EventSystem.eventCollection[EventType.WIN_GAME] += StopTimer;
        EventSystem.eventCollection[EventType.END_GAME] += StopTimer;
        EventSystem.eventCollection[EventType.END_GAME] += GameInactive;
        EventSystem.eventCollection[EventType.GAME_LOSE] += GameInactive;
        EventSystem.eventCollection[EventType.GAME_LOSE] += StopTimer;
        EventSystem.eventCollection[EventType.REVEAL_TILE] += TileClick;
        EventSystem.eventCollection[EventType.GAME_LOSE] += LoseGame;
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
        EventSystem.eventCollection[EventType.RANDOM_GRID] -= GameActive;
        EventSystem.eventCollection[EventType.WIN_GAME] -= StopTimer;
        EventSystem.eventCollection[EventType.END_GAME] -= StopTimer;
        EventSystem.eventCollection[EventType.END_GAME] -= GameInactive;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= GameInactive;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= StopTimer;
        EventSystem.eventCollection[EventType.REVEAL_TILE] -= TileClick;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= LoseGame;
        EventSystem.eventCollection[EventType.OTHER_CLICK] -= OtherClick;
        EventSystem.eventCollection[EventType.PLAY_FLAG] -= PlantFlag;
    }

    public void ShuffleGrid()
    {
        if (!gameActive) return;

        if (canShuffle && busyTiles == 0)
        {
            shuffleCount++;
            if (shuffleCount < 2) return;
            StartCoroutine(ShuffleBombs());
        }
    }
        
    private IEnumerator ShuffleBombs()
    {
        // Step 0: can not shuffle
        canShuffle = false;
        ResetChecks();
        yield return new WaitForEndOfFrame();

        // Step 1: mark all tiles as unplayable
        EventSystem.eventCollection[EventType.UNPLAYABLE]();
        while (!AllChecked())
        {
            yield return new WaitForEndOfFrame();
        }

        // Step 2: shuffle all positions
        List<GameObject> shuffles = new List<GameObject>();
        List<Vector3> positions = new List<Vector3>();
        for (int tileId = 0; tileId < tiles.Count; tileId++)
        {
            GameObject newTile = tiles[tileId];
            BaseTile tileData = newTile.GetComponent<BaseTile>();

            // if flagged skip
            Collider[] nearbyFlags = Physics.OverlapBox(newTile.transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
            if (nearbyFlags.Length > 0) continue;

            // if triggered skip
            if (tileData.triggered) continue;

            shuffles.Add(newTile);
            positions.Add(newTile.transform.position);
        }
        yield return new WaitForEndOfFrame();
            
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 temp = positions[i];
            int randomIndex = Random.Range(i, positions.Count);
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }
        yield return new WaitForEndOfFrame();

        // Step 3: assign new positions
        for (int i = 0; i < shuffles.Count; i++)
        {
            shuffles[i].transform.position = positions[i];
        }
        ResetChecks();
        yield return new WaitForEndOfFrame();

        // Step 4: mark all tiles as playable
        EventSystem.eventCollection[EventType.PLAYABLE]();
        while (!AllChecked())
        {
            yield return new WaitForEndOfFrame();
        }

        // Step 5: once all tiles are playable can shuffle again
        yield return new WaitForEndOfFrame();
        ResetChecks();
        canShuffle = true;
    }

    private void ResetChecks()
    {
        for (int i = 0; i < checks.Count; i++)
        {
            checks[i] = false;
        }
    }

    private bool AllChecked()
    {
        for (int i = 0; i < checks.Count; i++)
        {
            if (!checks[i]) return false;
        }

        return true;
    }

    protected override void LoseGame()
    {
        loseGame = true;

        if (tileClicks <= 1) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);
    }

    protected override void SaveData()
    {
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

        if (tileClicks <= 1 && loseGame) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);

        if (wonGame)
        {
            AD.gamesWon = AD.gamesWon + 1;

            if (timer < 10) steamAPI.SetAchievement(UserAchievements.speedrunPro);
            if (timer < 20) steamAPI.SetAchievement(UserAchievements.speedrun);

            steamAPI.SetStatInt(UserStats.totalGamesWon, AD.gamesWon);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.kris);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.noFlags);
            if (!usedFlag && difficulty >= 5) steamAPI.SetAchievement(UserAchievements.noFlagsPlus);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesWon, AD.gamesWon);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.timePlayed, (int)(AD.totalTimePlayed / 60f));
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }
            
        AD.bossVictories = (wonGame) ? AD.bossVictories + 1 : AD.bossVictories;
        AD.bossTotalClicks += tileClicks;
        AD.bossGamesPlayed += 1;

        // used for future purposes when more bosses will be added
        if (wonGame)
        {
            AD.bossVictories1 += 1;

            if (timer < AD.bossTime1 || (timer == AD.bossTime1 && efficiency > AD.bossEfficiency1) || AD.bossTime1 == 0)
            {
                AD.bossTime1 = timer;
                AD.bossEfficiency1 = efficiency;
                AD.bossClicks1 = tileClicks;

                steamAPI.SetStatFloat(UserStats.islands1BestTime, timer);
            }
            steamAPI.SetStatInt(UserStats.islands1Victories, AD.bossVictories1);
            steamAPI.SetStatInt(UserStats.islandsGamesWon, AD.bossVictories);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.islands1BestTime, (int)(AD.bossTime1 * 1000));
            steamAPI.UpdateLeaderBoard(LeaderboardStats.islandsGamesWon, AD.bossVictories);
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }
        steamAPI.SetStatInt(UserStats.islandsGamesPlayed, 1);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.islandsGamesPlayed, AD.bossGamesPlayed);

        DS.UpdateAccountData(AD);
        SetText();
            
        wonGame = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (data == null) data = DS.GetUserData();

        stars.text = "" + difficulty;
        infoText.text = 
            "Time: " + Helpers.RoundToThreeDecimals(data.bossTime1) + "s\n" +
            "Skill: " + Helpers.RoundToThreeDecimals(data.bossEfficiency1) + "%\n" +
            "Victories: " + data.bossVictories1;
    }

    private void GameActive()
    {
        gameActive = true;
    }

    private void GameInactive()
    {
        gameActive = false;
    }
}
