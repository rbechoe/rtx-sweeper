using System;
using UnityEngine;
using System.IO;

public class DataSerializer : MonoBehaviour
{
    string fileName = "/gamestats.dat";
    float versionNumber;
    float requiredVersion = 0.96f;
    CryptoManager crypto = new CryptoManager();
    SteamAPIManager steamAPI;

    private void Start()
    {
        steamAPI = SteamAPIManager.Instance;
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

        Write(AD);

        return AD;
    }

    // Retrieve account data
    public AccountData GetUserData()
    {
        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            return CreateNewFile();
        }
        else
        {
            return Read();
        }
    }

    // Update user data
    public void UpdateAccountData(AccountData newData)
    {
        // Set required params
        newData.tsSaved = GetUnixTimestamp();
        newData.versionNumber = versionNumber;

        Write(newData);

        // push achievement statistics
        steamAPI.PushToCloud();
    }

    private void Write(AccountData accountData)
    {
        RemoveFile();

        string data = JsonUtility.ToJson(accountData);
        File.WriteAllText(Application.persistentDataPath + fileName, data);

        Encrypt();
    }

    private AccountData Read()
    {
        Decrypt();

        string data = File.ReadAllText(Application.persistentDataPath + fileName);
        AccountData AD = JsonUtility.FromJson<AccountData>(data);

        Encrypt();

        return AD;
    }

    private void Encrypt()
    {
        // Encrypt saved file
        // load, read, encrypt, write
        string path = Application.persistentDataPath + fileName;
        StreamReader reader = new StreamReader(path);
        string newData = crypto.Encrypt(reader.ReadToEnd());
        reader.Close();

        RemoveFile();

        StreamWriter writer = new StreamWriter(path);
        writer.Write(newData);
        writer.Close();
    }

    private void Decrypt()
    {
        // Decrypt saved file
        // load, read, decrypt, write
        string path = Application.persistentDataPath + fileName;
        StreamReader reader = new StreamReader(path);
        string newData = crypto.Decrypt(reader.ReadToEnd());
        reader.Close();

        RemoveFile();

        StreamWriter writer = new StreamWriter(path);
        writer.Write(newData);
        writer.Close();
    }

    private void RemoveFile()
    {
        // Remove current file, otherwise it wont work
        if (File.Exists(Application.persistentDataPath + fileName))
            File.Delete(Application.persistentDataPath + fileName);
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
    public float totalTimePlayed = 0;

    // arctic
    public int arcticVictories = 0;
    public int arcticLoses = 0;
    public int arcticGamesPlayed = 0;
    public int arcticTotalClicks = 0;
    public float arcticTime1 = 0;
    public int arcticClicks1 = 0;
    public float arcticEfficiency1 = 0;
    public int arcticVictories1 = 0;
    public float arcticTime2 = 0;
    public int arcticClicks2 = 0;
    public float arcticEfficiency2 = 0;
    public int arcticVictories2 = 0;
    public float arcticTime3 = 0;
    public int arcticClicks3 = 0;
    public float arcticEfficiency3 = 0;
    public int arcticVictories3 = 0;

    // Asia
    public int asiaVictories = 0;
    public int asiaLoses = 0;
    public int asiaGamesPlayed = 0;
    public int asiaTotalClicks = 0;
    public float asiaTime1 = 0;
    public int asiaClicks1 = 0;
    public float asiaEfficiency1 = 0;
    public int asiaVictories1 = 0;
    public float asiaTime2 = 0;
    public int asiaClicks2 = 0;
    public float asiaEfficiency2 = 0;
    public int asiaVictories2 = 0;
    public float asiaTime3 = 0;
    public int asiaClicks3 = 0;
    public float asiaEfficiency3 = 0;
    public int asiaVictories3 = 0;

    // Desert
    public int desertVictories = 0;
    public int desertLoses = 0;
    public int desertGamesPlayed = 0;
    public int desertTotalClicks = 0;
    public float desertTime1 = 0;
    public int desertClicks1 = 0;
    public float desertEfficiency1 = 0;
    public int desertVictories1 = 0;
    public float desertTime2 = 0;
    public int desertClicks2 = 0;
    public float desertEfficiency2 = 0;
    public int desertVictories2 = 0;
    public float desertTime3 = 0;
    public int desertClicks3 = 0;
    public float desertEfficiency3 = 0;
    public int desertVictories3 = 0;

    // Tutorial
    public int tutorialVictories = 0;

    // Boss
    public int bossVictories = 0;
    public int bossLoses = 0;
    public int bossGamesPlayed = 0;
    public int bossTotalClicks = 0;
    public float bossTime1 = 0;
    public int bossClicks1 = 0;
    public float bossEfficiency1 = 0;
    public int bossVictories1 = 0;
}
