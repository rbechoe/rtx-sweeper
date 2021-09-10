using UnityEngine;
using System.Collections;

public class Checker : Base
{
    public Collider[] hitColliders;

    public void CheckBombs()
    {
        StartCoroutine(DoChecks());
    }

    IEnumerator DoChecks()
    {
        hitColliders = Physics.OverlapBox(gameObject.transform.position, Vector3.one * 1.25f, Quaternion.identity);
        int i = 0;
        int bombCount = 0;

        while (i < hitColliders.Length - 1)
        {
            i++;
            if (hitColliders[i].gameObject == gameObject) continue;
            if (hitColliders[i].gameObject.CompareTag("Bomb"))
            {
                // doesnt show all bombs somehow
                bombCount++;
            }
        }

        gameObject.GetComponent<Tile>().SetBombCount(bombCount);
        gameObject.GetComponent<Tile>().ShowBombAmount();

        yield return new WaitForEndOfFrame();
    }
}