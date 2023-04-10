using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyLvl2 : MonoBehaviour
{
    public List<AnomalyGridManager2D> unsolvedPuzzles = new List<AnomalyGridManager2D>();
    public List<AnomalyGridManager2D> solvedPuzzles = new List<AnomalyGridManager2D>();
    public int activePuzzle = 0;
    public int switchCount = 0;
    public int difficulty = 10;

    public Text infoText, stars;
    public DataSerializer DS;

    public Transform activeSpot, inactiveSpot, solvedSpot, lastSpot;

    private float timer = 0;
    public bool timeStarted = false;

    public GameUI uiManager;
    public int totalTileClicks, totalOtherClicks;
    public bool usedFlag = false;
    private bool wonGame = false;

    private SteamAPIManager steamAPI;

    private void OnEnable()
    {
        EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK] += ClickedTile;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += ResetGame;
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK] -= ClickedTile;
        EventSystem.eventCollection[EventType.RANDOM_GRID] += ResetGame;
    }

    private void Awake()
    {
        SetText();
    }

    private void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        unsolvedPuzzles[0].transform.position = activeSpot.position;
        unsolvedPuzzles[1].transform.position = inactiveSpot.position;
        unsolvedPuzzles[2].transform.position = lastSpot.position;

        unsolvedPuzzles[0].gridActive = true;
        unsolvedPuzzles[1].gridActive = false;
        unsolvedPuzzles[2].gridActive = false;
    }

    private void SetText()
    {
        AccountData data = DS.GetUserData();
        stars.text = "" + difficulty;
        infoText.text =
            "Time: " + Helpers.RoundToThreeDecimals(data.anomalyTime2) + "s\n" +
            "Skill: " + Helpers.RoundToThreeDecimals(data.anomalyEfficiency2) + "%\n" +
            "Victories: " + data.anomalyVictories2 + "\n";
    }

    private void Update()
    {
        if (timeStarted) timer += Time.deltaTime;
        EventSystem.eventCollectionParam[EventType.UPDATE_TIME](timer);
    }

    private void ResetGame()
    {
        timer = 0;
        activePuzzle = 0;
        switchCount = 0;
        totalTileClicks = 0;
        totalOtherClicks = 0;
        timeStarted = false;
        wonGame = false;

        unsolvedPuzzles[0].transform.position = activeSpot.position;
        unsolvedPuzzles[1].transform.position = inactiveSpot.position;
        unsolvedPuzzles[2].transform.position = lastSpot.position;

        unsolvedPuzzles[0].gridActive = true;
        unsolvedPuzzles[1].gridActive = false;
        unsolvedPuzzles[2].gridActive = false;
    }

    public void ClickedTile()
    {
        switchCount++;
        if (switchCount >= unsolvedPuzzles.Count && unsolvedPuzzles.Count > 0)
        {
            activePuzzle++;
            switchCount = 0;

            AnomalyGridManager2D grid = unsolvedPuzzles[0];
            unsolvedPuzzles.Remove(grid);
            unsolvedPuzzles.Add(grid);

            // prevents grids getting in each other even if for a single frame
            for (int i = 0; i < unsolvedPuzzles.Count; i++)
            {
                unsolvedPuzzles[i].transform.position = Vector3.up * 1000 * i;
            }

            for (int i = 0; i < unsolvedPuzzles.Count; i++)
            {
                if (i == 0)
                {
                    unsolvedPuzzles[i].transform.position = activeSpot.position;
                    unsolvedPuzzles[i].gridActive = true;
                }
                if (i == 1)
                {
                    unsolvedPuzzles[i].transform.position = lastSpot.position;
                    unsolvedPuzzles[i].gridActive = false;
                }
                if (i == 2)
                {
                    unsolvedPuzzles[i].transform.position = inactiveSpot.position;
                    unsolvedPuzzles[i].gridActive = false;
                }
            }
        }
    }

    public virtual void StopTimer()
    {
        timeStarted = false;
        SaveData();
    }

    public void CompleteGrid(AnomalyGridManager2D grid)
    {
        unsolvedPuzzles.Remove(grid);
        solvedPuzzles.Add(grid);
        grid.gridActive = false;
        grid.transform.position = solvedSpot.position;
        switchCount = 5;

        if (unsolvedPuzzles.Count == 0)
        {
            EventSystem.eventCollection[EventType.WIN_GAME]();
            wonGame = true;
            StopTimer();
        }
    }

    private void SaveData()
    {
        float efficiency = 1f * totalTileClicks / (totalTileClicks + totalOtherClicks) * 100f;
        efficiency = Mathf.Clamp(efficiency, 0, 100);
        uiManager.SetEfficiency(efficiency);

        AccountData AD = DS.GetUserData();
        AD.totalClicks = AD.totalClicks + totalTileClicks;
        AD.gamesPlayed = AD.gamesPlayed + 1;
        float timer = Helpers.RoundToThreeDecimals(this.timer);
        AD.totalTimePlayed = AD.totalTimePlayed + timer;

        steamAPI.SetStatInt(UserStats.totalGamesPlayed, AD.gamesPlayed);
        steamAPI.SetStatInt(UserStats.totalClicks, AD.totalClicks);

        if (totalTileClicks > 0)
        {
            steamAPI.UpdateLeaderBoard(LeaderboardStats.clicks, AD.totalClicks);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesPlayed, AD.gamesPlayed);
        }

        if (wonGame)
        {
            AD.gamesWon = AD.gamesWon + 1;

            steamAPI.SetStatInt(UserStats.totalGamesWon, AD.gamesWon);
            if (!usedFlag) steamAPI.SetAchievement(UserAchievements.noFlagsPlus);

            steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesWon, AD.gamesWon);
            steamAPI.UpdateLeaderBoard(LeaderboardStats.timePlayed, (int)(AD.totalTimePlayed / 60f));
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }

        // Asia anomaly
        AD.anomalyVictories = (wonGame) ? AD.anomalyVictories + 1 : AD.anomalyVictories;
        AD.anomalyTotalClicks += totalTileClicks;
        AD.anomalyGamesPlayed += 1;

        if (wonGame)
        {
            AD.anomalyVictories2 += 1;

            if (timer < AD.anomalyTime2 || (timer == AD.anomalyTime2 && efficiency > AD.anomalyEfficiency2) || AD.anomalyTime2 == 0)
            {
                AD.anomalyTime2 = timer;
                AD.anomalyEfficiency2 = efficiency;
                AD.anomalyClicks2 = totalTileClicks;
            }
        }

        DS.UpdateAccountData(AD);
        SetText();

        wonGame = false;
    }
}
