using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalk : MonoBehaviour
{
    public Vector3 offset;
    public GameObject target;

    void Update()
    {
        transform.position = target.transform.position + offset;
    }
}
