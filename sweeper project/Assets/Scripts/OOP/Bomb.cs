using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Tile
{
    protected override void Start()
    {
        base.Start();
        defaultCol = Color.black;
        selectCol = Color.blue;
        myMat.SetColor("_EmissiveColor", defaultCol);
        gameObject.tag = "Bomb";
    }
}