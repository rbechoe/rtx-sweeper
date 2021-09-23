using UnityEngine;
using System.Collections;

public class Checker3D : Base
{
    public Collider[] hitColliders;

    private void OnEnable()
    {
        EventSystem<Parameters>.AddListener(EventType.COUNT_BOMBS, CheckBombs);
    }

    private void OnDisable()
    {
        EventSystem<Parameters>.RemoveListener(EventType.COUNT_BOMBS, CheckBombs);
    }

    private void CheckBombs(object value)
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

        Tile3D tile = gameObject.GetComponent<Tile3D>();
        tile.SetBombCount(bombCount);
        tile.ShowBombAmount();

        // set as potential first tile
        if (bombCount == 0 && !gameObject.CompareTag("Bomb"))
        {
            Parameters param = new Parameters();
            param.gameObjects.Add(gameObject);
            EventSystem<Parameters>.InvokeEvent(EventType.ADD_EMPTY, param);
        }

        yield return new WaitForEndOfFrame();
    }
}