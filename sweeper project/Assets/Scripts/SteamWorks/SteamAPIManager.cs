using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamAPIManager : MonoBehaviour
{
    private Dictionary<UserAchievements, string> apiAchName = new Dictionary<UserAchievements, string>();
    private Dictionary<UserStats, string> apiStatName = new Dictionary<UserStats, string>();
    private Dictionary<LeaderboardStats, string> lbStatName = new Dictionary<LeaderboardStats, string>();

    private static SteamAPIManager instance;
    public static SteamAPIManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // populating achievements
        apiAchName.Add(UserAchievements.eight, "Eight");
        apiAchName.Add(UserAchievements.noFlags, "No flags");
        apiAchName.Add(UserAchievements.noFlagsPlus, "No flags on steroids");
        apiAchName.Add(UserAchievements.tasteOfMisery, "A taste of misery");
        apiAchName.Add(UserAchievements.speedrun, "Speedrun Novice");
        apiAchName.Add(UserAchievements.speedrunPro, "Speedrun Pro");
        apiAchName.Add(UserAchievements.kris, "k4b4448");
        apiAchName.Add(UserAchievements.ragequit, "Ragequit");
        // populating stats
        apiStatName.Add(UserStats.totalGamesPlayed, "total_games_played");
        apiStatName.Add(UserStats.totalGamesWon, "total_games_won");
        apiStatName.Add(UserStats.totalClicks, "total_clicks");
        apiStatName.Add(UserStats.iceGamesWon, "ice_games_won");
        apiStatName.Add(UserStats.iceGamesPlayed, "ice_games_played");
        apiStatName.Add(UserStats.ice1Victories, "ice_1_victories");
        apiStatName.Add(UserStats.ice1BestTime, "ice_1_best_time");
        apiStatName.Add(UserStats.ice1GamesPlayed, "ice_1_games_played");
        apiStatName.Add(UserStats.ice2Victories, "ice_2_victories");
        apiStatName.Add(UserStats.ice2BestTime, "ice_2_best_time");
        apiStatName.Add(UserStats.ice2GamesPlayed, "ice_2_games_played");
        apiStatName.Add(UserStats.ice3Victories, "ice_3_victories");
        apiStatName.Add(UserStats.ice3BestTime, "ice_3_best_time");
        apiStatName.Add(UserStats.ice3GamesPlayed, "ice_3_games_played");
        apiStatName.Add(UserStats.asiaGamesWon, "asia_games_won");
        apiStatName.Add(UserStats.asiaGamesPlayed, "asia_games_played");
        apiStatName.Add(UserStats.asia1Victories, "asia_1_victories");
        apiStatName.Add(UserStats.asia1BestTime, "asia_1_best_time");
        apiStatName.Add(UserStats.asia1GamesPlayed, "asia_1_games_played");
        apiStatName.Add(UserStats.asia2Victories, "asia_2_victories");
        apiStatName.Add(UserStats.asia2BestTime, "asia_2_best_time");
        apiStatName.Add(UserStats.asia2GamesPlayed, "asia_2_games_played");
        apiStatName.Add(UserStats.asia3Victories, "asia_3_victories");
        apiStatName.Add(UserStats.asia3BestTime, "asia_3_best_time");
        apiStatName.Add(UserStats.asia3GamesPlayed, "asia_3_games_played");
        apiStatName.Add(UserStats.desertGamesWon, "desert_games_won");
        apiStatName.Add(UserStats.desertGamesPlayed, "desert_games_played");
        apiStatName.Add(UserStats.desert1Victories, "desert_1_victories");
        apiStatName.Add(UserStats.desert1BestTime, "desert_1_best_time");
        apiStatName.Add(UserStats.desert1GamesPlayed, "desert_1_games_played");
        apiStatName.Add(UserStats.desert2Victories, "desert_2_victories");
        apiStatName.Add(UserStats.desert2BestTime, "desert_2_best_time");
        apiStatName.Add(UserStats.desert2GamesPlayed, "desert_2_games_played");
        apiStatName.Add(UserStats.desert3Victories, "desert_3_victories");
        apiStatName.Add(UserStats.desert3BestTime, "desert_3_best_time");
        apiStatName.Add(UserStats.desert3GamesPlayed, "desert_3_games_played");
        apiStatName.Add(UserStats.islandsGamesWon, "islands_games_won");
        apiStatName.Add(UserStats.islandsGamesPlayed, "islands_games_played");
        apiStatName.Add(UserStats.islands1Victories, "islands_1_victories");
        apiStatName.Add(UserStats.islands1BestTime, "islands_1_best_time");
        apiStatName.Add(UserStats.islands1GamesPlayed, "islands_1_games_played");
        // populating leaderboards
        lbStatName.Add(LeaderboardStats.clicks, "lb_clicks");
        lbStatName.Add(LeaderboardStats.gamesPlayed, "lb_games_played");
        lbStatName.Add(LeaderboardStats.gamesWon, "lb_games_won");
        lbStatName.Add(LeaderboardStats.timePlayed, "lb_time_played");
        lbStatName.Add(LeaderboardStats.iceGamesWon, "lb_ice_games_won");
        lbStatName.Add(LeaderboardStats.iceGamesPlayed, "lb_ice_games_played");
        lbStatName.Add(LeaderboardStats.ice1BestTime, "lb_ice_1_best_time");
        lbStatName.Add(LeaderboardStats.ice2BestTime, "lb_ice_2_best_time");
        lbStatName.Add(LeaderboardStats.ice3BestTime, "lb_ice_3_best_time");
        lbStatName.Add(LeaderboardStats.asiaGamesWon, "lb_asia_games_won");
        lbStatName.Add(LeaderboardStats.asiaGamesPlayed, "lb_asia_games_played");
        lbStatName.Add(LeaderboardStats.asia1BestTime, "lb_asia_1_best_time");
        lbStatName.Add(LeaderboardStats.asia2BestTime, "lb_asia_2_best_time");
        lbStatName.Add(LeaderboardStats.asia3BestTime, "lb_asia_3_best_time");
        lbStatName.Add(LeaderboardStats.desertGamesWon, "lb_desert_games_won");
        lbStatName.Add(LeaderboardStats.desertGamesPlayed, "lb_desert_games_played");
        lbStatName.Add(LeaderboardStats.desert1BestTime, "lb_desert_1_best_time");
        lbStatName.Add(LeaderboardStats.desert2BestTime, "lb_desert_2_best_time");
        lbStatName.Add(LeaderboardStats.desert3BestTime, "lb_desert_3_best_time");
        lbStatName.Add(LeaderboardStats.islandsGamesWon, "lb_islands_games_won");
        lbStatName.Add(LeaderboardStats.islandsGamesPlayed, "lb_islands_games_played");
        lbStatName.Add(LeaderboardStats.islands1BestTime, "lb_islands_1_best_time");
    }

    public void SetAchievement(UserAchievements name)
    {
        if (!SteamManager.Initialized)
        {
            Debug.Log("Steam manager not initialized!");
            return;
        }

        SteamUserStats.SetAchievement(apiAchName[name]);
    }

    public void SetStatInt(UserStats name, int value)
    {
        if (!SteamManager.Initialized)
        {
            Debug.Log("Steam manager not initialized!");
            return;
        }

        SteamUserStats.SetStat(apiStatName[name], value);
    }

    public void SetStatFloat(UserStats name, float value)
    {
        if (!SteamManager.Initialized)
        {
            Debug.Log("Steam manager not initialized!");
            return;
        }

        SteamUserStats.SetStat(apiStatName[name], value);
    }

    /// <summary>
    /// Used to update leaderboards
    /// </summary>
    /// <param name="lb"></param>
    /// <param name="val">for timings do x1000 for ms based lb</param>
    public void UpdateLeaderBoard(LeaderboardStats lb, int val)
    {
        StartCoroutine(LBUpdate(lb, val));
    }

    private IEnumerator LBUpdate(LeaderboardStats lb, int score)
    {
        bool error = false;

        SteamLeaderboard_t highScoreLeaderboard = new SteamLeaderboard_t();
        bool findLeaderboardCallCompleted = false;

        var findLeaderboardCall = SteamUserStats.FindLeaderboard(lbStatName[lb]);
        var findLeaderboardCallResult = CallResult<LeaderboardFindResult_t>.Create();
        findLeaderboardCallResult.Set(findLeaderboardCall, (leaderboardFindResult, failure) =>
        {
            if (!failure && leaderboardFindResult.m_bLeaderboardFound == 1)
            {
                highScoreLeaderboard = leaderboardFindResult.m_hSteamLeaderboard;
            }
            else
            {
                error = true;
            }

            findLeaderboardCallCompleted = true;
        });

        while (!findLeaderboardCallCompleted) yield return null;

        if (error)
        {
            Debug.Log("Error finding leaderboard: " + lbStatName[lb]);
            yield break;
        }

        LeaderboardScoreUploaded_t leaderboardScore = new LeaderboardScoreUploaded_t();
        bool uploadLeaderboardScoreCallCompleted = false;

        var uploadLeaderboardScoreCall = SteamUserStats.UploadLeaderboardScore(highScoreLeaderboard,
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, new int[0], 0);
        var uploadLeaderboardScoreCallResult = CallResult<LeaderboardScoreUploaded_t>.Create();
        uploadLeaderboardScoreCallResult.Set(uploadLeaderboardScoreCall, (scoreUploadedResult, failure) =>
        {
            if (!failure && scoreUploadedResult.m_bSuccess == 1)
            {
                leaderboardScore = scoreUploadedResult;
            }
            else
            {
                error = true;
            }

            uploadLeaderboardScoreCallCompleted = true;
        });

        while (!uploadLeaderboardScoreCallCompleted) yield return null;

        if (error)
        {
            Debug.Log("Error uploading high score to leaderboard: " + lbStatName[lb]);
            yield break;
        }
    }

    public void PushToCloud()
    {
        SteamUserStats.StoreStats();
    }
}

