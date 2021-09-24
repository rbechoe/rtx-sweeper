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
    private LayerMask selectionLayers;

    protected int bombCount;
    protected Material myMat;

    private GameManager gameManager;
    private MeshRenderer meshRenderer;
    private BoxCollider myCol;

    public bool triggered;
    public bool clickable;
    public bool previewClicked;
    public bool canReveal;
    public bool hovered;
    private bool startingTile;
    private Collider[] tilesPreviewed;

    private void Awake()
    {
        bombCountTMP = GetComponentInChildren<TMP_Text>();
    }

    protected override void Start()
    {
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
        defaultMid = new Color(0.25f, 1, 0.25f, 0.5f);
        defaultNone = new Color(0.1f, 0.1f, 0.1f, 0.01f);
        defaultSide = new Color(1, 1, 1, 0.1f);
        meshRenderer = bombCountTMP.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        myCol = gameObject.GetComponent<BoxCollider>();

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        flagMask = LayerMask.GetMask("Flag");
        allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");
        selectionLayers = LayerMask.GetMask("Selector", "Transparent");
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
        if (clickable)
        {
            UpdateColliders();
            SelectTile();
        }
    }

    private void SelectTile()
    {
        if (hovered)
        {
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
                DoAction();

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
    }

    // update base colors based on layer object currently is in
    private void UpdateColliders()
    {
        if (hovered || triggered || startingTile)
        {
            return;
        }

        Collider[] selectorType = Physics.OverlapBox(transform.position, Vector3.one * 0.1f, Quaternion.identity, selectionLayers);

        if (selectorType.Length > 0)
        {
            foreach (Collider col in selectorType)
            {
                if (col.CompareTag("Transparent"))
                {
                    defaultCol = defaultSide;
                    bombCountTMP.color = defaultCol;
                    SetColor();
                    myCol.enabled = false;
                    break;
                }

                if (col.CompareTag("Opaque"))
                {
                    defaultCol = defaultMid;
                    bombCountTMP.color = defaultCol;
                    SetColor(2);
                    myCol.enabled = true;
                    break;
                }
            }
        }
        else
        {
            defaultCol = defaultNone;
            bombCountTMP.color = defaultCol;
            SetColor();
            myCol.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Picker") && clickable)
        {
            hovered = true;

            // set tile back to base color
            myMat.color = selectCol;
            myMat.SetColor("_EmissiveColor", selectCol * 10);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Picker") && clickable)
        {
            hovered = false;

            // set tile back to base color
            myMat.color = defaultCol;
            myMat.SetColor("_EmissiveColor", defaultCol);
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

        triggered = true;

        defaultCol = Color.black;
        myMat.color = defaultCol;
        SetColor();

        if (gameObject.CompareTag("Bomb"))
        {
            gameManager.EndGame();
            defaultCol = Color.red;
            myMat.color = defaultCol;
            SetColor();
        }
        else
        {
            gameManager.AddGoodTile3D();
            meshRenderer.enabled = true;
            defaultCol = Color.black;
            myMat.color = defaultCol;
            SetColor();
        }

        // only remove empty tiles that are not bombs
        if (bombCount == 0 && !gameObject.CompareTag("Bomb"))
        {
            Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.5f, Quaternion.identity);
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].GetComponent<Tile3D>()?.NoBombReveal();
            }

            Destroy(gameObject);
        }

        myMat.SetColor("_EmissiveColor", defaultCol);
    }

    public void SetToDefaultCol()
    {
        if (clickable && !triggered)
        {
            myMat.color = defaultCol;
            myMat.SetColor("_EmissiveColor", defaultCol);
        }
    }

    private void SetColor(int _strenght = 1)
    {
        myMat.color = defaultCol;
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
        startingTile = true;
        defaultCol = new Color(0.9f, 0.1f, 0.7f);
        myMat.color = defaultCol;
        SetColor(2);
    }
}