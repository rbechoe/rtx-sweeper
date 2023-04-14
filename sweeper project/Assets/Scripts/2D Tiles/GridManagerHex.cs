using UnityEngine;

public class GridManagerHex : BaseGridManager
{
    private SteamAPIManager steamAPI;

    private int difficulty;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        Helpers.NestedChildToGob<HexTileAnomaly>(transform, tiles);
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 10;

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

    public void AddTile(GameObject value)
    {
        AddEmptyTile(value);
    }

    // activate a flag and place it above the tile
    protected override void ActivateFlag(object value)
    {
        if (!gridActive)
        {
            return;
        }

        Transform tile = value as Transform;
        if (inactiveFlags.Count > 0 && bombAmount > 0)
        {
            inactiveFlags[0].transform.parent = tile;
            inactiveFlags[0].transform.localPosition = Vector3.up;
            inactiveFlags[0].transform.localEulerAngles = Vector3.zero;
            inactiveFlags[0].transform.localScale = Vector3.one * 1.5f;
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
        flag.transform.parent = flagParent.transform;
        activeFlags.Remove(flag);
        inactiveFlags.Add(flag);
        RemoveFlag();
    }

    protected override void LoseGame()
    {
        loseGame = true;

        if (tileClicks <= 1)
        {
            steamAPI.SetAchievement(UserAchievements.tasteOfMisery);
        }
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

            // TODO trigger Baarn achievement

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

        AD.anomalyVictories = (wonGame) ? AD.anomalyVictories + 1 : AD.anomalyVictories;
        AD.anomalyTotalClicks += tileClicks;
        AD.anomalyGamesPlayed += 1;

        if (wonGame)
        {
            AD.anomalyVictories3 += 1;

            if (timer < AD.anomalyTime3 || (timer == AD.anomalyTime3 && efficiency > AD.anomalyEfficiency3) || AD.anomalyTime3 == 0)
            {
                AD.anomalyTime3 = timer;
                AD.anomalyEfficiency3 = efficiency;
                AD.anomalyClicks3 = tileClicks;
            }
        }

        DS.UpdateAccountData(AD);
        SetText();

        wonGame = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (data == null) data = DS.GetUserData();

        stars.text = "" + difficulty;
        infoText.text =
            "Time: " + Helpers.RoundToThreeDecimals(data.anomalyTime3) + "s\n" +
            "Skill: " + Helpers.RoundToThreeDecimals(data.anomalyEfficiency3) + "%\n" +
            "Victories: " + data.anomalyVictories3 + "\n";
    }
}
