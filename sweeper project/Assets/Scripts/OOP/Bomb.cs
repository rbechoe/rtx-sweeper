using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Tile
{
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "Bomb";
    }
}