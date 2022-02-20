using System.Collections;
using TMPro;
using UnityEngine;

public class Tile : Base
{
    [Header("Settings")]
    public Color defaultCol = Color.grey;
    public Color selectCol = Color.green;
    public VFXManipulator vfx;

    private LayerMask flagMask;
    private LayerMask allMask;

    protected int bombCount;
    protected Material gridMat;

    private bool triggered;
    private bool clickable;
    private bool previewClicked;
    private bool canReveal;
    private Collider[] tilesPreviewed;

    // TODO for boss battle tile count gets updates after every click

    protected override void Start()
    {
        base.Start();
        vfx = GetComponentInChildren<VFXManipulator>();
        gridMat = vfx.gridTile.GetComponent<Renderer>().material;
        gridMat.SetColor("_TextureColorTint", defaultCol);
        vfx.gameObject.SetActive(false);

        flagMask = LayerMask.GetMask("Flag");
        allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");
    }

    private void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.START_GAME, Clickable);
        EventSystem.AddListener(EventType.END_GAME, Unclickable);
        EventSystem.AddListener(EventType.WIN_GAME, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, RevealBomb);
    }

    private void OnDisable()
    {
        // unlisten
        EventSystem.RemoveListener(EventType.START_GAME, Clickable);
        EventSystem.RemoveListener(EventType.END_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.WIN_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.RemoveListener(EventType.GAME_LOSE, RevealBomb);
        vfx.gameObject.SetActive(true);
    }

    private void OnMouseOver()
    {
        if (clickable && !triggered)
        {
            gridMat.SetColor("_TextureColorTint", selectCol);
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
                EventSystem.InvokeEvent(EventType.PLAY_CLICK);
                EventSystem.InvokeEvent(EventType.TILE_CLICK);
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
            EventSystem.InvokeEvent(EventType.PLAY_FLAG);
            EventSystem<Vector3[]>.InvokeEvent(EventType.PLANT_FLAG, new Vector3[] { transform.position, transform.eulerAngles });
        }
    }

    private void OnMouseExit()
    {
        // set tile back to base color
        if (clickable && !triggered)
        {
            gridMat.SetColor("_TextureColorTint", defaultCol);
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
        StartCoroutine(FireAction());
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

        defaultCol = Color.grey;
        gridMat.SetColor("_TextureColorTint", defaultCol);

        if (gameObject.CompareTag("Bomb"))
        {
            EventSystem.InvokeEvent(EventType.GAME_LOSE);
        }
        else
        {
            EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
            defaultCol = Color.grey;
            gridMat.SetColor("_TextureColorTint", defaultCol);
            ShowBombAmount();
        }

        // only remove empty tiles that are not bombs
        if (bombCount == 0 && !gameObject.CompareTag("Bomb"))
        {
            Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.25f, Quaternion.identity);
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].GetComponent<Tile>()?.NoBombReveal();
            }
            gridMat.SetColor("_TextureColorTint", new Color(0.25f, 0.5f, 0.5f, 0));
            bombCount = 8;
        }
        else
        {
            gridMat.SetColor("_TextureColorTint", defaultCol);
        }
    }

    public void PreviewTileSelection()
    {
        if (clickable && !triggered)
        {
            gridMat.SetColor("_TextureColorTint", new Color(0.7f, 0.7f, 0.7f));
        }
    }

    public void SetToDefaultCol()
    {
        if (clickable && !triggered)
        {
            gridMat.SetColor("_TextureColorTint", defaultCol);
        }
    }

    private void RevealBomb()
    {
        if (gameObject.CompareTag("Bomb"))
        {
            defaultCol = new Color(0.5f, 0f, 0f);
            gridMat.SetColor("_TextureColorTint", defaultCol);
            Instantiate(vfx.bombEffect, transform.position, Quaternion.identity);
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

    private void ShowBombAmount()
    {
        if (bombCount < 1) return;

        if (!vfx.gameObject.activeSelf) vfx.gameObject.SetActive(true);
        vfx.UpdateEffect(bombCount);
    }

    private void Clickable()
    {
        clickable = true;
    }

    private void Unclickable()
    {
        clickable = false;
    }

    public void FirstTile()
    {
        defaultCol = new Color(0.6f, 1f, 0.8f);
        gridMat.SetColor("_TextureColorTint", defaultCol);
    }
}
