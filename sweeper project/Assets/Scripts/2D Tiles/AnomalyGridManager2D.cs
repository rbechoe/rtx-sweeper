using UnityEngine;

public class AnomalyGridManager2D : BaseGridManager
{
    private SteamAPIManager steamAPI;

    public AnomalyLvl2 puzzle2Manager;

    private int difficulty;
    public bool isPartial;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 1;

        Helpers.NestedChildToGob<Tile2D>(transform, tiles);
        if (tiles.Count == 0)
        {
            Helpers.NestedChildToGob<Tile2DAnomaly>(transform, tiles);
        }
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

        DS = gameObject.GetComponent<DataSerializer>();
        SetText();
    }

    protected override void OnEnable()
    {
        EventSystem.eventCollectionParam[EventType.PLANT_FLAG] += ActivateFlag;
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

    protected override void Update()
    {
    }

    public void AddTile(GameObject value)
    {
        AddEmptyTile(value);
    }

    public void AddSafeTile(object value)
    {
        if (puzzle2Manager == null)
        {
            AddGoodTile(value);
        }
        else
        {
            if (!puzzle2Manager.timeStarted)
            {
                puzzle2Manager.timeStarted = true;
            }
            goodTiles++;
            CheckForVictory();
        }
    }

    protected override void CheckForVictory()
    {
        if (puzzle2Manager == null)
        {
            base.CheckForVictory();
        }
        else
        {
            progress = goodTiles / (tiles.Count - initialBombAmount);
            if (goodTiles == (tiles.Count - initialBombAmount))
            {
                puzzle2Manager.CompleteGrid(this);
            }
        }
    }

    protected override void TileClick()
    {
        if (puzzle2Manager == null)
        {
            base.TileClick();
        }
        else
        {
            puzzle2Manager.totalTileClicks++;
        }
    }

    protected override void FlagClick(object value)
    {
        if (puzzle2Manager == null)
        {
            base.FlagClick(value);
        }
        else
        {
            puzzle2Manager.totalOtherClicks++;
        }
    }

    protected override void PlantFlag()
    {
        if (puzzle2Manager == null)
        {
            base.PlantFlag();
        }
        else
        {
            puzzle2Manager.usedFlag = true;
        }

    }

    protected override void OtherClick()
    {

        if (puzzle2Manager == null)
        {
            base.OtherClick();
        }
        else
        {
            puzzle2Manager.totalOtherClicks++;
        }
    }

    protected override void StopTimer()
    {
        if (puzzle2Manager == null)
        {
            base.StopTimer();
        }
        else
        {
            puzzle2Manager.StopTimer();
        }
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
        }

        DS.UpdateAccountData(AD);
        SetText();
            
        wonGame = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (isPartial)
        {
            return;
        }

        if (data == null) data = DS.GetUserData();

        switch (area)
        {
            case 1: // anomaly arctic
                stars.text = "" + difficulty;
                infoText.text =
                    "Time: " + Helpers.RoundToThreeDecimals(data.anomalyTime1) + "s\n" +
                    "Skill: " + Helpers.RoundToThreeDecimals(data.anomalyEfficiency1) + "%\n" +
                    "Victories: " + data.anomalyVictories1 + "\n";
                break;
        }
    }
}
