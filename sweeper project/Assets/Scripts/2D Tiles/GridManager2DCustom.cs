using UnityEngine;

public class GridManager2DCustom : BaseGridManager
{
    SteamAPIManager steamAPI;

    private int difficulty;

    protected override void Start()
    {
        steamAPI = SteamAPIManager.Instance;
        difficulty = (10 - bombDensity) + (tiles.Count / 200) + 1;

        Helpers.NestedChildToGob<Tile2D>(transform, tiles);
        Helpers.NestedChildToGob<Flag2D>(flagParent.transform, inactiveFlags);

        DS = gameObject.GetComponent<DataSerializer>();
        SetText();
    }

    protected override void OnEnable()
    {
        EventSystem<GameObject>.AddListener(EventType.ADD_GOOD_TILE, AddGoodTile);
        EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<GameObject>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
        EventSystem.AddListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.AddListener(EventType.WIN_GAME, StopTimer);
        EventSystem.AddListener(EventType.END_GAME, StopTimer);
        EventSystem.AddListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.AddListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.AddListener(EventType.REVEAL_TILE, TileClick);
        EventSystem.AddListener(EventType.OTHER_CLICK, OtherClick);
        EventSystem.AddListener(EventType.PLAY_FLAG, PlantFlag);
        EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, FlagClick);
    }

    protected override void OnDisable()
    {
        EventSystem<GameObject>.RemoveListener(EventType.ADD_GOOD_TILE, AddGoodTile);
        EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
        EventSystem<GameObject>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
        EventSystem.RemoveListener(EventType.RANDOM_GRID, ResetGame);
        EventSystem.RemoveListener(EventType.WIN_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.END_GAME, StopTimer);
        EventSystem.RemoveListener(EventType.GAME_LOSE, LoseGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, StopTimer);
        EventSystem.RemoveListener(EventType.REVEAL_TILE, TileClick);
        EventSystem.RemoveListener(EventType.OTHER_CLICK, OtherClick);
        EventSystem.RemoveListener(EventType.PLAY_FLAG, PlantFlag);
        EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
    }

    protected override void LoseGame()
    {
        loseGame = true;

        if (tileClicks <= 1) steamAPI.SetAchievement(UserAchievements.tasteOfMisery);
    }

    protected override void SaveData()
    {
        if (area == 4) return;

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

        if (wonGame)
        {
            AD.gamesWon = AD.gamesWon + 1;

            steamAPI.SetStatInt(UserStats.totalGamesWon, AD.gamesWon);
        }
        else
        {
            AD.gamesLost = AD.gamesLost + 1;
        }

        DS.UpdateAccountData(AD);
        SetText();
            
        wonGame = false;
    }

    protected override void SetText(AccountData data = null)
    {
        if (data == null) data = DS.GetUserData();
    }
}
