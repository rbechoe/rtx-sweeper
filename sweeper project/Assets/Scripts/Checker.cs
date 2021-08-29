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
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO FIX bomb amount etc
        if (isDone)
        {
            if (!other.gameObject.GetComponent<EmptyTile>().isBomb)
            {
                hasEmpty = true;
            }
            if (other.gameObject.GetComponent<EmptyTile>().isBomb)
            {
                hasBomb = true;
                transform.parent.GetComponent<EmptyTile>().bombAmount++;
            }
        }
    }
}
