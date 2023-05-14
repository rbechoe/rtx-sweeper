using UnityEngine;

public abstract class BaseTile : MonoBehaviour
{
    protected VFXManipulator vfx;

    protected LayerMask flagMask;
    protected LayerMask allMask;

    protected int bombCount;
    protected Material gridMat;

    public bool triggered;
    public bool clickable;
    protected bool previewClicked;
    protected bool canReveal;
    protected bool gameEnded;
    protected bool inSettings;
    protected Collider[] tilesPreviewed;

    protected float glowIntensity = 8192; // value is in nits
    protected BaseGridManager manager;
    protected Color emptyTileColor; // received from grid manager
    protected Color startColor; // received from grid manager
    protected Color selectCol; // received from grid manager
    protected Color defaultCol;

    public TileStates state = TileStates.Empty;
    public GameObject rewardObj; // show object after clearing tile
    public GameObject breakObj; // remove object when clicking on tile

    public Collider[] nearbyFlags;
    public Collider[] hasFlag;

    protected virtual void Start()
    {
        vfx = GetComponentInChildren<VFXManipulator>();
        if (vfx != null && vfx.gridTile != null)
        {
            gridMat = vfx.gridTile.GetComponent<Renderer>().material;
            gridMat.EnableKeyword("_EmissiveColor");
        }
        vfx?.gameObject.SetActive(false);
        if (vfx == null) Debug.LogWarning("VFX object not found on " + gameObject.name);

        flagMask = LayerMask.GetMask("Flag");
        allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");

        manager = Helpers.FindInParent<BaseGridManager>(transform).GetComponent<BaseGridManager>();

        emptyTileColor = manager.emptyTileColor;
        startColor = manager.startColor;
        selectCol = manager.selectColor;
        defaultCol = manager.defaultColor;

        UpdateMaterial(defaultCol);
        UpdateSettings();
        TypeSettings();
    }

    protected abstract void OnEnable();

    protected abstract void OnDisable();

    protected virtual void EndGame()
    {
        gameEnded = true;
    }

    protected virtual void StartGame()
    {
        if (rewardObj != null) rewardObj.SetActive(false);
        if (breakObj != null) breakObj.SetActive(true);
        gameEnded = false;
    }

    protected abstract void OnMouseOver();

    protected virtual void OnMouseExit()
    {
        UpdateMaterial(defaultCol);

        // set all nearby tiles back to base color
        if (previewClicked)
        {
            foreach (Collider tile in tilesPreviewed)
            {
                tile.GetComponent<BaseTile>()?.SetToDefaultCol();
            }
            previewClicked = false;
            tilesPreviewed = null;
        }
    }

    protected virtual void CheckBombs()
    {
        // count all nearby bombs
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 1.25f);
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

    protected virtual void AddEmpty()
    {
        EventSystem.eventCollectionParam[EventType.ADD_EMPTY](gameObject);
    }

    protected virtual void SetToDefaultCol()
    {
        if (clickable && !triggered)
        {
            UpdateMaterial(defaultCol);
        }
    }

    protected virtual void RevealBomb()
    {
        if (gameObject.CompareTag("Bomb"))
        {
            defaultCol = new Color(0.5f, 0f, 0f);
            UpdateMaterial(defaultCol);
            //Instantiate(vfx.bombEffect, transform.position, Quaternion.identity);
        }
    }

    public virtual void ShowBombAmount()
    {
        if (bombCount < 1 || vfx == null) return;

        if (!vfx.gameObject.activeSelf) vfx.gameObject.SetActive(true);
        vfx.UpdateEffect(bombCount);
    }

    protected virtual void Clickable()
    {
        clickable = true;
    }

    protected virtual void Unclickable()
    {
        clickable = false;
    }

    protected virtual void UpdateMaterial(Color color, float intensity = -10)
    {
        if (intensity == -10) intensity = glowIntensity;

        if (!triggered) gridMat?.SetColor("_EmissiveColor", color * intensity);
        if (triggered) gridMat?.SetColor("_EmissiveColor", color * 0);

        gridMat?.SetColor("_BaseColor", color);
    }

    public abstract void DoAction();

    public virtual void PreviewTileSelection()
    {
        // do nothing when revealed
        if (state == TileStates.Revealed) return;

        // return if there is a flag on this position
        Collider[] nearbyFlags = Physics.OverlapSphere(transform.position, 0.25f, flagMask);
        if (nearbyFlags.Length > 0) return;

        if (clickable && !triggered)
        {
            UpdateMaterial(Color.white);
        }
    }

    public virtual void FirstTile()
    {
        defaultCol = startColor;
        UpdateMaterial(defaultCol);
    }

    public virtual void NoBombReveal()
    {
        DoAction();
    }

    public virtual void SetBombCount(int amount)
    {
        bombCount = amount;
        UpdateBombAmount(bombCount);
    }

    public virtual void UpdateBombAmount(int amount)
    {
        bombCount = amount;

        if (bombCount == 8) SteamAPIManager.Instance.SetAchievement(UserAchievements.eight);

        if (vfx == null) return;

        vfx.UpdateEffect(bombCount);
        if (bombCount > 0 && state == TileStates.Revealed)
        {
            vfx.gameObject.SetActive(true);
        }
        if (bombCount == 0)
        {
            vfx.gameObject.SetActive(false);
        }
    }

    public virtual void ResetSelf()
    {
        clickable = true;
        triggered = false;
        defaultCol = manager.defaultColor;
        UpdateMaterial(defaultCol);
        vfx.gameObject.SetActive(false);
    }

    protected virtual void EnteredSettings()
    {
        inSettings = true;
    }

    protected virtual void ExitSettings()
    {
        inSettings = false;
    }

    public virtual void TypeSpecificAction()
    {
        if (state != TileStates.Revealed) EventSystem.eventCollection[EventType.REVEAL_TILE]();

        switch (state)
        {
            case TileStates.Bomb:
                EventSystem.eventCollection[EventType.GAME_LOSE]();
                break;

            case TileStates.Empty:
                EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE](gameObject);
                Collider[] tiles = Physics.OverlapSphere(gameObject.transform.position, 1.25f);
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i].GetComponent<BaseTile>()?.NoBombReveal();
                }
                state = TileStates.Revealed;
                if (rewardObj != null) rewardObj.SetActive(true);
                if (breakObj != null) breakObj.SetActive(false);
                break;

            case TileStates.Number:
                EventSystem.eventCollectionParam[EventType.ADD_GOOD_TILE](gameObject);
                ShowBombAmount();
                state = TileStates.Revealed;
                if (rewardObj != null) rewardObj.SetActive(true);
                if (breakObj != null) breakObj.SetActive(false);
                break;
        }
    }

    public virtual void UpdateSettings()
    {
        switch (state)
        {
            case TileStates.Bomb:
                gameObject.tag = "Bomb";
                gameObject.layer = 11;
                break;

            case TileStates.Empty:
                gameObject.tag = "Empty";
                gameObject.layer = 12;
                break;

            default:
                gameObject.tag = "Untagged";
                gameObject.layer = 0;
                break;
        }
    }

    public abstract void TypeSettings();
}
