using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public bool emptyCheckOnly;
    public bool hasBomb, hasEmpty;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Empty"))
        {
            hasEmpty = true;
        }
        if (other.gameObject.CompareTag("bomb"))
        {
            hasBomb = true;
        }
    }
}
