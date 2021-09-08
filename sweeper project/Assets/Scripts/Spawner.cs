using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject emptyTile;
    public GameObject bombTile;

    [Header("Settings")] 
    public GameObject mainCam;
    public int gridSize;
    public int bombs = 10;

    [Header("Statistics")]
    public bool isDone;
    public int bombCount = 0;
    
    void Start()
    {
        mainCam.transform.position = new Vector3(gridSize / 2f, gridSize, gridSize / 2f - 0.5f);
        StartCoroutine(Grid());
    }

    IEnumerator Grid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject newTile = Instantiate(emptyTile, new Vector3(x, 0, z), Quaternion.identity);
                if (Random.Range(0, 10) > 5 && bombCount < bombs)
                {
                    newTile.GetComponent<GridTile>().isBomb = true;
                    bombCount++;
                }
                // make tiles fall down anim or add rigidbodies on Y-axis
            }
        }

        isDone = true;
        yield return new WaitForEndOfFrame();
    }
    
    void Update()
    {
        
    }
}
