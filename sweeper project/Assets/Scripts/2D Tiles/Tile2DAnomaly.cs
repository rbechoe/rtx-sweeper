using System.Collections;
using UnityEngine;

public class Tile2DAnomaly : BaseTile
{
    private MeshRenderer myMesh;
    private bool clicked;

    protected override void OnEnable()
    {
        // listen
        EventSystem.eventCollection[EventType.START_GAME] += Clickable;
        EventSystem.eventCollection[EventType.END_GAME] += Unclickable;
        EventSystem.eventCollection[EventType.WIN_GAME] += Unclickable;
        EventSystem.eventCollection[EventType.GAME_LOSE] += Unclickable;
        EventSystem.eventCollection[EventType.GAME_LOSE] += RevealBomb;
        EventSystem.eventCollection[EventType.END_GAME] += ResetSelf;
        EventSystem.eventCollection[EventType.END_GAME] += EnableMesh;
        EventSystem.eventCollection[EventType.PREPARE_GAME] += ResetSelf;
        EventSystem.eventCollection[EventType.PREPARE_GAME] += StartGame;
        EventSystem.eventCollection[EventType.WIN_GAME] += EndGame;
        EventSystem.eventCollection[EventType.GAME_LOSE] += EndGame;
        EventSystem.eventCollection[EventType.COUNT_BOMBS] += CheckBombs;
        EventSystem.eventCollection[EventType.IN_SETTINGS] += EnteredSettings;
        EventSystem.eventCollection[EventType.OUT_SETTINGS] += ExitSettings;
    }

    protected override void OnDisable()
    {
        // unlisten
        EventSystem.eventCollection[EventType.START_GAME] -= Clickable;
        EventSystem.eventCollection[EventType.END_GAME] -= Unclickable;
        EventSystem.eventCollection[EventType.WIN_GAME] -= Unclickable;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= Unclickable;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= RevealBomb;
        EventSystem.eventCollection[EventType.END_GAME] -= ResetSelf;
        EventSystem.eventCollection[EventType.END_GAME] -= EnableMesh;
        EventSystem.eventCollection[EventType.PREPARE_GAME] -= ResetSelf;
        EventSystem.eventCollection[EventType.PREPARE_GAME] -= StartGame;
        EventSystem.eventCollection[EventType.WIN_GAME] -= EndGame;
        EventSystem.eventCollection[EventType.GAME_LOSE] -= EndGame;
        EventSystem.eventCollection[EventType.COUNT_BOMBS] -= CheckBombs;
        EventSystem.eventCollection[EventType.IN_SETTINGS] -= EnteredSettings;
        EventSystem.eventCollection[EventType.OUT_SETTINGS] -= ExitSettings;
        vfx.gameObject.SetActive(true);
    }

    protected override void AddEmpty()
    {
        transform.parent.GetComponent<AnomalyGridManager2D>().AddTile(gameObject);
    }

    protected override void OnMouseOver()
    {
        UpdateMaterial(selectCol);

        if (gameEnded || (bombCount == 0 && triggered) || inSettings)
        {
            UpdateMaterial(defaultCol);
            return;
        }

        // press left button - highlight adjecant tiles that can be revealed if this tile is revealed
        if (Input.GetMouseButton(0) && triggered && !previewClicked)
        {
            // use box to detect all nearby tiles that can be activated once amount bombs equals amount of flags, not more or less
            Collider[] nearbyFlags = Physics.OverlapSphere(transform.position, 0.75f, flagMask);
            Collider[] allTiles = Physics.OverlapSphere(transform.position, 0.75f, allMask);

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
                tile.GetComponent<Tile2DAnomaly>()?.PreviewTileSelection();
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
                EventSystem.eventCollection[EventType.PLAY_CLICK]();
                EventSystem.eventCollection[EventType.TILE_CLICK]();
                clicked = true;
                DoAction();
                didSomething = true;
            }

            // reveal all nearby tiles
            if (previewClicked && canReveal)
            {
                int actionsInvoked = 0;
                foreach (Collider tile in tilesPreviewed)
                {
                    Collider[] tileFlags = Physics.OverlapSphere(tile.transform.position, 0.25f, flagMask);
                    if (tileFlags.Length == 0)
                    {
                        if (tile.GetComponent<Tile2DAnomaly>().state != TileStates.Revealed)
                        {
                            actionsInvoked++;
                        }
                        tile.GetComponent<Tile2DAnomaly>()?.DoAction();
                    }
                }
                if (actionsInvoked > 0 && clicked)
                {
                    EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK]();
                }

                previewClicked = false;
                tilesPreviewed = null;
                didSomething = true;
            }

            if (!didSomething) EventSystem.eventCollection[EventType.OTHER_CLICK]();
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1) && !triggered)
        {
            EventSystem.eventCollection[EventType.PLAY_FLAG]();
            EventSystem.eventCollectionParam[EventType.PLANT_FLAG](new Vector3[] { transform.position, transform.eulerAngles });
            EventSystem.eventCollection[EventType.OTHER_CLICK]();
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
        if (triggered)
        {
            yield break;
        }

        // return if there is a flag on this position
        Collider[] nearbyFlags = Physics.OverlapSphere(transform.position, 0.25f, flagMask);
        if (nearbyFlags.Length > 0)
        {
            yield break;
        }

        triggered = true;

        defaultCol = emptyTileColor;
        UpdateMaterial(defaultCol, 1);

        if (!gameObject.CompareTag("Bomb"))
        {
            myMesh.enabled = false;
        }

        TypeSpecificAction();

        clicked = false;
    }

    public override void TypeSpecificAction()
    {
        if (state != TileStates.Revealed) EventSystem.eventCollection[EventType.REVEAL_TILE]();

        switch (state)
        {
            case TileStates.Bomb:
                EventSystem.eventCollection[EventType.GAME_LOSE]();
                break;

            case TileStates.Empty:
                transform.parent.GetComponent<AnomalyGridManager2D>().AddSafeTile(gameObject);
                Collider[] tiles = Physics.OverlapSphere(gameObject.transform.position, 1.25f);
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].GetComponent<BaseTile>()?.NoBombReveal();
                }
                state = TileStates.Revealed;
                if (rewardObj != null) rewardObj.SetActive(true);
                if (breakObj != null) breakObj.SetActive(false);
                if (clicked)
                {
                    EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK]();
                }
                break;

            case TileStates.Number:
                transform.parent.GetComponent<AnomalyGridManager2D>().AddSafeTile(gameObject);
                ShowBombAmount();
                state = TileStates.Revealed;
                if (rewardObj != null) rewardObj.SetActive(true);
                if (breakObj != null) breakObj.SetActive(false);
                {
                    if (clicked)
                    {
                        EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK]();
                    }
                }
                break;
        }
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
