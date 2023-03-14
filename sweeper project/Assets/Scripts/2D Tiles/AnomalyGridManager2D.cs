using UnityEngine;

public class AnomalyGridManager2D : BaseGridManager
{
    SteamAPIManager steamAPI;

    private int difficulty;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 1;

        Helpers.NestedChildToGob<Tile2D>(transform, tiles);
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

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
        EventSystem<GameObject>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.RemoveListener(EventType.WIN_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.END_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.RemoveListener(EventType.REVEAL_TILE, TileClick);
        EventSystem.RemoveListener(EventType.OTHER_CLICK, OtherClick);
        EventSystem.RemoveListener(EventType.PLAY_FLAG, PlantFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
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

        switch (area)
        {
            case 1: // Arctic anomaly
                AD.anomalyVictories = (wonGame) ? AD.anomalyVictories + 1 : AD.anomalyVictories;
                AD.anomalyTotalClicks += tileClicks;
                AD.anomalyGamesPlayed += 1;

                if (wonGame)
                {
                    AD.anomalyVictories1 += 1;

                    if (timer < AD.anomalyTime1 || (timer == AD.anomalyTime1 && efficiency > AD.anomalyEfficiency1) || AD.anomalyTime1 == 0)
                    {
                        AD.anomalyTime1 = timer;
                        AD.anomalyEfficiency1 = efficiency;
                        AD.anomalyClicks1 = tileClicks;
                    }
                }
                break;

            case 2: // Asia anomaly
                AD.anomalyVictories = (wonGame) ? AD.anomalyVictories + 1 : AD.anomalyVictories;
                AD.anomalyTotalClicks += tileClicks;
                AD.anomalyGamesPlayed += 1;

                if (wonGame)
                {
                    AD.anomalyVictories1 += 1;

                    if (timer < AD.anomalyTime1 || (timer == AD.anomalyTime1 && efficiency > AD.anomalyEfficiency1) || AD.anomalyTime1 == 0)
                    {
                        AD.anomalyTime1 = timer;
                        AD.anomalyEfficiency1 = efficiency;
                        AD.anomalyClicks1 = tileClicks;
                    }
                }
                break;

            case 3: // Desert anomaly
                AD.anomalyVictories = (wonGame) ? AD.anomalyVictories + 1 : AD.anomalyVictories;
                AD.anomalyTotalClicks += tileClicks;
                AD.anomalyGamesPlayed += 1;

                if (wonGame)
                {
                    AD.anomalyVictories1 += 1;

                    if (timer < AD.anomalyTime1 || (timer == AD.anomalyTime1 && efficiency > AD.anomalyEfficiency1) || AD.anomalyTime1 == 0)
                    {
                        AD.anomalyTime1 = timer;
                        AD.anomalyEfficiency1 = efficiency;
                        AD.anomalyClicks1 = tileClicks;
                    }
                }
                break;
        }

        DS.UpdateAccountData(AD);
        SetText();
            
        wonGame = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (data == null) data = DS.GetUserData();

        switch (area)
        {
            case 1: // anomaly
                stars.text = "" + difficulty;
                infoText.text =
                    "Time: " + Helpers.RoundToThreeDecimals(data.anomalyTime1) + "s\n" +
                    "Skill: " + Helpers.RoundToThreeDecimals(data.anomalyEfficiency1) + "%\n" +
                    "Victories: " + data.anomalyVictories1 + "\n";
                break;

            case 2: // anomaly
                stars.text = "" + difficulty;
                infoText.text =
                    "Time: " + Helpers.RoundToThreeDecimals(data.anomalyTime1) + "s\n" +
                    "Skill: " + Helpers.RoundToThreeDecimals(data.anomalyEfficiency1) + "%\n" +
                    "Victories: " + data.anomalyVictories1 + "\n";
                break;

            case 3: // anomaly
                stars.text = "" + difficulty;
                infoText.text =
                    "Time: " + Helpers.RoundToThreeDecimals(data.anomalyTime1) + "s\n" +
                    "Skill: " + Helpers.RoundToThreeDecimals(data.anomalyEfficiency1) + "%\n" +
                    "Victories: " + data.anomalyVictories1 + "\n";
                break;
        }
    }
}
