using TMPro;
using UnityEngine;

public class Tile : Base
{
    [Header("Settings")]
    public Color defaultCol = Color.grey;
    public Color selectCol = Color.green;
    public TMP_Text bombCountTMP;

    private LayerMask bombMask;
    private LayerMask flagMask;
    private LayerMask tileMask;
    private LayerMask allMask;

    protected int bombCount;
    protected Material myMat;

    private MeshRenderer meshRenderer;
    private GameManager gameManager;

    private bool triggered;
    private bool clickable;
    private bool previewClicked;
    private bool canReveal;
    private Collider[] tilesPreviewed;

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

        bombMask = LayerMask.GetMask("Bomb");
        flagMask = LayerMask.GetMask("Flag");
        tileMask = LayerMask.GetMask("Empty");
        allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");
    }

    private void OnEnable()
    {
        // listen
        EventSystem<Parameters>.AddListener(EventType.START_GAME, Clickable);
        EventSystem<Parameters>.AddListener(EventType.END_GAME, Unclickable);
        EventSystem<Parameters>.AddListener(EventType.END_GAME, RevealBomb);
    }

    private void OnDisable()
    {
        // unlisten
        EventSystem<Parameters>.RemoveListener(EventType.START_GAME, Clickable);
        EventSystem<Parameters>.RemoveListener(EventType.END_GAME, Unclickable);
        EventSystem<Parameters>.RemoveListener(EventType.END_GAME, RevealBomb);
    }

    private void OnMouseOver()
    {
        if (clickable && !triggered)
        {
            myMat.color = selectCol;
            myMat.SetColor("_EmissiveColor", selectCol * 10);
        }

        // press left button - highlight adjecant tiles that can be revealed if this tile is revealed
        if (Input.GetMouseButtonDown(0) && triggered)
        {
            // use box to detect all nearby tiles that can be activated once amount bombs equals amount of flags, not more or less
            Collider[] nearbyBombs = Physics.OverlapBox(transform.position, Vector3.one * 0.75f, Quaternion.identity, bombMask);
            Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.75f, Quaternion.identity, flagMask);
            Collider[] nearbyTiles = Physics.OverlapBox(transform.position, Vector3.one * 0.75f, Quaternion.identity, tileMask);
            Collider[] allTiles = Physics.OverlapBox(transform.position, Vector3.one * 0.75f, Quaternion.identity, allMask);

            if (bombCount == nearbyFlags.Length)
            {
                canReveal = true;
            }

            if (canReveal)
            {
                foreach (Collider _tile in nearbyTiles)
                {
                    _tile.GetComponent<Tile>()?.PreviewTileSelection();
                    tilesPreviewed = nearbyTiles;
                }
            }
            else
            {
                foreach (Collider _tile in allTiles)
                {
                    _tile.GetComponent<Tile>()?.PreviewTileSelection();
                    tilesPreviewed = allTiles;
                }
            }

            previewClicked = true;
        }

        // release left button - reveal tile
        if (Input.GetMouseButtonUp(0))
        {
            if (clickable)
            {
                DoAction();
            }

            // reveal all nearby tiles
            if (previewClicked && canReveal)
            {
                foreach (Collider _tile in tilesPreviewed)
                {
                    _tile.GetComponent<Tile>().DoAction();
                }
                previewClicked = false;
                tilesPreviewed = null;
            }
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1))
        {
            Parameters param = new Parameters();
            param.vector3s.Add(transform.position);
            EventSystem<Parameters>.InvokeEvent(EventType.PLANT_FLAG, param);
        }
    }

    private void OnMouseExit()
    {
        // set tile back to base color
        if (clickable && !triggered)
        {
            myMat.color = defaultCol;
            myMat.SetColor("_EmissiveColor", defaultCol);
        }

        // set all nearby tiles back to base color
        if (previewClicked)
        {
            foreach (Collider _tile in tilesPreviewed)
            {
                _tile.GetComponent<Tile>()?.SetToDefaultCol();
            }
            previewClicked = false;
            tilesPreviewed = null;
        }
    }

    public void DoAction()
    {
        if (triggered)
        {
            return;
        }

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
            meshRenderer.enabled = true;
            defaultCol = Color.black;
            myMat.color = defaultCol;
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

    public void PreviewTileSelection()
    {
        if (clickable && !triggered)
        {
            myMat.SetColor("_EmissiveColor", new Color(0.7f, 0.7f, 0.7f));
        }
    }

    public void SetToDefaultCol()
    {
        if (clickable && !triggered)
        {
            myMat.SetColor("_EmissiveColor", defaultCol);
        }
    }

    private void RevealBomb(object value)
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

    private void Clickable(object value)
    {
        clickable = true;
    }

    private void Unclickable(object value)
    {
        clickable = false;
    }

    public void FirstTile()
    {
        defaultCol = new Color(0.9f, 0.1f, 0.7f);
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }
}