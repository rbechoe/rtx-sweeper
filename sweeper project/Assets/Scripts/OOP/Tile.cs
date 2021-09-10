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

    private MeshRenderer meshRenderer;
    private GameManager gameManager;
    private Collider[] neighbourTiles;

    bool triggered;

    protected override void Start()
    {
        base.Start();
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
        bombCountTMP = GetComponentInChildren<TMP_Text>();
        meshRenderer = bombCountTMP.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
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
        DoAction();
    }

    public void DoAction()
    {
        if (triggered) return;

        meshRenderer.enabled = true;
        triggered = true;

        defaultCol = Color.black;
        myMat.color = defaultCol;

        if (bombCount == 0)
        {
            for (int i = 0; i < neighbourTiles.Length; i++)
            {
                neighbourTiles[i].GetComponent<Tile>().NoBombReveal();
            }
        }
        if (gameObject.CompareTag("Bomb"))
        {
            print("Triggered a bomb!");
            defaultCol = Color.red;
            myMat.color = defaultCol;
        }
        else
        {
            gameManager.AddGoodTile();
        }

        myMat.SetColor("_EmissiveColor", defaultCol);
    }

    public void NoBombReveal()
    {
        if (bombCount == 0)
        {
            DoAction();
        }
    }

    public void SetBombCount(int amount)
    {
        bombCount = amount;
    }

    public void SetGameManager(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }

    public void ShowBombAmount()
    {
        bombCountTMP.text = "" + bombCount;
    }

    public void SetNeighbourTiles(Collider[] _neighbourTiles)
    {
        neighbourTiles = _neighbourTiles;
    }
}