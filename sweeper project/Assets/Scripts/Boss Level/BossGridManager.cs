using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGridManager : BaseGridManager
{
    public List<bool> checks = new List<bool>(); // used to check if each tile has performed required logic
    public int busyTiles = 0; // when not zero tiles are busy getting revealed

    private LayerMask flagMask;
    private LayerMask bombMask;

    private bool gameActive;
    private bool canShuffle;

    SteamAPIManager steamAPI;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        flagMask = LayerMask.GetMask("Flag");
        bombMask = LayerMask.GetMask("Bomb");
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

        difficultyStars = "Difficulty: ***"; // base +3 due to boss stage
        for (int i = 0; i < ((10 - bombDensity) + (tiles.Count / 200)); i++)
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
        EventSystem<GameObject>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
        EventSystem.AddListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.AddListener(EventType.RANDOM_GRID, GameActive);
        EventSystem.AddListener(EventType.WIN_GAME, StopTimer);
        EventSystem.AddListener(EventType.END_GAME, StopTimer);
        EventSystem.AddListener(EventType.END_GAME, GameInactive);
        EventSystem.AddListener(EventType.GAME_LOSE, GameInactive);
        EventSystem.AddListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.AddListener(EventType.TILE_CLICK, TileClick);
        EventSystem.AddListener(EventType.GAME_LOSE, LoseGame);
        EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, TileClick);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, FlagClick);
    }

    protected override void OnDisable()
    {
        EventSystem<GameObject>.RemoveListener(EventType.ADD_GOOD_TILE, AddGoodTile);
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<GameObject>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, GameActive);
        EventSystem.RemoveListener(EventType.WIN_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.END_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.RemoveListener(EventType.TILE_CLICK, TileClick);
        EventSystem.RemoveListener(EventType.END_GAME, GameInactive);
        EventSystem.RemoveListener(EventType.GAME_LOSE, GameInactive);
        EventSystem.RemoveListener(EventType.GAME_LOSE, LoseGame);
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, TileClick);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
    }

    public void ShuffleGrid()
    {
        if (tileClicks <= 1) return;

        if (canShuffle && busyTiles == 0 && gameActive) StartCoroutine(ShuffleBombs());
    }
        
    private IEnumerator ShuffleBombs()
    {
        // Step 0: can not shuffle
        canShuffle = false;
        ResetChecks();
        yield return new WaitForEndOfFrame();

        // Step 1: mark all tiles as unplayable
        EventSystem.InvokeEvent(EventType.UNPLAYABLE);
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
        EventSystem.InvokeEvent(EventType.PLAYABLE);
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

    protected override void SaveData()
    {
        float efficiency = 1f * (tiles.Count - initialBombAmount) / tileClicks * 100f;
        efficiency = Mathf.Clamp(efficiency, 0, 100);
        uiManager.SetEfficiency(efficiency);

        AccountData AD = DS.GetUserData();
        AD.totalClicks = AD.totalClicks + tileClicks;
        float timer = Helpers.RoundToThreeDecimals(this.timer);
        AD.totalTimePlayed = AD.totalTimePlayed + timer;

        if (timer < 10) steamAPI.SetAchievement(UserAchievements.speedrunPro);
        if (timer < 20) steamAPI.SetAchievement(UserAchievements.speedrun);

        steamAPI.SetStatInt(UserStats.totalGamesPlayed, 1);
        steamAPI.SetStatInt(UserStats.totalClicks, tileClicks);

        steamAPI.UpdateLeaderBoard(LeaderboardStats.clicks, AD.totalClicks);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesPlayed, AD.gamesPlayed);

        if (tileClicks == 1 && loseGame) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);

        if (wonGame)
        {
            AD.gamesWon = AD.gamesWon + 1;

            steamAPI.SetStatInt(UserStats.totalGamesWon, 1);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.kris);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.noFlags);
            if (!usedFlag && (10 - bombDensity) >= 5) steamAPI.SetAchievement(UserAchievements.noFlagsPlus);

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
            steamAPI.SetStatInt(UserStats.islands1GamesPlayed, 1);
            steamAPI.SetStatInt(UserStats.islands1Victories, 1);
            steamAPI.SetStatInt(UserStats.islandsGamesWon, 1);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.islands1BestTime, (int)(AD.bossTime1 * 1000));
            steamAPI.UpdateLeaderBoard(LeaderboardStats.islandsGamesWon, AD.bossVictories);
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
            
        infoText.text = difficultyStars + "\n" +
            "Time: " + Helpers.RoundToThreeDecimals(data.bossTime1) + "s\n" +
            "Skill: " + Helpers.RoundToThreeDecimals(data.bossEfficiency1) + "%\n" +
            "Victories: " + data.bossVictories1 + "\n";
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
