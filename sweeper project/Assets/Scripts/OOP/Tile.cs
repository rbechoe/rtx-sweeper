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

    private void Awake()
    {
        bombCountTMP = GetComponentInChildren<TMP_Text>();
    }

    protected override void Start()
    {
        base.Start();
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
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

        // press left button - highlight adjecant tiles that can be revealed if this tile is revealed
        if (Input.GetMouseButtonDown(0))
        {
            // only reveal tile when flags nearby equals amount of bombs nearby
            // do note that if flags are placed incorrect that the user can lose if it reveals a bomb
        }

        // release left button - reveal tile
        if (Input.GetMouseButtonUp(0))
        {
            if (clickable)
            {
                DoAction();
            }
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1))
        {
            // plant flag on top of tile
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

        // only remove empty tiles that are not bombs
        if (bombCount == 0 && !gameObject.CompareTag("Bomb"))
        {
            Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.25f, Quaternion.identity);
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].GetComponent<Tile>()?.NoBombReveal();
            }

            Destroy(gameObject);
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
        DoAction();
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