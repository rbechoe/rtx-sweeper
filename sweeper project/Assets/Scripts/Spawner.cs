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

    [HideInInspector]
    public bool isDone;
    
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
                if (Random.Range(0, 10) > 5) newTile.GetComponent<EmptyTile>().isBomb = true;
                // make tiles fall down anim
            }
        }

        isDone = true;
        yield return new WaitForEndOfFrame();
    }
    
    void Update()
    {
        
    }
}
