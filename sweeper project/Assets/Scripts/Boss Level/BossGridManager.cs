using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BossTiles
{
    public class BossGridManager : MonoBehaviour
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

        private LayerMask flagMask;
        private LayerMask bombMask;

        private bool timeStarted;
        private bool inReset;
        private bool wonGame;
        private int tileClicks;
        private float timer;

        private DataSerializer DS;

        public TextMeshProUGUI infoText;

        [Header("Level specific")]
        public Color emptyTileColor = Color.black;
        public Color startColor = Color.blue;
        public Color defaultColor = Color.grey;
        public Color selectColor = Color.green;

        private string difficultyStars;
        private bool firstTime = true; // used to avoid bug, clean solution needs to be fixed!

        private void Start()
        {
            flagMask = LayerMask.GetMask("Flag");
            bombMask = LayerMask.GetMask("Bomb");

            difficultyStars = "Difficulty: ***"; // base +3 due to boss stage
            for (int i = 0; i < (10 - bombDensity); i++)
            {
                difficultyStars += "*";
            }

            foreach (Transform child in transform)
            {
                if (child.GetComponent<BossChecker>())
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

        private void Update()
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
            EventSystem.AddListener(EventType.PLAY_CLICK, ShuffleGrid);
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
            EventSystem.RemoveListener(EventType.TILE_CLICK, ShuffleGrid);
            EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, TileClick);
            EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, FlagClick);
        }

        private void ShuffleGrid()
        {
            if (tileClicks <= 1) return;

            StartCoroutine(ShuffleBombs());
        }

        private IEnumerator ShuffleBombs()
        {
            int curTile = 0;
            int tilesLeft = 0;
            int spawnChance = 0;
            int bombCount = 0;
            int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
            int curTileCount = 0;

            EventSystem.InvokeEvent(EventType.UNPLAYABLE);

            yield return new WaitForSeconds(1);
            
            // check how many bombs have been flagged
            for (int tileId = 0; tileId < tiles.Count; tileId++)
            {
                GameObject newTile = tiles[tileId];
                if (newTile.GetComponent<BossTile>().state == BossTileStates.Bomb)
                {
                    // if flagged increase bombcount, meaning we have to shuffle less bombs
                    Collider[] nearbyFlags = Physics.OverlapBox(newTile.transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
                    if (nearbyFlags.Length > 0) bombCount++;
                }
            }

            // rearrange grid
            List<GameObject> rearrangables = tiles;

            // assign bombs
            for (int tileId = 0; tileId < tiles.Count; tileId++)
            {
                GameObject newTile = tiles[tileId];

                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    tilesLeft = tiles.Count - curTile;
                    spawnChance = tilesLeft / (bombAmount - bombCount);
                }

                // skip if flag is on top of tile
                Collider[] nearbyFlags = Physics.OverlapBox(newTile.transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
                if (nearbyFlags.Length > 0) continue;

                BossTile tileData = newTile.GetComponent<BossTile>();

                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.tag = "Bomb";
                    newTile.layer = 11;
                    tileData.state = BossTileStates.Bomb;
                    bombCount++;
                    rearrangables.Remove(newTile);
                }

                curTile++;
                curTileCount++;

                // continue next frame
                if (curTileCount >= tilesPerFrame)
                {
                    curTileCount = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
            
            // assign empty and number tiles, can only be done after all bombs has been placed
            for (int tileId = 0; tileId < rearrangables.Count; tileId++)
            {
                GameObject newTile = tiles[tileId];
                BossTile tileData = newTile.GetComponent<BossTile>();
                newTile.tag = "Empty";
                newTile.layer = 12;

                // check if there are bombs nearby
                Collider[] nearbyBombs = Physics.OverlapBox(newTile.transform.position, Vector3.one * 0.25f, Quaternion.identity, bombMask);
                if (nearbyBombs.Length > 0)
                {
                    // number
                    tileData.state = BossTileStates.Number;
                    tileData.UpdateBombAmount(nearbyBombs.Length);
                }
                else
                {
                    // empty
                    tileData.state = BossTileStates.Empty;
                    tileData.UpdateBombAmount(0);
                }

                curTile++;
                curTileCount++;

                // continue next frame
                if (curTileCount >= tilesPerFrame)
                {
                    curTileCount = 0;
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForEndOfFrame();
            EventSystem.InvokeEvent(EventType.PLAYABLE);
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
                    newTile.GetComponent<BossTile>().state = BossTileStates.Bomb;
                    bombCount++;
                }
                else
                {
                    newTile.tag = "Empty";
                    newTile.layer = 12;
                    newTile.GetComponent<BossTile>().state = BossTileStates.Empty;
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
                firstTile.GetComponent<BossTile>().FirstTile();
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
            
            AD.bossVictories = (wonGame) ? AD.bossVictories + 1 : AD.bossVictories;
            AD.bossTotalClicks += tileClicks;
            AD.bossGamesPlayed += 1;

            if (wonGame)
            {
                AD.bossVictories1 += 1;

                if (timer < AD.bossTime1 || (timer == AD.bossTime1 && efficiency > AD.bossEfficiency1) || AD.bossTime1 == 0)
                {
                    AD.bossTime1 = timer;
                    AD.bossEfficiency1 = (int)efficiency;
                    AD.bossClicks1 = tileClicks;
                }
            }

            DS.UpdateAccountData(AD);
            SetText();
            
            wonGame = false;
        }

        private void SetText(AccountData data = null)
        {
            if (data == null) data = DS.GetUserData();
            
            infoText.text = difficultyStars + "\n" +
                "Best time: " + data.bossTime1 + "s\n" +
                "Efficiency: " + data.bossEfficiency1 + "\n" +
                "Victories: " + data.bossVictories1 + "\n";
        }
    }
}
