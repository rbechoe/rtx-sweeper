using System.Collections;
using UnityEngine;

public class BossTile : BaseTile
{
    public int myId; // used to update checks on manager
    private bool shuffling;
    private BossGridManager bossManager;

    private void FixedUpdate()
    {
        CheckBombs();
    }

    protected override void OnEnable()
    {
        // listen
        EventSystem.AddListener(EventType.START_GAME, Clickable);
        EventSystem.AddListener(EventType.END_GAME, Unclickable);
        EventSystem.AddListener(EventType.WIN_GAME, Unclickable);
        EventSystem.AddListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.AddListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.AddListener(EventType.WIN_GAME, EndGame);
        EventSystem.AddListener(EventType.GAME_LOSE, EndGame);
        EventSystem.AddListener(EventType.GAME_LOSE, RevealBomb);
        EventSystem.AddListener(EventType.END_GAME, ResetSelf);
        EventSystem.AddListener(EventType.PREPARE_GAME, ResetSelf);
        EventSystem.AddListener(EventType.UNPLAYABLE, Unplayable);
        EventSystem.AddListener(EventType.PLAYABLE, Playable);
    }

    protected override void OnDisable()
    {
        // unlisten
        EventSystem.RemoveListener(EventType.START_GAME, Clickable);
        EventSystem.RemoveListener(EventType.END_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.WIN_GAME, Unclickable);
        EventSystem.RemoveListener(EventType.GAME_LOSE, Unclickable);
        EventSystem.RemoveListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.RemoveListener(EventType.WIN_GAME, EndGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, EndGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, RevealBomb);
        EventSystem.RemoveListener(EventType.END_GAME, ResetSelf);
        EventSystem.RemoveListener(EventType.PREPARE_GAME, ResetSelf);
        EventSystem.RemoveListener(EventType.UNPLAYABLE, Unplayable);
        EventSystem.RemoveListener(EventType.PLAYABLE, Playable);
        vfx.gameObject.SetActive(true);
    }

    protected override void OnMouseOver()
    {
        UpdateMaterial(selectCol);

        if (shuffling || gameEnded)
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
                BossTile bossTile = tile.GetComponent<BossTile>();
                bossTile?.PreviewTileSelection();
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
                    tile.GetComponent<BossTile>()?.DoAction();
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

    private void Unplayable()
    {
        shuffling = true;
        bossManager.checks[myId] = true;
    }

    private void Playable()
    {
        bossManager.checks[myId] = true;
        shuffling = false;
    }

    protected override void AddEmpty()
    {
        // dont add empty
    }

    protected override void UpdateMaterial(Color color, float intensity = -10)
    {
        if (intensity == -10) intensity = glowIntensity;

        if (state != TileStates.Revealed) gridMat?.SetColor("_EmissiveColor", color * intensity);
        else gridMat?.SetColor("_EmissiveColor", color * 0);

        gridMat?.SetColor("_BaseColor", color);
    }

    private IEnumerator FireAction()
    {
        bossManager.busyTiles++;
        yield return new WaitForEndOfFrame();

        if (triggered)
        {
            bossManager.busyTiles--;
            yield break;
        }

        // return if there is a flag on this position
        Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
        if (nearbyFlags.Length > 0)
        {
            bossManager.busyTiles--;
            yield break;
        }

        triggered = true;

        defaultCol = emptyTileColor;
        UpdateMaterial(defaultCol, 1);

        TypeSpecificAction();

        yield return new WaitForEndOfFrame();
        bossManager.busyTiles--;

        // shuffle if all tiles are done and this was the last one in the queue
        if (bossManager.busyTiles == 0)
        {
            bossManager.ShuffleGrid();
        }
    }

    public override void DoAction()
    {
        StartCoroutine(FireAction());
    }

    public override void NoBombReveal()
    {
        DoAction();
    }

    public override void TypeSettings()
    {
        bossManager = transform.parent.GetComponent<BossGridManager>();
    }
}
