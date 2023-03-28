using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyLvl2 : MonoBehaviour
{
    public List<GameObject> unsolvedPuzzles = new List<GameObject>();
    public List<GameObject> solvedPuzzles = new List<GameObject>();
    public int activePuzzle = 0;
    public int switchCount = 0;
    public int switchTreshold = 3;
    public int difficulty = 10;

    public Text infoText, stars;
    public DataSerializer DS;

    public Transform activeSpot, inactiveSpot, solvedSpot;

    private float timer = 0;
    public bool timeStarted = false;

    public GameUI uiManager;
    public int totalTileClicks, totalOtherClicks;
    public bool usedFlag = false;
    private bool wonGame = false;

    private SteamAPIManager steamAPI;

    private void Awake()
    {
        SetText();
    }

    private void Start()
    {
        steamAPI = SteamAPIManager.Instance;

        foreach (GameObject puzzle in unsolvedPuzzles)
        {
            puzzle.transform.position = inactiveSpot.position;
        }
        unsolvedPuzzles[activePuzzle].transform.position = activeSpot.position;
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
        EventSystem<float>.InvokeEvent(EventType.UPDATE_TIME, timer);
    }

    private void Reset()
    {
        timer = 0;
        totalTileClicks = 0;
        totalOtherClicks = 0;
        timeStarted = false;
        wonGame = false;
    }

    public void ClickedTile()
    {
        switchCount++;

        if (switchCount >= switchTreshold)
        {
            activePuzzle++;
            switchCount = 0;
        }

        if (activePuzzle > unsolvedPuzzles.Count)
        {
            activePuzzle = 0;
        }

        foreach(GameObject puzzle in unsolvedPuzzles)
        {
            puzzle.transform.position = inactiveSpot.position;
        }
        unsolvedPuzzles[activePuzzle].transform.position = activeSpot.position;
    }

    public virtual void StopTimer()
    {
        timeStarted = false;
        SaveData();
    }

    public void CompleteGrid(GameObject grid)
    {
        unsolvedPuzzles.Remove(grid);
        solvedPuzzles.Add(grid);
        grid.transform.position = solvedSpot.position;

        if (unsolvedPuzzles.Count == 0)
        {
            timeStarted = false;
            wonGame = true;
            SaveData();
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

        steamAPI.UpdateLeaderBoard(LeaderboardStats.clicks, AD.totalClicks);
        steamAPI.UpdateLeaderBoard(LeaderboardStats.gamesPlayed, AD.gamesPlayed);

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
