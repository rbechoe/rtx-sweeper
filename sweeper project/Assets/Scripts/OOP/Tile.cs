using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : Base
{
    private Material myMat;

    [Header("Settings")] 
    public Color defaultCol;
    public Color selectCol;
    
    private void Start()
    {
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }
    
    private void OnMouseOver()
    {
        myMat.color = selectCol;
        myMat.SetColor("_EmissiveColor", selectCol * 10);
    }

    private void OnMouseExit()
    {
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }

    public virtual void OnMouseDown()
    {
        // do action
    }
}
