using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : Base
{
    [Header("Settings")]
    public Color defaultCol = Color.grey;
    public Color selectCol = Color.green;
    public TMP_Text bombCountTMP;

    protected int bombCount;
    protected Material myMat;

    private MeshRenderer meshRenderer;
    private GameManager gameManager;

    private bool triggered;
    private bool clickable;

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
        if (clickable && !triggered)
        {
            myMat.color = selectCol;
            myMat.SetColor("_EmissiveColor", selectCol * 10);
        }
    }

    private void OnMouseExit()
    {
        if (clickable && !triggered)
        {
            myMat.color = defaultCol;
            myMat.SetColor("_EmissiveColor", defaultCol);
        }
    }

    private void OnMouseDown()
    {
        if (clickable)
        {
            DoAction();
        }
    }

    private void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.START_GAME, Clickable);
        EventSystem.AddListener(EventType.END_GAME, Unclickable);
        EventSystem.AddListener(EventType.END_GAME, RevealBomb);

    }

    private void OnDisable()
    {
        // unlisten
        EventSystem.RemoveListener(EventType.START_GAME, Clickable);
        EventSystem.RemoveListener(EventType.END_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.END_GAME, RevealBomb);
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
            Collider[] horCol = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 0.5f) * 1.25f, Quaternion.identity);
            Collider[] verCol = Physics.OverlapBox(gameObject.transform.position, new Vector3(0.5f, 1, 1) * 1.25f, Quaternion.identity);
            for (int i = 0; i < horCol.Length; i++)
            {
                horCol[i].GetComponent<Tile>().NoBombReveal();
            }
            for (int i = 0; i < verCol.Length; i++)
            {
                verCol[i].GetComponent<Tile>().NoBombReveal();
            }
        }
        if (gameObject.CompareTag("Bomb"))
        {
            gameManager.EndGame();
            defaultCol = Color.red;
            myMat.color = defaultCol;
        }
        else
        {
            gameManager.AddGoodTile();
        }

        myMat.SetColor("_EmissiveColor", defaultCol);
    }

    private void RevealBomb()
    {
        if (gameObject.CompareTag("Bomb"))
        {
            DoAction();
        }
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

    private void Clickable()
    {
        clickable = true;
    }

    private void Unclickable()
    {
        clickable = false;
    }
}