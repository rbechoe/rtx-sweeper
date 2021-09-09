using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tile;

    [Header("Settings")] 
    public GameObject mainCam;
    public int gridSize;
    public int bombAmount = 10;

    [Header("Statistics")]
    public bool isDone;
    public int bombCount = 0;
    
    void Start()
    {
        mainCam.transform.position = new Vector3(gridSize / 2f, gridSize, gridSize / 2f - 0.5f);
        Reset();
        StartCoroutine(Grid());
    }

    IEnumerator Grid()
    {
        int curTile = 0;
        int tilesLeft = 0;
        int spawnChance = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // formula: based on tiles and bombs left increase chance for next tile to be bomb
                if (bombCount < bombAmount)
                {
                    tilesLeft = (int)Mathf.Pow(gridSize, 2) - curTile;
                    spawnChance = tilesLeft / (bombAmount - bombCount);
                }    

                GameObject newTile = Instantiate(tile, new Vector3(x, 0, z), Quaternion.identity);
                if (bombCount < bombAmount && Random.Range(0, spawnChance) == 0)
                {
                    newTile.AddComponent<Bomb>();
                    bombCount++;
                }
                else
                {
                    newTile.AddComponent<Empty>();
                }

                curTile++;
                yield return new WaitForEndOfFrame();
            }
        }

        isDone = true;
    }

    private void Reset()
    {
        isDone = false;
        bombCount = 0;
    }

    private void Update()
    {
        
    }
}
