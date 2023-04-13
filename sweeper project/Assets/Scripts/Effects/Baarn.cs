using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baarn : MonoBehaviour
{
    float timeAlive = 1;

    private void Update()
    {
        if (timeAlive > 0)
        {
            timeAlive -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        Collider[] tiles = Physics.OverlapSphere(transform.position, 0.25f);
        foreach (Collider col in tiles)
        {
            if (col.GetComponent<HexTileAnomaly>())
            {
                col.transform.GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.black;
            }
        }
    }
}
