using UnityEngine;
using System.Collections;

public class Checker3D : MonoBehaviour
{
    public Collider[] hitColliders;

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

        BaseTile tile = gameObject.GetComponent<BaseTile>();
        tile.SetBombCount(bombCount);
        tile.ShowBombAmount();

        // set as potential first tile
        if (bombCount == 0 && !gameObject.CompareTag("Bomb"))
        {
            EventSystem<GameObject>.InvokeEvent(EventType.ADD_EMPTY, gameObject);
        }

        yield return new WaitForEndOfFrame();
    }
}