public enum UserAchievements
{
    eight,
    noFlags,
    noFlagsPlus,
    tasteOfMisery,
    speedrun,
    speedrunPro,
    kris,
    ragequit
}

public enum UserStats
{
    totalGamesPlayed,
    totalGamesWon,
    totalClicks,
    iceGamesWon,
    iceGamesPlayed,
    ice1Victories,
    ice1BestTime,
    ice1GamesPlayed,
    ice2Victories,
    ice2BestTime,
    ice2GamesPlayed,
    ice3Victories,
    ice3BestTime,
    ice3GamesPlayed,
    asiaGamesWon,
    asiaGamesPlayed,
    asia1Victories,
    asia1BestTime,
    asia1GamesPlayed,
    asia2Victories,
    asia2BestTime,
    asia2GamesPlayed,
    asia3Victories,
    asia3BestTime,
    asia3GamesPlayed,
    desertGamesWon,
    desertGamesPlayed,
    desert1Victories,
    desert1BestTime,
    desert1GamesPlayed,
    desert2Victories,
    desert2BestTime,
    desert2GamesPlayed,
    desert3Victories,
    desert3BestTime,
    desert3GamesPlayed,
    islandsGamesWon,
    islandsGamesPlayed,
    islands1Victories,
    islands1BestTime,
    islands1GamesPlayed
}

public enum LeaderboardStats
{
    clicks,
    gamesPlayed,
    gamesWon,
    timePlayed,
    iceGamesWon,
    iceGamesPlayed,
    ice1BestTime,
    ice2BestTime,
    ice3BestTime,
    asiaGamesWon,
    asiaGamesPlayed,
    asia1BestTime,
    asia2BestTime,
    asia3BestTime,
    desertGamesWon,
    desertGamesPlayed,
    desert1BestTime,
    desert2BestTime,
    desert3BestTime,
    islandsGamesWon,
    islandsGamesPlayed,
    islands1BestTime
}
