using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    private Material myMat;

    [Header("Settings")] 
    public Color defaultCol;
    public Color selectCol;
    public TextMeshPro bombCount;

    [HideInInspector]
    public int bombAmount;
    public bool isBomb;
    
    void Start()
    {
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }
    
    void Update()
    {
        bombCount.text = "" + bombAmount;
    }
    
    void OnMouseOver()
    {
        myMat.color = selectCol;
        myMat.SetColor("_EmissiveColor", selectCol * 10);
    }

    private void OnMouseExit()
    {
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }
}