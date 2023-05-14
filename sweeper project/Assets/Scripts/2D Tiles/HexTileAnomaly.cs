using System.Collections;
using UnityEngine;

public class HexTileAnomaly : BaseTile
{
    private MeshRenderer myMesh;
    private bool clicked;
    private bool clickedAction = false;

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
        transform.parent.GetComponent<GridManagerHex>().AddTile(gameObject);
    }

    private void Update()
    {
        nearbyFlags = Physics.OverlapSphere(transform.position, 1.25f, flagMask);
        hasFlag = Physics.OverlapSphere(transform.position, 0.4f, flagMask);
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
                tile.GetComponent<HexTileAnomaly>()?.PreviewTileSelection();
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
                clickedAction = false;
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
                    if (!tile.GetComponent<HexTileAnomaly>())
                    {
                        continue;
                    }
                    if (tile.GetComponent<HexTileAnomaly>().state != TileStates.Revealed && tile.GetComponent<HexTileAnomaly>().hasFlag.Length == 0)
                    {
                        actionsInvoked++;
                    }
                    tile.GetComponent<HexTileAnomaly>()?.DoAction();
                }

                if (actionsInvoked > 0 && clicked && !clickedAction)
                {
                    EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK]();
                    clickedAction = true;
                }

                previewClicked = false;
                tilesPreviewed = null;
                didSomething = true;
            }

            if (!didSomething) EventSystem.eventCollection[EventType.OTHER_CLICK]();
        }

        // right click - place flag
        if (Input.GetMouseButtonUp(1) && !triggered && hasFlag.Length == 0)
        {
            EventSystem.eventCollection[EventType.PLAY_FLAG]();
            EventSystem.eventCollectionParam[EventType.PLANT_FLAG](transform);
            EventSystem.eventCollection[EventType.OTHER_CLICK]();
        }
        // right click - remove flag
        else if (Input.GetMouseButtonUp(1) && !triggered && hasFlag.Length > 0)
        {
            EventSystem.eventCollectionParam[EventType.REMOVE_FLAG](hasFlag[0].gameObject);
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

    public override void ShowBombAmount()
    {
        if (bombCount < 1 || vfx == null) return;

        if (!vfx.gameObject.activeSelf) vfx.gameObject.SetActive(true);
        vfx.UpdateEffect(bombCount);
    }

    public void SpreadInfection()
    {
        if (state != TileStates.Bomb || hasFlag.Length > 0)
        {
            return;
        }

        Collider[] neighbourHexes = Physics.OverlapSphere(transform.position, 1.25f, allMask);
        foreach (Collider hex in neighbourHexes)
        {
            HexTileAnomaly hexComponent = hex.GetComponent<HexTileAnomaly>();
            if (hexComponent != null && hexComponent.state == TileStates.Revealed)
            {
                hexComponent.ResetTile();
                break;
            }
        }
    }

    protected override void CheckBombs()
    {
        // count all nearby bombs
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 1.2f);
        int bombCount = 0;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject == gameObject) continue;
            if (hitColliders[i].gameObject.CompareTag("Bomb"))
            {
                bombCount++;
            }
        }

        if (state != TileStates.Bomb && state != TileStates.Revealed)
        {
            if (bombCount > 0)
            {
                state = TileStates.Number;
            }
            else
            {
                AddEmpty();
                state = TileStates.Empty;
            }
        }

        UpdateBombAmount(bombCount);
    }

    public void ResetTile()
    {
        state = TileStates.Empty;
        EventSystem.eventCollectionParam[EventType.REMOVE_GOOD_TILE](gameObject);
        ResetSelf();
        EnableMesh();
        StartGame();
        CheckBombs();
    }

    private IEnumerator FireAction()
    {
        if (triggered)
        {
            yield break;
        }

        // return if there is a flag on this position
        if (hasFlag.Length > 0)
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
        if (state != TileStates.Revealed)
        {
            EventSystem.eventCollection[EventType.REVEAL_TILE]();
        }

        switch (state)
        {
            case TileStates.Bomb:
                EventSystem.eventCollection[EventType.GAME_LOSE]();
                return;

            case TileStates.Empty:
                Collider[] tiles = Physics.OverlapSphere(gameObject.transform.position, 1.25f);
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].GetComponent<HexTileAnomaly>()?.NoBombReveal();
                }
                break;

            case TileStates.Number:
                ShowBombAmount();
                break;
        }

        EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE](gameObject);
        state = TileStates.Revealed;
        if (rewardObj != null) rewardObj.SetActive(true);
        if (breakObj != null) breakObj.SetActive(false);
        if (clicked && !clickedAction)
        {
            EventSystem.eventCollection[EventType.MOUSE_LEFT_CLICK]();
            clickedAction = true;
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
