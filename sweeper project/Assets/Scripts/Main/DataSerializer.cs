using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataSerializer : MonoBehaviour
{
    string fileName = "/gamestats.dat";
    float versionNumber;
    float requiredVersion = 0.75f;

    private void Start()
    {
        versionNumber = float.Parse(Application.version);
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

        // Update data
        AD.versionNumber = versionNumber;
        AD.tsSaved = GetUnixTimestamp();

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
    public float versionNumber = 0;
    public int tsSaved = 0;
    
    // account stats
    public int gamesPlayed = 0;
    public int gamesWon = 0;
    public int gamesLost = 0;
    public int totalClicks = 0;
    public int totalTimePlayed = 0;

    // arctic
    public int arcticVictories = 0;
    public int arcticLoses = 0;
    public int arcticGamesPlayed = 0;
    public int arcticTotalClicks = 0;
    public int arcticTime1 = 0;
    public int arcticClicks1 = 0;
    public int arcticEfficiency1 = 0;
    public int arcticVictories1 = 0;
    public int arcticTime2 = 0;
    public int arcticClicks2 = 0;
    public int arcticEfficiency2 = 0;
    public int arcticVictories2 = 0;
    public int arcticTime3 = 0;
    public int arcticClicks3 = 0;
    public int arcticEfficiency3 = 0;
    public int arcticVictories3 = 0;

    // Asia
    public int asiaVictories = 0;
    public int asiaLoses = 0;
    public int asiaGamesPlayed = 0;
    public int asiaTotalClicks = 0;
    public int asiaTime1 = 0;
    public int asiaClicks1 = 0;
    public int asiaEfficiency1 = 0;
    public int asiaVictories1 = 0;
    public int asiaTime2 = 0;
    public int asiaClicks2 = 0;
    public int asiaEfficiency2 = 0;
    public int asiaVictories2 = 0;
    public int asiaTime3 = 0;
    public int asiaClicks3 = 0;
    public int asiaEfficiency3 = 0;
    public int asiaVictories3 = 0;

    // Desert
    public int desertVictories = 0;
    public int desertLoses = 0;
    public int desertGamesPlayed = 0;
    public int desertTotalClicks = 0;
    public int desertTime1 = 0;
    public int desertClicks1 = 0;
    public int desertEfficiency1 = 0;
    public int desertVictories1 = 0;
    public int desertTime2 = 0;
    public int desertClicks2 = 0;
    public int desertEfficiency2 = 0;
    public int desertVictories2 = 0;
    public int desertTime3 = 0;
    public int desertClicks3 = 0;
    public int desertEfficiency3 = 0;
    public int desertVictories3 = 0;

    // Tutorial
    public int tutorialVictories = 0;

    // Boss
    public int bossVictories = 0;
    public int bossLoses = 0;
    public int bossGamesPlayed = 0;
    public int bossTotalClicks = 0;
    public int bossTime1 = 0;
    public int bossClicks1 = 0;
    public int bossEfficiency1 = 0;
    public int bossVictories1 = 0;
}
