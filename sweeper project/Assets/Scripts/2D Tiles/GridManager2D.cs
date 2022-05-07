using UnityEngine;

public class GridManager2D : BaseGridManager
{
    SteamAPIManager steamAPI;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Tile2D>())
            {
                tiles.Add(child.gameObject);
            }
        }

        foreach (Transform child in flagParent.transform)
        {
            if (child.GetComponent<Flag2D>())
            {
                inactiveFlags.Add(child.gameObject);
            }
        }

        difficultyStars = "Difficulty: ";
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
        if (area == 4) return;

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
            if (!usedFlag && (10 - bombDensity) >= 5) steamAPI.SetAchievement(UserAchievements.noFlagsPlus);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesWon, AD.gamesWon);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.timePlayed, (int)(AD.totalTimePlayed / 60f));
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }

        switch (area)
        {
            case 1: // arctic
                AD.arcticVictories = (wonGame) ? AD.arcticVictories + 1 : AD.arcticVictories;
                AD.arcticTotalClicks += tileClicks;
                AD.arcticGamesPlayed += 1;

                if (wonGame)
                {
                    switch (level)
                    {
                        case 1: // level 1 - ice land
                            AD.arcticVictories1 += 1;

                            if (timer < AD.arcticTime1 || (timer == AD.arcticTime1 && efficiency > AD.arcticEfficiency1) || AD.arcticTime1 == 0)
                            {
                                AD.arcticTime1 = timer;
                                AD.arcticEfficiency1 = efficiency;
                                AD.arcticClicks1 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.ice1BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.ice1Victories, AD.arcticVictories1);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.ice1BestTime, (int)(AD.arcticTime1 * 1000));
                            break;

                        case 2: // level 2 - castle plaza
                            AD.arcticVictories2 += 1;

                            if (timer < AD.arcticTime2 || (timer == AD.arcticTime2 && efficiency > AD.arcticEfficiency2) || AD.arcticTime2 == 0)
                            {
                                AD.arcticTime2 = timer;
                                AD.arcticEfficiency2 = efficiency;
                                AD.arcticClicks2 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.ice2BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.ice2Victories, AD.arcticVictories2);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.ice2BestTime, (int)(AD.arcticTime2 * 1000));
                            break;

                        case 3: // level 3 - village
                            AD.arcticVictories3 += 1;

                            if (timer < AD.arcticTime3 || (timer == AD.arcticTime3 && efficiency > AD.arcticEfficiency3) || AD.arcticTime3 == 0)
                            {
                                AD.arcticTime3 = timer;
                                AD.arcticEfficiency3 = efficiency;
                                AD.arcticClicks3 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.ice3BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.ice3Victories, AD.arcticVictories3);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.ice3BestTime, (int)(AD.arcticTime3 * 1000));
                            break;
                    }

                    steamAPI.SetStatInt(UserStats.iceGamesWon, AD.arcticVictories);
                    steamAPI.UpdateLeaderBoard(LeaderboardStats.iceGamesWon, AD.arcticVictories);
                }
                steamAPI.SetStatInt(UserStats.iceGamesPlayed, AD.arcticGamesPlayed);
                steamAPI.UpdateLeaderBoard(LeaderboardStats.iceGamesPlayed, AD.arcticGamesPlayed);
                break;

            case 2: // asia
                AD.asiaVictories = (wonGame) ? AD.asiaVictories + 1 : AD.asiaVictories;
                AD.asiaTotalClicks += tileClicks;
                AD.asiaGamesPlayed += 1;

                if (wonGame)
                {
                    switch (level)
                    {
                        case 1: // level 1 - cliff
                            AD.asiaVictories1 += 1;

                            if (timer < AD.asiaTime1 || (timer == AD.asiaTime1 && efficiency > AD.asiaEfficiency1) || AD.asiaTime1 == 0)
                            {
                                AD.asiaTime1 = timer;
                                AD.asiaEfficiency1 = efficiency;
                                AD.asiaClicks1 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.asia1BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.asia1Victories, AD.asiaVictories1);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.asia1BestTime, (int)(AD.asiaTime1 * 1000));
                            break;

                        case 2: // level 2 - waterfalls
                            AD.asiaVictories2 += 1;

                            if (timer < AD.asiaTime2 || (timer == AD.asiaTime2 && efficiency > AD.asiaEfficiency2) || AD.asiaTime2 == 0)
                            {
                                AD.asiaTime2 = timer;
                                AD.asiaEfficiency2 = efficiency;
                                AD.asiaClicks2 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.asia2BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.asia2Victories, AD.asiaVictories2);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.asia2BestTime, (int)(AD.asiaTime2 * 1000));
                            break;

                        case 3: // level 3 - gate
                            AD.asiaVictories3 += 1;

                            if (timer < AD.asiaTime3 || (timer == AD.asiaTime3 && efficiency > AD.asiaEfficiency3) || AD.asiaTime3 == 0)
                            {
                                AD.asiaTime3 = timer;
                                AD.asiaEfficiency3 = efficiency;
                                AD.asiaClicks3 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.asia3BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.asia3Victories, AD.asiaVictories3);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.asia3BestTime, (int)(AD.asiaTime3 * 1000));
                            break;
                    }

                    steamAPI.SetStatInt(UserStats.asiaGamesWon, AD.asiaVictories);
                    steamAPI.UpdateLeaderBoard(LeaderboardStats.asiaGamesWon, AD.asiaVictories);
                }
                steamAPI.SetStatInt(UserStats.asiaGamesPlayed, AD.asiaGamesPlayed);
                steamAPI.UpdateLeaderBoard(LeaderboardStats.asiaGamesPlayed, AD.asiaGamesPlayed);
                break;

            case 3: // desert
                AD.desertVictories = (wonGame) ? AD.desertVictories + 1 : AD.desertVictories;
                AD.desertTotalClicks += tileClicks;
                AD.desertGamesPlayed += 1;

                if (wonGame)
                {
                    switch (level)
                    {
                        case 1: // level 1 - camp
                            AD.desertVictories1 += 1;

                            if (timer < AD.desertTime1 || (timer == AD.desertTime1 && efficiency > AD.desertEfficiency1) || AD.desertTime1 == 0)
                            {
                                AD.desertTime1 = timer;
                                AD.desertEfficiency1 = efficiency;
                                AD.desertClicks1 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.desert1BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.desert1Victories, AD.desertVictories1);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.desert1BestTime, (int)(AD.desertTime1 * 1000));
                            break;

                        case 2: // level 2 - village
                            AD.desertVictories2 += 1;

                            if (timer < AD.desertTime2 || (timer == AD.desertTime2 && efficiency > AD.desertEfficiency2) || AD.desertTime2 == 0)
                            {
                                AD.desertTime2 = timer;
                                AD.desertEfficiency2 = efficiency;
                                AD.desertClicks2 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.desert2BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.desert2Victories, AD.desertVictories2);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.desert2BestTime, (int)(AD.desertTime2 * 1000));
                            break;

                        case 3: // level 3 - desert
                            AD.desertVictories3 += 1;

                            if (timer < AD.desertTime3 || (timer == AD.desertTime3 && efficiency > AD.desertEfficiency3) || AD.desertTime3 == 0)
                            {
                                AD.desertTime3 = timer;
                                AD.desertEfficiency3 = efficiency;
                                AD.desertClicks3 = tileClicks;

                                steamAPI.SetStatFloat(UserStats.desert3BestTime, timer);
                            }

                            steamAPI.SetStatInt(UserStats.desert3Victories, AD.desertVictories3);
                            steamAPI.UpdateLeaderBoard(LeaderboardStats.desert3BestTime, (int)(AD.desertTime3 * 1000));
                            break;
                    }

                    steamAPI.SetStatInt(UserStats.desertGamesWon, AD.desertVictories);
                    steamAPI.UpdateLeaderBoard(LeaderboardStats.desertGamesWon, AD.desertVictories);
                }
                steamAPI.SetStatInt(UserStats.desertGamesPlayed, AD.desertGamesPlayed);
                steamAPI.UpdateLeaderBoard(LeaderboardStats.desertGamesPlayed, AD.desertGamesPlayed);
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
            case 1: // arctic
                switch (level)
                {
                    case 1: // level 1
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.arcticTime1) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.arcticEfficiency1) + "%\n" +
                            "Victories: " + data.arcticVictories1 + "\n";
                        break;
                    case 2: // level 2
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.arcticTime2) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.arcticEfficiency2) + "%\n" +
                            "Victories: " + data.arcticVictories2 + "\n";
                        break;
                    case 3: // level 3
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.arcticTime3) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.arcticEfficiency3) + "%\n" +
                            "Victories: " + data.arcticVictories3 + "\n";
                        break;
                }
                break;

            case 2: // asia
                switch (level)
                {
                    case 1: // level 1
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.asiaTime1) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.asiaEfficiency1) + "%\n" +
                            "Victories: " + data.asiaVictories1 + "\n";
                        break;
                    case 2: // level 2
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.asiaTime2) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.asiaEfficiency2) + "%\n" +
                            "Victories: " + data.asiaVictories2 + "\n";
                        break;
                    case 3: // level 3
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.asiaTime3) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.asiaEfficiency3) + "%\n" +
                            "Victories: " + data.asiaVictories3 + "\n";
                        break;
                }
                break;

            case 3: // desert
                switch (level)
                {
                    case 1: // level 1
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.desertTime1) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.desertEfficiency1) + "%\n" +
                            "Victories: " + data.desertVictories1 + "\n";
                        break;
                    case 2: // level 2
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.desertTime2) + "s\n" +
                            "Skill: " + Helpers.RoundToThreeDecimals(data.desertEfficiency2) + "%\n" +
                            "Victories: " + data.desertVictories2 + "\n";
                        break;
                    case 3: // level 3
                        infoText.text = difficultyStars + "\n" +
                            "Time: " + Helpers.RoundToThreeDecimals(data.desertTime3) + "s\n" +
                            "Efficiency: " + Helpers.RoundToThreeDecimals(data.desertEfficiency3) + "%\n" +
                            "Victories: " + data.desertVictories3 + "\n";
                        break;
                }
                break;
        }
    }
}
