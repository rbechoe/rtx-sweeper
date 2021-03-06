using System.Collections;
using UnityEngine;

public class Tile2D : BaseTile
{
    private MeshRenderer myMesh;
    protected override void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.START_GAME, Clickable);
        EventSystem.AddListener(EventType.END_GAME, Unclickable);
        EventSystem.AddListener(EventType.WIN_GAME, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, RevealBomb);
        EventSystem.AddListener(EventType.END_GAME, ResetSelf);
        EventSystem.AddListener(EventType.END_GAME, EnableMesh);
        EventSystem.AddListener(EventType.PREPARE_GAME, ResetSelf);
        EventSystem.AddListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.AddListener(EventType.WIN_GAME, EndGame);
        EventSystem.AddListener(EventType.GAME_LOSE, EndGame);
        EventSystem.AddListener(EventType.COUNT_BOMBS, CheckBombs);
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
        EventSystem.RemoveListener(EventType.END_GAME, EnableMesh);
        EventSystem.RemoveListener(EventType.PREPARE_GAME, ResetSelf);
        EventSystem.RemoveListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.RemoveListener(EventType.WIN_GAME, EndGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, EndGame);
        EventSystem.RemoveListener(EventType.COUNT_BOMBS, CheckBombs);
        vfx.gameObject.SetActive(true);
    }

    protected override void OnMouseOver()
    {
        UpdateMaterial(selectCol);

        if (gameEnded || (bombCount == 0 && triggered))
        {
            UpdateMaterial(defaultCol);
            return;
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
            else
            {
                canReveal = false;
            }

            foreach (Collider tile in allTiles)
            {
                tile.GetComponent<Tile2D>()?.PreviewTileSelection();
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
                foreach (Collider tile in tilesPreviewed)
                {
                    tile.GetComponent<Tile2D>()?.DoAction();
                }
                previewClicked = false;
                tilesPreviewed = null;
                didSomething = true;
            }

            if (!didSomething) EventSystem.InvokeEvent(EventType.OTHER_CLICK);
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1) && !triggered)
        {
            EventSystem.InvokeEvent(EventType.PLAY_FLAG);
            EventSystem<Vector3[]>.InvokeEvent(EventType.PLANT_FLAG, new Vector3[] { transform.position, transform.eulerAngles });
            EventSystem.InvokeEvent(EventType.OTHER_CLICK);
        }
    }

    protected override void UpdateMaterial(Color color, float intensity = -10)
    {
        if (intensity == -10) intensity = glowIntensity;
            
        if (!triggered) gridMat?.SetColor("_EmissiveColor", color * intensity);
        if (triggered) gridMat?.SetColor("_EmissiveColor", color * 0);

        gridMat?.SetColor("_BaseColor", color);
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

        if (!gameObject.CompareTag("Bomb"))
            myMesh.enabled = false;

        TypeSpecificAction();
    }

    private void EnableMesh()
    {
        myMesh.enabled = true;
    }

    public override void DoAction()
    {
        StartCoroutine(FireAction());
    }

    public override void TypeSettings() 
    {
        myMesh = vfx.gridTile.GetComponent<MeshRenderer>();
    }
}
