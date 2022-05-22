using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager3D : BaseGridManager
{
    SteamAPIManager steamAPI;

    private int difficulty;

    private bool saving;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 6;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Tile3D>())
            {
                tiles.Add(child.gameObject);
            }
        }

        foreach (Transform child in flagParent.transform)
        {
            if (child.GetComponent<Flag3D>())
            {
                inactiveFlags.Add(child.gameObject);
            }
        }

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
        EventSystem<GameObject>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
        EventSystem.AddListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.AddListener(EventType.WIN_GAME, StopTimer);
        EventSystem.AddListener(EventType.END_GAME, StopTimer);
        EventSystem.AddListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.AddListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.AddListener(EventType.TILE_CLICK, TileClick);
        EventSystem.AddListener(EventType.REVEAL_TILE, TileClick);
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
        EventSystem.RemoveListener(EventType.WIN_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.END_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.RemoveListener(EventType.TILE_CLICK, TileClick);
        EventSystem.RemoveListener(EventType.REVEAL_TILE, TileClick);
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, TileClick);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
    }

    protected override void SaveData()
    {
        if (saving) return;
        saving = true;

        float efficiency = 1f * (tiles.Count - initialBombAmount) / tileClicks * 100f;
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

            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesWon, AD.gamesWon);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.timePlayed, (int)(AD.totalTimePlayed / 60f));
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }

        AD.galaxyVictories += 1;
        if (timer < AD.galaxyTime1 || (timer == AD.galaxyTime1 && efficiency > AD.galaxyEfficiency1) || AD.galaxyTime1 == 0)
        {
            AD.galaxyTime1 = timer;
            AD.galaxyEfficiency1 = efficiency;
            AD.galaxyClicks1 = tileClicks;

            //steamAPI.SetStatFloat(UserStats.galaxy1BestTime, timer);
        }

        //steamAPI.SetStatInt(UserStats.galaxy1Victories, AD.galaxyVictories1);
        //steamAPI.UpdateLeaderBoard(LeaderboardStats.galaxy1BestTime, (int)(AD.galaxyTime1 * 1000));

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
}
