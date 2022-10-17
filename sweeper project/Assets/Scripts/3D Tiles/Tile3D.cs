using UnityEngine;
using System.Collections;
using UnityEditor;

public class Tile3D : BaseTile
{
    [Header("Settings")]
    public Color defaultNone;

    private TMPBomb3D bombCountTMP;

    protected Material myMat;
    private MeshRenderer myMeshRenderer;
    private BoxCollider myCollider;

    private bool hovered;
    private bool startingTile;

    public Material normalMat, wireframeMat;

    private void Awake()
    {
        bombCountTMP = GetComponentInChildren<TMPBomb3D>();
        myMat = gameObject.GetComponent<Renderer>().material;
        myMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        myCollider = gameObject.GetComponent<BoxCollider>();
        myMat.EnableKeyword("_EMISSION");
        myMat.color = defaultCol;
        myMat.SetColor("_EmissiveColor", defaultCol);
    }

    protected override void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.START_GAME, Clickable);
        EventSystem.AddListener(EventType.END_GAME, Unclickable);
        EventSystem.AddListener(EventType.WIN_GAME, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, RevealBomb);
        EventSystem.AddListener(EventType.END_GAME, ResetSelf);
        EventSystem.AddListener(EventType.PREPARE_GAME, ResetSelf);
        EventSystem.AddListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.AddListener(EventType.WIN_GAME, EndGame);
        EventSystem.AddListener(EventType.GAME_LOSE, EndGame);
        EventSystem.AddListener(EventType.COUNT_BOMBS, CheckBombs);
        EventSystem.AddListener(EventType.IN_SETTINGS, EnteredSettings);
        EventSystem.AddListener(EventType.OUT_SETTINGS, ExitSettings);
    }

    protected override void OnDisable()
    {
        // unlisten
        EventSystem.RemoveListener(EventType.START_GAME, Clickable);
        EventSystem.RemoveListener(EventType.END_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.WIN_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.RemoveListener(EventType.GAME_LOSE, RevealBomb);
        EventSystem.RemoveListener(EventType.END_GAME, ResetSelf);
        EventSystem.RemoveListener(EventType.PREPARE_GAME, ResetSelf);
        EventSystem.RemoveListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.RemoveListener(EventType.WIN_GAME, EndGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, EndGame);
        EventSystem.RemoveListener(EventType.COUNT_BOMBS, CheckBombs);
        EventSystem.RemoveListener(EventType.IN_SETTINGS, EnteredSettings);
        EventSystem.RemoveListener(EventType.OUT_SETTINGS, ExitSettings);
    }

    private void FixedUpdate()
    {
        CheckBombs();
    }

    public override void ResetSelf()
    {
        clickable = true;
        triggered = false;
        hovered = false;
        startingTile = false;
        defaultCol = manager.defaultColor;
        myMeshRenderer.material = normalMat;
        gridMat = myMeshRenderer.material;
        gridMat.EnableKeyword("_EmissiveColor");
        myCollider.enabled = true;
        myMeshRenderer.enabled = true;
        UpdateMaterial(defaultCol);
    }

    private void Update()
    {
        if (!clickable || gameEnded) return;

        UpdateColliders();
        SelectTile();
    }

    private void SelectTile()
    {
        if (triggered || !hovered || inSettings) return;

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
            bool didSomething = false;
            if (clickable)
            {
                EventSystem.InvokeEvent(EventType.PLAY_CLICK);
                EventSystem.InvokeEvent(EventType.TILE_CLICK);
                DoAction();
                didSomething = true;
            }

            // reveal all nearby tiles
            if (previewClicked && canReveal)
            {
                foreach (Collider _tile in tilesPreviewed)
                {
                    _tile.GetComponent<Tile2D>()?.DoAction();
                }
                previewClicked = false;
                tilesPreviewed = null;
                didSomething = true;
            }

            // if no flag on here perform function below
            Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
            if (nearbyFlags.Length == 0)
            {
                if (!didSomething) EventSystem.InvokeEvent(EventType.OTHER_CLICK);
            }
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1) && !triggered)
        {
            // return if there is a flag on this position
            Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
            if (nearbyFlags.Length > 0)
            {
                EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, nearbyFlags[0].gameObject);
                EventSystem.InvokeEvent(EventType.OTHER_CLICK);
            }
            else
            {
                EventSystem.InvokeEvent(EventType.PLAY_FLAG);
                EventSystem<Vector3[]>.InvokeEvent(EventType.PLANT_FLAG, new Vector3[] { transform.position, transform.eulerAngles });
                EventSystem.InvokeEvent(EventType.OTHER_CLICK);
            }
        }
    }

    private void UpdateColliders()
    {
        if (hovered || triggered || startingTile)
        {
            return;
        }

        defaultCol = defaultNone;
        UpdateMaterial(defaultCol);
    }

    public override void FirstTile()
    {
        gameObject.name = "first tile";
        defaultCol = startColor;
        startingTile = true;
        UpdateMaterial(defaultCol, 1024);
    }

    protected override void UpdateMaterial(Color color, float intensity = -10)
    {
        if (intensity == -10) intensity = glowIntensity;

        if (!triggered) gridMat?.SetColor("_EmissiveColor", color * intensity);
        if (triggered) gridMat?.SetColor("_EmissiveColor", color * 0);

        gridMat?.SetColor("_BaseColor", color);
    }

    protected override void RevealBomb()
    {
        hovered = false;
        triggered = false;
        if (state == TileStates.Bomb)
        {
            defaultCol = new Color(0.5f, 0f, 0f, 1);
            UpdateMaterial(defaultCol, 1024);
        }
    }

    public override void UpdateBombAmount(int amount)
    {
        bombCount = amount;
        ShowBombAmount();
    }

    public override void ShowBombAmount()
    {
        if (state != TileStates.Revealed || bombCount == 0)
        {
            bombCountTMP.BombCount(0);
            return;
        }
        bombCountTMP.BombCount(bombCount);
    }

    protected override void OnMouseOver() 
    {
        if (!clickable || gameEnded) return;
        hovered = true;

        UpdateMaterial(selectCol, 1024);
    }

    protected override void OnMouseExit()
    {
        if (!clickable || gameEnded) return;
        hovered = false;

        UpdateMaterial(defaultCol, 1024);
    }

    public override void DoAction()
    {
        StartCoroutine(FireAction());
    }

    public override void TypeSettings()
    {
        defaultNone = new Color(0.1f, 0.1f, 0.1f, 1f);
        gridMat = gameObject.GetComponent<Renderer>().material;
    }

    private IEnumerator FireAction()
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

        triggered = true;

        defaultCol = emptyTileColor;
        UpdateMaterial(defaultCol, 1);

        TypeSpecificAction();
    }

    public override void TypeSpecificAction()
    {
        switch (state)
        {
            case TileStates.Bomb:
                EventSystem.InvokeEvent(EventType.GAME_LOSE);
                break;

            case TileStates.Empty:
                EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
                Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, Vector3.one * 1.25f, Quaternion.identity);
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].GetComponent<BaseTile>()?.NoBombReveal();
                }
                HideTile();
                break;

            case TileStates.Number:
                EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
                ShowBombAmount();
                HideTile();
                break;
        }
    }

    private void HideTile()
    {
        state = TileStates.Revealed;
        if (rewardObj != null) rewardObj.SetActive(true);
        if (breakObj != null) breakObj.SetActive(false);
        myMeshRenderer.material = wireframeMat;
        gridMat = myMeshRenderer.material;
        gridMat.EnableKeyword("_EmissiveColor");
        myCollider.enabled = false;

        if (bombCount == 0)
        {
            myMeshRenderer.enabled = false;
        }
    }
}
