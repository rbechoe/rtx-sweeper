using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossTiles
{
    public class BossSpawner : MonoBehaviour
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

        private void Start()
        {
            xSize = TheCreator.Instance.xSize;
            zSize = TheCreator.Instance.zSize;
            gridSize = TheCreator.Instance.gridSize;
            bombAmount = TheCreator.Instance.bombAmount;
            Vector3 position = new Vector3(xSize / 2f, (xSize + zSize / 2f) * 0.5f, zSize / 2f);
            EventSystem<Vector3>.InvokeEvent(EventType.START_POS, position);
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
                firstTile.GetComponent<BossTile>().FirstTile();
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
            
            foreach (GameObject tile in tiles)
            {
                tile.GetComponent<BossTile>().state = BossTileStates.Empty;
            }
            yield return new WaitForEndOfFrame();

            foreach (GameObject flag in activeFlags)
            {
                EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, flag);
            }

            yield return new WaitForEndOfFrame();
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
