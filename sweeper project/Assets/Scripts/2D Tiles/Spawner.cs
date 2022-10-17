using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tiles2D
{
    public class Spawner : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject tile;
        public GameObject flag;

        [Header("Statistics")]
        public bool isDone;
        public int bombCount = 0;

        private int gridSize;
        private int bombAmount = 10;
        private int xSize = 0;
        private int zSize = 0;
        private int bombs = 0;

        private List<GameObject> tiles = new List<GameObject>();
        private List<GameObject> activeFlags = new List<GameObject>();
        private List<GameObject> inactiveFlags = new List<GameObject>();

        private GameObject firstTile;
        private List<GameObject> emptyTiles = new List<GameObject>();
        private bool inReset = false;
        private bool resetDone = false;

        [Header("Assignables")]
        public GameObject managerObj;
        public GameObject flagObj;
        public GameObject generateUI;
        public InputField widthText, lengthText, bombText;
        private GridManager2DCustom gridManager;

        private void Start()
        {
            Vector3 position = new Vector3(xSize / 2f, (xSize + zSize / 2f) * 0.5f, zSize / 2f);
            EventSystem<Vector3>.InvokeEvent(EventType.START_POS, position);
            gridManager = managerObj.GetComponent<GridManager2DCustom>();
        }

        private void OnEnable()
        {
            EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, ActivateFlag);
            EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, ReturnFlag);
            EventSystem.AddListener(EventType.RESET_GAME, ResetGame);
            EventSystem<Vector3[]>.AddListener(EventType.PLANT_FLAG, AddFlag);
            EventSystem<GameObject>.AddListener(EventType.REMOVE_FLAG, RemoveFlag);
            EventSystem.AddListener(EventType.PICK_TILE, PickStartingTile);
            EventSystem<GameObject>.AddListener(EventType.ADD_EMPTY, AddEmptyTile);
        }

        private void OnDisable()
        {
            EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, ActivateFlag);
            EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, ReturnFlag);
            EventSystem.RemoveListener(EventType.RESET_GAME, ResetGame);
            EventSystem<Vector3[]>.RemoveListener(EventType.PLANT_FLAG, AddFlag);
            EventSystem<GameObject>.RemoveListener(EventType.REMOVE_FLAG, RemoveFlag);
            EventSystem.RemoveListener(EventType.PICK_TILE, PickStartingTile);
            EventSystem<GameObject>.RemoveListener(EventType.ADD_EMPTY, AddEmptyTile);
        }

        public void CreateGrid()
        {
            bombAmount = int.Parse(bombText.text);
            xSize = int.Parse(widthText.text);
            zSize = int.Parse(lengthText.text);
            StartCoroutine(Grid());
        }

        private IEnumerator Grid()
        {
            int curTile = 0;
            int tilesLeft = 0;
            int spawnChance = 0;
            gridSize = xSize * zSize;
            bombs = bombAmount;
            GameObject newTile = null;

            ResetGameNoUI();
            while(!resetDone)
            {
                yield return new WaitForEndOfFrame();
            }

            int tilesPerFrame = SystemInfo.processorCount * 4; // spawn more tiles based on core count
            int curTileCount = 0;
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    // formula: based on tiles and bombs left increase chance for next tile to be bomb
                    if (bombCount < bombAmount)
                    {
                        tilesLeft = gridSize - curTile;
                        spawnChance = tilesLeft / (bombAmount - bombCount);
                    }

                    newTile = Instantiate(tile, new Vector3(x, 0, z), Quaternion.identity);
                    if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                    {
                        newTile.GetComponent<Tile2D>().state = TileStates.Bomb;
                        bombCount++;

                        // create flag for the pool, 1 flag per bomb
                        if (inactiveFlags.Count < curTile)
                        {
                            AddNewFlag();
                        }
                    }
                    else
                    {
                        newTile.GetComponent<Tile2D>().state = TileStates.Empty;
                    }

                    curTile++;
                    curTileCount++;
                    newTile.name = "tile " + curTile;
                    newTile.transform.parent = managerObj.transform;
                    tiles.Add(newTile);

                    // continue next frame
                    if (curTileCount >= tilesPerFrame)
                    {
                        curTileCount = 0;
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            yield return new WaitForEndOfFrame();
            managerObj.transform.position = new Vector3(-xSize / 2f, 0, -zSize / 2f);
            gridManager.SetTiles(tiles);

            isDone = true;
            EventSystem.InvokeEvent(EventType.PREPARE_GAME);
            yield return new WaitForEndOfFrame();
            StartGame();
            yield return new WaitForEndOfFrame();
        }

        // add flag to pool
        private void AddNewFlag()
        {
            GameObject _flag = Instantiate(flag, Vector3.up * 5000, Quaternion.identity);
            _flag.transform.parent = flagObj.transform;
            inactiveFlags.Add(_flag);
        }

        // activate a flag and place it above the tile
        private void ActivateFlag(Vector3[] vectors)
        {
            if (inactiveFlags.Count > 0 && bombs > 0)
            {
                inactiveFlags[0].transform.position = vectors[0];
                inactiveFlags[0].transform.eulerAngles = vectors[1];
                activeFlags.Add(inactiveFlags[0]);
                inactiveFlags.RemoveAt(0);
            }
        }

        // remove a flag from the tile
        public void ReturnFlag(GameObject flag)
        {
            flag.transform.position = Vector3.up * 5000;
            activeFlags.Remove(flag);
            inactiveFlags.Add(flag);
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
                firstTile.GetComponent<Tile2D>().FirstTile();
            }
        }

        protected virtual void StartGame()
        {
            EventSystem.InvokeEvent(EventType.COUNT_BOMBS);
            PickStartingTile();
            EventSystem.InvokeEvent(EventType.START_GAME);
            EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
            inReset = false;
        }

        private void ResetGame()
        {
            if (inReset) return;

            inReset = true;
            isDone = false;
            resetDone = false;
            bombCount = 0;
            firstTile = null;
            StartCoroutine(ResetLogic(true));
        }

        public void ResetGameNoUI()
        {
            if (inReset) return;

            inReset = true;
            isDone = false;
            resetDone = false;
            bombCount = 0;
            firstTile = null;
            StartCoroutine(ResetLogic(false));
        }

        // super efficient system......not
        IEnumerator ResetLogic(bool enableUI = true)
        {
            emptyTiles = new List<GameObject>();
            EventSystem.InvokeEvent(EventType.END_GAME);

            // remove all flags and tiles
            foreach (GameObject tile in tiles)
            {
                Destroy(tile);
            }
            tiles = new List<GameObject>();
            yield return new WaitForEndOfFrame();

            foreach (GameObject flag in activeFlags)
            {
                Destroy(flag);
            }
            activeFlags = new List<GameObject>();
            yield return new WaitForEndOfFrame();

            foreach (GameObject flag in inactiveFlags)
            {
                Destroy(flag);
            }
            inactiveFlags = new List<GameObject>();
            yield return new WaitForEndOfFrame();

            resetDone = true;

            // Enable UI
            if (enableUI) generateUI.SetActive(true);
        }

        private void AddFlag(Vector3[] vectors)
        {
            bombs--;
            EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombs);
        }

        private void RemoveFlag(GameObject flag)
        {
            bombs++;
            EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombs);
        }
    }
}
