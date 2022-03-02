using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tiles2D
{
    public class GridManager : Base
    {
        private List<GameObject> tiles = new List<GameObject>();
        private List<GameObject> activeFlags = new List<GameObject>();
        private List<GameObject> inactiveFlags = new List<GameObject>();

        private GameObject firstTile;
        private List<GameObject> emptyTiles = new List<GameObject>();
        private int bombAmount;
        private int initialBombAmount;
        private int goodTiles = 0;

        [Header("Settings")]
        [Tooltip("1:X where 1 is the bomb and X is the amount of non bomb tiles")]
        [SerializeField] private int bombDensity = 6;
        [SerializeField] private GameObject flagParent;

        private bool timeStarted;
        private bool inReset;
        private bool wonGame;
        private int tileClicks;
        private float timer;

        private DataSerializer DS;

        public TextMeshProUGUI infoText;

        [Header("Level specific")]
        [Tooltip("1 = arctic, 2 = asia, 3 = desert, 4 = tutorial")]
        public int area;
        public int level;
        public Color emptyTileColor = Color.black;
        public Color startColor = Color.blue;

        private string difficultyStars;
        private bool firstTime = true; // used to avoid bug, clean solution needs to be fixed!

        protected override void Start()
        {
            base.Start();

            difficultyStars = "Difficulty: ";
            for (int i = 0; i < (10 - bombDensity); i++)
            {
                difficultyStars += "*";
            }

            foreach (Transform child in transform)
            {
                if (child.GetComponent<Checker>())
                {
                    tiles.Add(child.gameObject);
                }
            }

            foreach (Transform child in flagParent.transform)
            {
                if (child.GetComponent<Flag>())
                {
                    inactiveFlags.Add(child.gameObject);
                }
            }

            DS = gameObject.GetComponent<DataSerializer>();
            SetText();
        }

        protected override void Update()
        {
            if (timeStarted)
            {
                timer += Time.deltaTime;
            }
            EventSystem<float>.InvokeEvent(EventType.UPDATE_TIME, timer);
        }

        private void OnEnable()
        {
            EventSystem<GameObject>.AddListener(EventType.ADD_GOOD_TILE, AddGoodTile);
            EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
            EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
            EventSystem<GameObject>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
            EventSystem.AddListener(EventType.RANDOM_GRID, ResetGame);
            EventSystem.AddListener(EventType.WIN_GAME, StopTimer);
            EventSystem.AddListener(EventType.END_GAME, StopTimer);
            EventSystem.AddListener(EventType.GAME_LOSE, StopTimer);
            EventSystem.AddListener(EventType.TILE_CLICK, TileClick);
            EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, TileClick);
            EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, FlagClick);
        }

        private void OnDisable()
        {
            EventSystem<GameObject>.RemoveListener(EventType.ADD_GOOD_TILE, AddGoodTile);
            EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
            EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
            EventSystem<GameObject>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
            EventSystem.RemoveListener(EventType.RANDOM_GRID, ResetGame);
            EventSystem.RemoveListener(EventType.WIN_GAME, StopTimer);
            EventSystem.RemoveListener(EventType.END_GAME, StopTimer);
            EventSystem.RemoveListener(EventType.GAME_LOSE, StopTimer);
            EventSystem.RemoveListener(EventType.TILE_CLICK, TileClick);
            EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, TileClick);
            EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
        }

        private IEnumerator RandomizeGrid()
        {
            int curTile = 0;
            int tilesLeft = 0;
            int spawnChance = 0;
            int bombCount = 0;
            int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
            int curTileCount = 0;

            for (int tileId = 0; tileId < tiles.Count; tileId++)
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    tilesLeft = tiles.Count - curTile;
                    spawnChance = tilesLeft / (bombAmount - bombCount);
                }

                GameObject newTile = tiles[tileId];
                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.tag = "Bomb";
                    newTile.layer = 11;
                    newTile.AddComponent<BombTile>();
                    bombCount++;
                }
                else
                {
                    newTile.tag = "Empty";
                    newTile.layer = 12;
                }

                curTile++;
                curTileCount++;
                newTile.name = "tile " + curTile;

                // continue next frame
                if (curTileCount >= tilesPerFrame)
                {
                    curTileCount = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();

            EventSystem.InvokeEvent(EventType.PREPARE_GAME);
            StartGame();
            yield return new WaitForEndOfFrame();
        }

        // activate a flag and place it above the tile
        private void ActivateFlag(Vector3[] vectors)
        {
            if (inactiveFlags.Count > 0 && bombAmount > 0)
            {
                inactiveFlags[0].transform.position = vectors[0];
                inactiveFlags[0].transform.eulerAngles = vectors[1];
                activeFlags.Add(inactiveFlags[0]);
                inactiveFlags.RemoveAt(0);
                AddFlag();
            }
        }

        // remove a flag from the tile
        public void ReturnFlag(GameObject flag)
        {
            flag.transform.position = Vector3.up * 5000;
            activeFlags.Remove(flag);
            inactiveFlags.Add(flag);
            RemoveFlag();
        }

        private void AddFlag()
        {
            bombAmount--;
            EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
        }

        private void RemoveFlag()
        {
            bombAmount++;
            EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
        }

        private void AddEmptyTile(GameObject gameobject)
        {
            emptyTiles.Add(gameobject);
        }

        private void PickStartingTile()
        {
            if (emptyTiles.Count > 0)
            {
                firstTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
                firstTile.GetComponent<Tile>().FirstTile();
            }
        }

        private void ResetGame()
        {
            if (inReset)
            {
                return;
            }
            else
            {
                inReset = true;
                firstTile = null;
                timeStarted = false;
                goodTiles = 0;
                timer = 0;
                tileClicks = 0;
                bombAmount = tiles.Count / bombDensity;
                initialBombAmount = bombAmount;
                emptyTiles = new List<GameObject>();
                StartCoroutine(ResetLogic());
            }
        }

        IEnumerator ResetLogic()
        {
            EventSystem.InvokeEvent(EventType.END_GAME);
            yield return new WaitForEndOfFrame();

            // move all active flags to inactive
            foreach (GameObject flag in activeFlags)
            {
                flag.transform.parent = flagParent.transform;
                flag.transform.localPosition = Vector3.zero;
                inactiveFlags.Add(flag);
            }
            activeFlags = new List<GameObject>();
            yield return new WaitForEndOfFrame();

            // reset the grid
            StartCoroutine(RandomizeGrid());
        }

        private void StartGame()
        {
            EventSystem.InvokeEvent(EventType.COUNT_BOMBS);
            PickStartingTile();
            EventSystem.InvokeEvent(EventType.START_GAME);
            EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
            inReset = false;
        }

        private void AddGoodTile(GameObject tile)
        {
            if (!timeStarted)
            {
                timeStarted = true;
            }
            goodTiles++;
            CheckForVictory();
        }

        private void CheckForVictory()
        {
            if (goodTiles == (tiles.Count - initialBombAmount))
            {
                wonGame = true;
                EventSystem.InvokeEvent(EventType.WIN_GAME);
            }
        }

        private void TileClick()
        {
            tileClicks++;
        }

        private void TileClick(Vector3[] vectors)
        {
            tileClicks++;
        }

        private void FlagClick(GameObject flag)
        {
            tileClicks++;
        }

        private void StopTimer()
        {
            if (firstTime || tileClicks == 0)
            {
                firstTime = false;
                return;
            }
            timeStarted = false;

            SaveData();
        }

        private void SaveData()
        {
            if (area == 4) return;

            float efficiency = (initialBombAmount * (bombDensity - 1f)) / tileClicks * 50f;
            efficiency = Mathf.Clamp(efficiency, 0, 100);

            AccountData AD = DS.GetUserData();
            AD.totalClicks = AD.totalClicks + tileClicks;
            int timer = (int)this.timer;
            AD.totalTimePlayed = AD.totalTimePlayed + timer;

            if (wonGame)
                AD.gamesWon = AD.gamesWon + 1;
            else
                AD.gamesLost = AD.gamesLost + 1;

            switch (area)
            {
                case 1: // arctic
                    AD.arcticVictories = (wonGame) ? AD.arcticVictories + 1 : AD.arcticVictories;
                    AD.arcticTotalClicks += tileClicks;
                    AD.arcticGamesPlayed += 1;

                    if (wonGame)
                    {
                        switch (level)
                        {
                            case 1: // level 1
                                AD.arcticVictories1 += 1;

                                if (timer < AD.arcticTime1 || (timer == AD.arcticTime1 && efficiency > AD.arcticEfficiency1) || AD.arcticTime1 == 0)
                                {
                                    AD.arcticTime1 = timer;
                                    AD.arcticEfficiency1 = (int)efficiency;
                                    AD.arcticClicks1 = tileClicks;
                                }
                                break;

                            case 2: // level 2
                                AD.arcticVictories2 += 1;

                                if (timer < AD.arcticTime2 || (timer == AD.arcticTime2 && efficiency > AD.arcticEfficiency2) || AD.arcticTime2 == 0)
                                {
                                    AD.arcticTime2 = timer;
                                    AD.arcticEfficiency2 = (int)efficiency;
                                    AD.arcticClicks2 = tileClicks;
                                }
                                break;

                            case 3: // level 3
                                AD.arcticVictories3 += 1;

                                if (timer < AD.arcticTime3 || (timer == AD.arcticTime3 && efficiency > AD.arcticEfficiency3) || AD.arcticTime1 == 0)
                                {
                                    AD.arcticTime3 = timer;
                                    AD.arcticEfficiency3 = (int)efficiency;
                                    AD.arcticClicks3 = tileClicks;
                                }
                                break;
                        }
                    }
                    break;

                case 2: // asia
                    AD.asiaVictories = (wonGame) ? AD.asiaVictories + 1 : AD.asiaVictories;
                    AD.asiaTotalClicks += tileClicks;
                    AD.asiaGamesPlayed += 1;

                    if (wonGame)
                    {
                        switch (level)
                        {
                            case 1: // level 1
                                AD.asiaVictories1 += 1;

                                if (timer < AD.asiaTime1 || (timer == AD.asiaTime1 && efficiency > AD.asiaEfficiency1) || AD.asiaTime1 == 0)
                                {
                                    AD.asiaTime1 = timer;
                                    AD.asiaEfficiency1 = (int)efficiency;
                                    AD.asiaClicks1 = tileClicks;
                                }
                                break;

                            case 2: // level 2
                                AD.asiaVictories2 += 1;

                                if (timer < AD.asiaTime2 || (timer == AD.asiaTime2 && efficiency > AD.asiaEfficiency2) || AD.asiaTime2 == 0)
                                {
                                    AD.asiaTime2 = timer;
                                    AD.asiaEfficiency2 = (int)efficiency;
                                    AD.asiaClicks2 = tileClicks;
                                }
                                break;

                            case 3: // level 3
                                AD.asiaVictories3 += 1;

                                if (timer < AD.asiaTime3 || (timer == AD.asiaTime3 && efficiency > AD.asiaEfficiency3) || AD.asiaTime1 == 0)
                                {
                                    AD.asiaTime3 = timer;
                                    AD.asiaEfficiency3 = (int)efficiency;
                                    AD.asiaClicks3 = tileClicks;
                                }
                                break;
                        }
                    }
                    break;

                case 3: // desert
                    AD.desertVictories = (wonGame) ? AD.desertVictories + 1 : AD.desertVictories;
                    AD.desertTotalClicks += tileClicks;
                    AD.desertGamesPlayed += 1;

                    if (wonGame)
                    {
                        switch (level)
                        {
                            case 1: // level 1
                                AD.desertVictories1 += 1;

                                if (timer < AD.desertTime1 || (timer == AD.desertTime1 && efficiency > AD.desertEfficiency1) || AD.desertTime1 == 0)
                                {
                                    AD.desertTime1 = timer;
                                    AD.desertEfficiency1 = (int)efficiency;
                                    AD.desertClicks1 = tileClicks;
                                }
                                break;

                            case 2: // level 2
                                AD.desertVictories2 += 1;

                                if (timer < AD.desertTime2 || (timer == AD.desertTime2 && efficiency > AD.desertEfficiency2) || AD.desertTime2 == 0)
                                {
                                    AD.desertTime2 = timer;
                                    AD.desertEfficiency2 = (int)efficiency;
                                    AD.desertClicks2 = tileClicks;
                                }
                                break;

                            case 3: // level 3
                                AD.desertVictories3 += 1;

                                if (timer < AD.desertTime3 || (timer == AD.desertTime3 && efficiency > AD.desertEfficiency3) || AD.desertTime1 == 0)
                                {
                                    AD.desertTime3 = timer;
                                    AD.desertEfficiency3 = (int)efficiency;
                                    AD.desertClicks3 = tileClicks;
                                }
                                break;
                        }
                    }
                    break;
            }

            DS.UpdateAccountData(AD);
            SetText();
            
            wonGame = false;
        }

        private void SetText(AccountData data = null)
        {
            if (data == null) data = DS.GetUserData();

            switch (area)
            {
                case 1: // arctic
                    switch (level)
                    {
                        case 1: // level 1
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.arcticTime1 + "s\n" +
                                "Efficiency: " + data.arcticEfficiency1 + "\n" +
                                "Victories: " + data.arcticVictories1 + "\n";
                            break;
                        case 2: // level 2
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.arcticTime2 + "s\n" +
                                "Efficiency: " + data.arcticEfficiency2 + "\n" +
                                "Victories: " + data.arcticVictories2 + "\n";
                            break;
                        case 3: // level 3
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.arcticTime3 + "s\n" +
                                "Efficiency: " + data.arcticEfficiency3 + "\n" +
                                "Victories: " + data.arcticVictories3 + "\n";
                            break;
                    }
                    break;

                case 2: // asia
                    switch (level)
                    {
                        case 1: // level 1
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.asiaTime1 + "s\n" +
                                "Efficiency: " + data.asiaEfficiency1 + "\n" +
                                "Victories: " + data.asiaVictories1 + "\n";
                            break;
                        case 2: // level 2
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.asiaTime2 + "s\n" +
                                "Efficiency: " + data.asiaEfficiency2 + "\n" +
                                "Victories: " + data.asiaVictories2 + "\n";
                            break;
                        case 3: // level 3
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.asiaTime3 + "s\n" +
                                "Efficiency: " + data.asiaEfficiency3 + "\n" +
                                "Victories: " + data.asiaVictories3 + "\n";
                            break;
                    }
                    break;

                case 3: // desert
                    switch (level)
                    {
                        case 1: // level 1
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.desertTime1 + "s\n" +
                                "Efficiency: " + data.desertEfficiency1 + "\n" +
                                "Victories: " + data.desertVictories1 + "\n";
                            break;
                        case 2: // level 2
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.desertTime2 + "s\n" +
                                "Efficiency: " + data.desertEfficiency2 + "\n" +
                                "Victories: " + data.desertVictories2 + "\n";
                            break;
                        case 3: // level 3
                            infoText.text = difficultyStars + "\n" +
                                "Best time: " + data.desertTime3 + "s\n" +
                                "Efficiency: " + data.desertEfficiency3 + "\n" +
                                "Victories: " + data.desertVictories3 + "\n";
                            break;
                    }
                    break;
            }
        }
    }
}
