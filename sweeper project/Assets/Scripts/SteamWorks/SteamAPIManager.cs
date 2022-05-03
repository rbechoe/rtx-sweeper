using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamAPIManager : MonoBehaviour
{
    public void SetAchievement(string name)
    {
        if (!SteamManager.Initialized)
        {
            Debug.Log("Steam manager not initialized!");
            return;
        }

        SteamUserStats.SetAchievement(name);
    }

    public void SetStat(string name, int value)
    {
        if (!SteamManager.Initialized)
        {
            Debug.Log("Steam manager not initialized!");
            return;
        }

        SteamUserStats.SetStat(name, value);
    }

    public void PushToCloud()
    {
        SteamUserStats.StoreStats();
    }
}
