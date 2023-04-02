using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnomalyLvl2 : MonoBehaviour
{
    public List<AnomalyGridManager2D> unsolvedPuzzles = new List<AnomalyGridManager2D>();
    public List<AnomalyGridManager2D> solvedPuzzles = new List<AnomalyGridManager2D>();
    public int activePuzzle = 0;
    public int switchCount = 0;
    public int switchTreshold = 3;
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
    }

    private void OnDisable()
    {
        EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK] -= ClickedTile;
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

    private void Reset()
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

        int prevPuzzle = activePuzzle;
        if (switchCount >= switchTreshold)
        {
            activePuzzle++;
            switchCount = 0;
        }

        if (activePuzzle >= unsolvedPuzzles.Count)
        {
            activePuzzle = 0;
        }

        if (prevPuzzle != activePuzzle)
        {
            unsolvedPuzzles[prevPuzzle].transform.position = lastSpot.position;
            unsolvedPuzzles[activePuzzle].transform.position = activeSpot.position;

            int nextPuzzle = activePuzzle + 1;
            if (nextPuzzle >= unsolvedPuzzles.Count)
            {
                nextPuzzle = 0;
            }
            unsolvedPuzzles[nextPuzzle].transform.position = inactiveSpot.position;

            unsolvedPuzzles[prevPuzzle].gridActive = false;
            unsolvedPuzzles[activePuzzle].gridActive = true;
            unsolvedPuzzles[nextPuzzle].gridActive = false;
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
