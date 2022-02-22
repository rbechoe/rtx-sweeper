using System.Collections;
using UnityEngine;

namespace Tiles2D
{
    public class Tile : Base
    {
        [Header("Settings")]
        public Color defaultCol = Color.grey;
        public Color selectCol = Color.green;
        public VFXManipulator vfx;

        protected LayerMask flagMask;
        protected LayerMask allMask;

        protected int bombCount;
        protected Material gridMat;

        protected bool triggered;
        protected bool clickable;
        protected bool previewClicked;
        protected bool canReveal;
        protected Collider[] tilesPreviewed;

        private float glowIntensity = 8192; // value is in nits
        private Color emptyTileColor; // received from grid manager

        protected override void Start()
        {
            base.Start();

            vfx = GetComponentInChildren<VFXManipulator>();
            gridMat = vfx.gridTile.GetComponent<Renderer>().material;
            gridMat.EnableKeyword("_EmissiveColor");
            UpdateMaterial(defaultCol);
            vfx.gameObject.SetActive(false);

            flagMask = LayerMask.GetMask("Flag");
            allMask = LayerMask.GetMask("Empty", "Flag", "Bomb");

            emptyTileColor = gameObject.GetComponent<Checker>().emptyTileColor;

            StartSettings();
        }

        private void OnEnable()
        {
            // listen
            EventSystem.AddListener(EventType.START_GAME, Clickable);
            EventSystem.AddListener(EventType.END_GAME, Unclickable);
            EventSystem.AddListener(EventType.WIN_GAME, Unclickable);
            EventSystem.AddListener(EventType.GAME_LOSE, Unclickable);
            EventSystem.AddListener(EventType.GAME_LOSE, RevealBomb);
            EventSystem.AddListener(EventType.END_GAME, RemoveSelf);
        }

        private void OnDisable()
        {
            // unlisten
            EventSystem.RemoveListener(EventType.START_GAME, Clickable);
            EventSystem.RemoveListener(EventType.END_GAME, Unclickable);
            EventSystem.RemoveListener(EventType.WIN_GAME, Unclickable);
            EventSystem.RemoveListener(EventType.GAME_LOSE, Unclickable);
            EventSystem.RemoveListener(EventType.GAME_LOSE, RevealBomb);
            EventSystem.RemoveListener(EventType.END_GAME, RemoveSelf);
            vfx.gameObject.SetActive(true);
        }

        private void OnMouseOver()
        {
            if (clickable && !triggered)
            {
                UpdateMaterial(selectCol);
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
                UpdateMaterial(defaultCol);
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

        protected IEnumerator FireAction()
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

        protected void SetToDefaultCol()
        {
            if (clickable && !triggered)
            {
                UpdateMaterial(defaultCol);
            }
        }

        protected void RevealBomb()
        {
            if (gameObject.CompareTag("Bomb"))
            {
                defaultCol = new Color(0.5f, 0f, 0f);
                UpdateMaterial(defaultCol);
                Instantiate(vfx.bombEffect, transform.position, Quaternion.identity);
            }
        }

        protected void ShowBombAmount()
        {
            if (bombCount < 1) return;

            if (!vfx.gameObject.activeSelf) vfx.gameObject.SetActive(true);
            vfx.UpdateEffect(bombCount);
        }

        protected void Clickable()
        {
            clickable = true;
        }

        protected void Unclickable()
        {
            clickable = false;
        }

        protected void RemoveSelf()
        {
            Destroy(this);
        }

        protected void UpdateMaterial(Color color, float intensity = -10)
        {
            if (intensity == -10) intensity = glowIntensity;

            gridMat?.SetColor("_EmissiveColor", color * intensity);
            gridMat?.SetColor("_BaseColor", color);
        }

        public void DoAction()
        {
            StartCoroutine(FireAction());
        }

        public void PreviewTileSelection()
        {
            if (clickable && !triggered)
            {
                UpdateMaterial(Color.white);
            }
        }

        public void FirstTile()
        {
            defaultCol = gameObject.GetComponent<Checker>().startColor;
            UpdateMaterial(defaultCol);
        }

        public void NoBombReveal()
        {
            DoAction();
        }

        public void SetBombCount(int amount)
        {
            bombCount = amount;
        }

        protected virtual void TypeSpecificAction() { }

        protected virtual void StartSettings() { }
    }
}
