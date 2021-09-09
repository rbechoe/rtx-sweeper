using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : Base
{
    [Header("Settings")] 
    public Color defaultCol = Color.grey;
    public Color selectCol  = Color.green;
    public TMP_Text bombCountTMP;

    protected int bombCount;
    protected Material myMat;

    protected override void Start()
    {
        base.Start();
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
        bombCountTMP = GetComponentInChildren<TMP_Text>();
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

    private void OnMouseDown()
    {
        // do action
    }

    public void SetBombCount(int amount)
    {
        bombCount = amount;
        ShowBombAmount();
    }

    public void ShowBombAmount()
    {
        bombCountTMP.text = "" + bombCount;
    }
}