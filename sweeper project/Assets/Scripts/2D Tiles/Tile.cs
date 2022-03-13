using System.Collections;
using UnityEngine;

namespace Tiles2D
{
    public class Tile : MonoBehaviour
    {
        private VFXManipulator vfx;

        private LayerMask flagMask;
        private LayerMask allMask;

        private int bombCount;
        private Material gridMat;

        private bool triggered;
        private bool clickable;
        private bool previewClicked;
        private bool canReveal;
        private bool gameEnded;
        private Collider[] tilesPreviewed;

        private float glowIntensity = 8192; // value is in nits
        private GridManager manager;
        private Color emptyTileColor; // received from grid manager
        private Color startColor; // received from grid manager
        private Color selectCol; // received from grid manager
        private Color defaultCol;

        public TileStates state = TileStates.Empty;
        public GameObject rewardObj; // show object after clearing tile
        public GameObject breakObj; // remove object when clicking on tile

        private void Start()
        {
            vfx = GetComponentInChildren<VFXManipulator>();
            gridMat = vfx.gridTile.GetComponent<Renderer>().material;
            gridMat.EnableKeyword("_EmissiveColor");
            vfx.gameObject.SetActive(false);

            flagMask = LayerMask.GetMask("Flag");
            allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");

            manager = transform.parent.GetComponent<GridManager>();
            emptyTileColor = manager.emptyTileColor;
            startColor = manager.startColor;
            selectCol = manager.selectColor;
            defaultCol = manager.defaultColor;

            UpdateMaterial(defaultCol);
            UpdateSettings();
        }

        private void OnEnable()
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
        }

        private void OnDisable()
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
            vfx.gameObject.SetActive(true);
        }

        private void EndGame()
        {
            gameEnded = true;
        }

        private void StartGame()
        {
            if (rewardObj != null) rewardObj.SetActive(false);
            if (breakObj != null) breakObj.SetActive(true);
            gameEnded = false;
        }

        private void OnMouseOver()
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
                    tile.GetComponent<Tile>()?.PreviewTileSelection();
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
                    foreach (Collider tile in tilesPreviewed)
                    {
                        tile.GetComponent<Tile>()?.DoAction(true);
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
            UpdateMaterial(defaultCol);

            // set all nearby tiles back to base color
            if (previewClicked)
            {
                foreach (Collider tile in tilesPreviewed)
                {
                    tile.GetComponent<Tile>()?.SetToDefaultCol();
                }
                previewClicked = false;
                tilesPreviewed = null;
            }
        }

        private void CheckBombs()
        {
            // count all nearby bombs
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, Vector3.one * 1.25f, Quaternion.identity);
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
                    EventSystem<GameObject>.InvokeEvent(EventType.ADD_EMPTY, gameObject);
                    state = TileStates.Empty;
                }
            }

            UpdateBombAmount(bombCount);
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

        private void SetToDefaultCol()
        {
            if (clickable && !triggered)
            {
                UpdateMaterial(defaultCol);
            }
        }

        private void RevealBomb()
        {
            if (gameObject.CompareTag("Bomb"))
            {
                defaultCol = new Color(0.5f, 0f, 0f);
                UpdateMaterial(defaultCol);
                Instantiate(vfx.bombEffect, transform.position, Quaternion.identity);
            }
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

        private void UpdateMaterial(Color color, float intensity = -10)
        {
            if (intensity == -10) intensity = glowIntensity;
            
            if (!triggered) gridMat?.SetColor("_EmissiveColor", color * intensity);
            if (triggered) gridMat?.SetColor("_EmissiveColor", color * 0);

            gridMat?.SetColor("_BaseColor", color);
        }

        public void DoAction(bool sequenced = false)
        {
            StartCoroutine(FireAction(sequenced));
        }

        public void PreviewTileSelection()
        {
            // do nothing when revealed
            if (state == TileStates.Revealed) return;

            // return if there is a flag on this position
            Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
            if (nearbyFlags.Length > 0) return;

            if (clickable && !triggered)
            {
                UpdateMaterial(Color.white);
            }
        }

        public void FirstTile()
        {
            defaultCol = startColor;
            UpdateMaterial(defaultCol);
        }

        public void NoBombReveal()
        {
            DoAction(true);
        }

        public void SetBombCount(int amount)
        {
            bombCount = amount;
            UpdateBombAmount(bombCount);
        }

        public void UpdateBombAmount(int amount)
        {
            bombCount = amount;
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

        public void ResetSelf()
        {
            clickable = true;
            triggered = false;
            defaultCol = manager.defaultColor;
            UpdateMaterial(defaultCol);
            vfx.gameObject.SetActive(false);
        }

        public void TypeSpecificAction()
        {
            switch (state)
            {
                case TileStates.Bomb:
                    EventSystem.InvokeEvent(EventType.GAME_LOSE);
                    UpdateMaterial(Color.red);
                    break;

                case TileStates.Empty:
                    EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
                    Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.25f, Quaternion.identity);
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        tiles[i].GetComponent<Tile>()?.NoBombReveal();
                    }
                    state = TileStates.Revealed;
                    if (rewardObj != null) rewardObj.SetActive(true);
                    if (breakObj != null) breakObj.SetActive(false);
                    break;

                case TileStates.Number:
                    EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
                    ShowBombAmount();
                    state = TileStates.Revealed;
                    if (rewardObj != null) rewardObj.SetActive(true);
                    if (breakObj != null) breakObj.SetActive(false);
                    break;
            }
        }

        public void UpdateSettings()
        {
            switch (state)
            {
                case TileStates.Bomb:
                    gameObject.tag = "Bomb";
                    gameObject.layer = 11;
                    break;

                default:
                    gameObject.tag = "Untagged";
                    gameObject.layer = 0;
                    break;
            }
        }
    }
}
