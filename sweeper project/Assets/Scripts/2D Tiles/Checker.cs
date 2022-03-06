using UnityEngine;
using System.Collections;

namespace Tiles2D
{
    public class Checker : MonoBehaviour
    {
        public Collider[] hitColliders;

        private GridManager gridManager;
        public Color emptyTileColor;
        public Color startColor;

        private void Start()
        {
            gridManager = transform.parent.GetComponent<GridManager>();
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
                    gameObject.AddComponent<EmptyTile>();
                }
                else
                {
                    gameObject.AddComponent<NumberTile>();
                    gameObject.GetComponent<NumberTile>().SetBombCount(bombCount);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
