using System.Collections;
using UnityEngine;

namespace BossTiles
{
    public class BossTile : MonoBehaviour
    {
        [Header("Settings")]
        public VFXManipulator vfx;

        private LayerMask flagMask;
        private LayerMask allMask;

        private int bombCount;
        private Material gridMat;

        public int myId; // used to update checks on manager
        public bool triggered;
        private bool clickable;
        private bool previewClicked;
        private bool canReveal;
        private bool shuffling;
        private Collider[] tilesPreviewed;

        private float glowIntensity = 8192; // value is in nits
        private BossGridManager manager;
        private Color emptyTileColor; // received from grid manager
        private Color startColor; // received from grid manager
        private Color selectCol; // received from grid manager
        private Color defaultCol;

        public BossTileStates state = BossTileStates.Empty;

        private void Start()
        {
            vfx = GetComponentInChildren<VFXManipulator>();
            gridMat = vfx.gridTile.GetComponent<Renderer>().material;
            gridMat.EnableKeyword("_EmissiveColor");
            vfx.gameObject.SetActive(false);

            flagMask = LayerMask.GetMask("Flag");
            allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");

            manager = transform.parent.GetComponent<BossGridManager>();
            emptyTileColor = manager.emptyTileColor;
            startColor = manager.defaultColor;
            selectCol = manager.selectColor;
            defaultCol = startColor;

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
            EventSystem.AddListener(EventType.UNPLAYABLE, Unplayable);
            EventSystem.AddListener(EventType.PLAYABLE, Playable);
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
            EventSystem.RemoveListener(EventType.UNPLAYABLE, Unplayable);
            EventSystem.RemoveListener(EventType.PLAYABLE, Playable);
            vfx.gameObject.SetActive(true);
        }

        private void OnMouseOver()
        {
            UpdateMaterial(selectCol);

            if (shuffling) return;

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
                        BossTile bossTile = _tile.GetComponent<BossTile>();
                        if (bossTile != null)
                        {
                            bossTile?.DoAction();
                        }
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
                foreach (Collider _tile in tilesPreviewed)
                {
                    _tile.GetComponent<BossTile>()?.SetToDefaultCol();
                }
                previewClicked = false;
                tilesPreviewed = null;
            }
        }

        private void Unplayable()
        {
            shuffling = true;
            manager.checks[myId] = true;
        }

        private void Playable()
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

            if (state != BossTileStates.Bomb && state != BossTileStates.Revealed)
            {
                if (bombCount > 0)
                {
                    state = BossTileStates.Number;
                }
                else
                {
                    state = BossTileStates.Empty;
                }
            }

            UpdateBombAmount(bombCount);
            
            manager.checks[myId] = true;
            shuffling = false;
        }

        private IEnumerator FireAction()
        {
            manager.busyTiles++;
            yield return new WaitForEndOfFrame();

            if (triggered)
            {
                manager.busyTiles--;
                yield break;
            }

            // return if there is a flag on this position
            Collider[] nearbyFlags = Physics.OverlapBox(transform.position, Vector3.one * 0.25f, Quaternion.identity, flagMask);
            if (nearbyFlags.Length > 0)
            {
                manager.busyTiles--;
                yield break;
            }

            triggered = true;

            defaultCol = emptyTileColor;
            UpdateMaterial(defaultCol, 1);

            TypeSpecificAction();

            yield return new WaitForEndOfFrame();
            manager.busyTiles--;

            // shuffle if all tiles are done and this was the last one in the queue
            if (manager.busyTiles == 0)
            {
                manager.ShuffleGrid();
            }
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

            if (state != BossTileStates.Revealed) gridMat?.SetColor("_EmissiveColor", color * intensity);
            gridMat?.SetColor("_BaseColor", color);
        }

        public void DoAction()
        {
            StartCoroutine(FireAction());
        }

        public void PreviewTileSelection()
        {
            // do nothing when revealed
            if (state == BossTileStates.Revealed) return;

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
            defaultCol = gameObject.GetComponent<BossChecker>().startColor;
            UpdateMaterial(defaultCol);
        }

        public void NoBombReveal()
        {
            DoAction();
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
            if (bombCount > 0 && state == BossTileStates.Revealed)
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
            defaultCol = startColor;
            UpdateMaterial(defaultCol);
            vfx.gameObject.SetActive(false);
        }

        public void TypeSpecificAction()
        {
            switch (state)
            {
                case BossTileStates.Bomb:
                    EventSystem.InvokeEvent(EventType.GAME_LOSE);
                    break;

                case BossTileStates.Empty:
                    EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
                    Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.25f, Quaternion.identity);
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        tiles[i].GetComponent<BossTile>()?.NoBombReveal();
                    }
                    state = BossTileStates.Revealed;
                    break;

                case BossTileStates.Number:
                    EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
                    ShowBombAmount();
                    state = BossTileStates.Revealed;
                    break;
            }
        }

        public void UpdateSettings()
        {
            switch (state)
            {
                case BossTileStates.Bomb:
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
