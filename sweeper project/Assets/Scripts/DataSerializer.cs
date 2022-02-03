using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataSerializer : MonoBehaviour
{
    string fileName = "/gamestats.dat";
    float versionNumber = 0.69f; //save logic number
    float requiredVersion = 0.5f;

    private void Start()
    {
        PurgeFilesCheck();
    }

    // Get unix timestamp
    public int GetUnixTimestamp()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int curTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return curTime;
    }

    // Check for purge
    public void PurgeFilesCheck()
    {
        AccountData AD = GetUserData();

        // Clean entire directory when too outdated
        if (AD.versionNumber < requiredVersion)
        {
            Directory.Delete(Application.persistentDataPath, true);
            Debug.Log("Removed old save files and generated complete new ones");
            CreateNewFile();
        }
    }

    // Create/overwrite current file with default data
    public AccountData CreateNewFile()
    {
        AccountData AD = new AccountData();

        // Create data
        AD.versionNumber = versionNumber;
        AD.tsSaved = GetUnixTimestamp();
        AD.gamesPlayed = 0;
        AD.gamesWon = 0;
        AD.gamesLost = 0;
        AD.totalClicks = 0;
        AD.totalTimePlayed = 0;
        AD.asiaVictories = 0;
        AD.asiaLoses = 0;
        AD.asiaGamesPlayed = 0;
        AD.asiaTotalClicks = 0;
        AD.asiaTime1 = 0;
        AD.asiaClicks1 = 0;
        AD.asiaEfficiency1 = 0;
        AD.asiaTime2 = 0;
        AD.asiaClicks2 = 0;
        AD.asiaEfficiency2 = 0;
        AD.asiaTime3 = 0;
        AD.asiaClicks3 = 0;
        AD.asiaEfficiency3 = 0;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + fileName, FileMode.OpenOrCreate);

        // Save changes
        bf.Serialize(fs, AD);
        fs.Close();

        return AD;
    }

    // Retrieve account data
    public AccountData GetUserData()
    {
        AccountData AD = new AccountData();

        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            AD = CreateNewFile();
        }
        else
        {
            // Open and deserialize data
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            AD = (AccountData)bf.Deserialize(fs);
            fs.Close();
        }

        return AD;
    }

    // Update user data
    public void UpdateAccountData(AccountData newData)
    {
        // Remove current file, otherwise it wont work
        if (File.Exists(Application.persistentDataPath + fileName))
            File.Delete(Application.persistentDataPath + fileName);

        // Create new file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + fileName, FileMode.OpenOrCreate);

        // Set required params
        newData.tsSaved = GetUnixTimestamp();
        newData.versionNumber = versionNumber;

        // Save new file
        bf.Serialize(fs, newData);
        fs.Close();
    }
}

[Serializable]
public class AccountData
{
    // global stats
    public float versionNumber;
    public int tsSaved;
    
    // account stats
    public int gamesPlayed;
    public int gamesWon;
    public int gamesLost;
    public int totalClicks;
    public int totalTimePlayed;

    // Asia
    public int asiaVictories;
    public int asiaLoses;
    public int asiaGamesPlayed;
    public int asiaTotalClicks;
    public int asiaTime1;
    public int asiaClicks1;
    public int asiaEfficiency1;
    public int asiaVictories1;
    public int asiaTime2;
    public int asiaClicks2;
    public int asiaEfficiency2;
    public int asiaVictories2;
    public int asiaTime3;
    public int asiaClicks3;
    public int asiaEfficiency3;
    public int asiaVictories3;
}