using TMPro;
using UnityEngine;

public class Tile3D : Base
{
    [Header("Settings")]
    public Color defaultCol = Color.grey;
    public Color selectCol = Color.green;
    public Color defaultMid;
    public Color defaultNone;
    public Color defaultSide;
    public TMP_Text bombCountTMP;

    private LayerMask flagMask;
    private LayerMask allMask;

    protected int bombCount;
    protected Material myMat;

    private GameManager gameManager;
    private MeshRenderer meshRenderer;
    private BoxCollider myCol;

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
        defaultMid = new Color(0.25f, 1, 0.25f, 0.5f);
        defaultNone = new Color(0.1f, 0.1f, 0.1f, 0.001f);
        defaultSide = new Color(1, 1, 1, 0.005f);
        meshRenderer = bombCountTMP.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        myCol = gameObject.GetComponent<BoxCollider>();

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        flagMask = LayerMask.GetMask("Flag");
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

    protected override void Update()
    {
        base.Update();

        Collider[] selectorType = Physics.OverlapBox(transform.position, Vector3.one * 0.1f, Quaternion.identity);

        defaultCol = defaultNone;
        SetColor();
        if (!triggered)
        {
            clickable = false;
            myCol.enabled = false;
        }

        if (selectorType.Length > 0)
        {
            foreach(Collider col in selectorType)
            {
                if (col.CompareTag("Transparent"))
                {
                    defaultCol = defaultSide;
                    SetColor();
                    break;
                }
                if (col.CompareTag("Opaque"))
                {
                    defaultCol = defaultMid;
                    SetColor(10);
                    if (!triggered)
                    {
                        clickable = true;
                        myCol.enabled = true;
                    }
                    break;
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (clickable && !triggered)
        {
            myMat.color = selectCol;
            myMat.SetColor("_EmissiveColor", selectCol * 10);
        }

        // press left button - highlight adjecant tiles that can be revealed if this tile is revealed
        if (Input.GetMouseButton(0) && triggered && !previewClicked)
        {
            // use box to detect all nearby tiles that can be activated once amount bombs equals amount of flags, not more or less
            Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.75f, Quaternion.identity, flagMask);
            Collider[] allTiles = Physics.OverlapBox(transform.position, Vector3.one * 0.75f, Quaternion.identity, allMask);

            if (bombCount == nearbyFlags.Length)
            {
                canReveal = true;
            }

            foreach (Collider _tile in allTiles)
            {
                _tile.GetComponent<Tile>()?.PreviewTileSelection();
            }

            tilesPreviewed = allTiles;
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
                    _tile.GetComponent<Tile>()?.DoAction();
                }
                previewClicked = false;
                tilesPreviewed = null;
            }
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1) && !triggered)
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

    public virtual void DoAction()
    {
        if (triggered)
        {
            return;
        }

        // return if there is a flag on this position
        Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
        if (nearbyFlags.Length > 0)
        {
            return;
        }

        print("clicked");

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
            Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.5f, Quaternion.identity);
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
            //myMat.SetColor("_EmissiveColor", new Color(0.7f, 0.7f, 0.7f));
        }
    }

    public void SetToDefaultCol()
    {
        if (clickable && !triggered)
        {
            myMat.SetColor("_EmissiveColor", defaultCol);
        }
    }

    private void SetColor(int _strenght = 1)
    {
        myMat.SetColor("_EmissiveColor", defaultCol * _strenght);
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