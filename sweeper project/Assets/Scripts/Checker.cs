using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public bool emptyCheckOnly;
    public bool hasBomb, hasEmpty;

    private Spawner spawner;
    private bool isDone;
    
    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("GameController").GetComponent<Spawner>();
    }
    
    void Update()
    {
        isDone = spawner.isDone;

        if (isDone)
        {
            // force bomb check
            // update bomb count in parent
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // TODO FIX bomb amount etc, force update check
        if (isDone)
        {
            if (!other.gameObject.GetComponent<GridTile>().isBomb && !hasEmpty)
            {
                hasEmpty = true;
            }
            if (other.gameObject.GetComponent<GridTile>().isBomb && !hasBomb)
            {
                hasBomb = true;
                transform.parent.GetComponent<GridTile>().bombAmount++;
            }
        }
    }
}
