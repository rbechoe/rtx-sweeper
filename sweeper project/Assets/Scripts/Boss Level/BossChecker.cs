using UnityEngine;
using System.Collections;

namespace BossTiles
{
    public class BossChecker : MonoBehaviour
    {
        public Collider[] hitColliders;

        private BossGridManager gridManager;
        public Color emptyTileColor;
        public Color startColor;

        private void Start()
        {
            gridManager = transform.parent.GetComponent<BossGridManager>();
            emptyTileColor = gridManager.emptyTileColor;
            startColor = gridManager.startColor;
        }

        private void OnEnable()
        {
            EventSystem.AddListener(EventType.COUNT_BOMBS, CheckBombs);
        }

        private void OnDisable()
        {
            EventSystem.RemoveListener(EventType.COUNT_BOMBS, CheckBombs);
        }

        private void CheckBombs()
        {
            StartCoroutine(DoChecks());
        }

        IEnumerator DoChecks()
        {
            hitColliders = Physics.OverlapBox(gameObject.transform.position, Vector3.one * 1.25f, Quaternion.identity);
            int bombCount = 0;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject == gameObject) continue;
                if (hitColliders[i].gameObject.CompareTag("Bomb"))
                {
                    bombCount++;
                }
            }

            if (!gameObject.CompareTag("Bomb"))
            {
                if (bombCount == 0)
                {
                    // set as potential first tile
                    EventSystem<GameObject>.InvokeEvent(EventType.ADD_EMPTY, gameObject);
                    gameObject.GetComponent<BossTile>().state = TileStates.Empty;
                }
                else
                {
                    gameObject.GetComponent<BossTile>().state = TileStates.Number;
                    gameObject.GetComponent<BossTile>().SetBombCount(bombCount);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
