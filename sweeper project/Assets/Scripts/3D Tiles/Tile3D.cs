using UnityEngine;
using TMPro;
using System.Collections;

public class Tile3D : BaseTile
{
    [Header("Settings")]
    public Color defaultNone;

    private LayerMask selectionLayers;
    private TMP_Text bombCountTMP;

    protected Material myMat;

    private GameManager gameManager;
    private MeshRenderer meshRenderer;

    private bool hovered;
    private bool startingTile;

    private void Awake()
    {
        bombCountTMP = GetComponentInChildren<TMP_Text>();
        myMat = gameObject.GetComponent<Renderer>().material;
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }

    protected override void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.START_GAME, Clickable);
        EventSystem.AddListener(EventType.END_GAME, Unclickable);
        EventSystem.AddListener(EventType.END_GAME, RevealBomb);
    }

    protected override void OnDisable()
    {
        // unlisten
        EventSystem.RemoveListener(EventType.START_GAME, Clickable);
        EventSystem.RemoveListener(EventType.END_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.END_GAME, RevealBomb);
    }

    private void Update()
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
                    _tile.GetComponent<Tile2D>()?.PreviewTileSelection();
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
                        _tile.GetComponent<Tile2D>()?.DoAction();
                    }
                    previewClicked = false;
                    tilesPreviewed = null;
                }
            }

            // right click - place flag
            if (Input.GetMouseButtonUp(1) && !triggered)
            {
                EventSystem<Vector3[]>.InvokeEvent(EventType.PLANT_FLAG, new Vector3[] { transform.position, transform.eulerAngles });
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
                    defaultCol = new Color(selectCol.r, selectCol.g, selectCol.b, 0.1f);
                    UpdateMaterial(defaultCol);
                    break;
                }

                if (col.CompareTag("Opaque"))
                {
                    defaultCol = selectCol;
                    UpdateMaterial(defaultCol);
                    break;
                }
            }
        }
        else
        {
            defaultCol = defaultNone;
            UpdateMaterial(defaultCol);
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

    private void SetColor(int _strenght = 1)
    {
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol * _strenght);
    }

    public override void ShowBombAmount()
    {
        bombCountTMP.text = "" + bombCount;
    }

    protected override void OnMouseOver()
    {

    }

    protected override void OnMouseExit()
    {

    }

    public override void DoAction(bool sequenced = false)
    {
        StartCoroutine(FireAction(sequenced));
    }

    public override void TypeSettings()
    {
        selectionLayers = LayerMask.GetMask("Selector", "Transparent");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        defaultNone = new Color(0.1f, 0.1f, 0.1f, 0.01f);
        meshRenderer = bombCountTMP.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;

        gridMat = gameObject.GetComponent<Renderer>().material;
    }

    private IEnumerator FireAction(bool sequenced = false)
    {
        yield return new WaitForEndOfFrame();

        if (triggered)
        {
            yield break;
        }

        // return if there is a flag on this position
        Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
        if (nearbyFlags.Length > 0)
        {
            yield break;
        }

        if (sequenced) EventSystem.InvokeEvent(EventType.REVEAL_TILE);

        triggered = true;

        defaultCol = emptyTileColor;
        UpdateMaterial(defaultCol, 1);

        TypeSpecificAction();
    }
}
