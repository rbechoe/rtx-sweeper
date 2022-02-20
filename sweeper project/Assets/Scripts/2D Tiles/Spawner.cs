using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles2D
{
    public class Spawner : Base
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

        protected override void Start()
        {
            base.Start();
            xSize = TheCreator.Instance.xSize;
            zSize = TheCreator.Instance.zSize;
            gridSize = TheCreator.Instance.gridSize;
            bombAmount = TheCreator.Instance.bombAmount;
            Vector3 position = new Vector3(xSize / 2f, (xSize + zSize / 2f) * 0.5f, zSize / 2f);
            EventSystem<Vector3>.InvokeEvent(EventType.START_POS, position);
            CreateGrid(xSize, zSize, bombAmount);
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

        public void CreateGrid(int _x, int _z, int _bombAmount)
        {
            bombAmount = _bombAmount;
            xSize = _x;
            zSize = _z;
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
                        newTile.AddComponent<BombTile>();
                        bombCount++;

                        // create flag for the pool, 1 flag per bomb
                        if (inactiveFlags.Count < curTile)
                        {
                            AddNewFlag();
                        }
                    }
                    else
                    {
                        newTile.AddComponent<EmptyTile>();
                    }

                    curTile++;
                    curTileCount++;
                    newTile.name = "tile " + curTile;
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

            isDone = true;
            EventSystem.InvokeEvent(EventType.PREPARE_GAME);
            yield return new WaitForEndOfFrame();
        }

        // add flag to pool
        private void AddNewFlag()
        {
            GameObject _flag = Instantiate(flag, Vector3.up * 5000, Quaternion.identity);
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
                firstTile.GetComponent<Tile>().FirstTile();
            }
        }

        private void ResetGame()
        {
            isDone = false;
            bombCount = 0;
            firstTile = null;
            StartCoroutine(ResetLogic());
        }

        // super efficient system......not
        IEnumerator ResetLogic()
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

            // start spawner with current settings
            StartCoroutine(Grid());
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
