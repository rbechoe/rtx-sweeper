using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoiManager : MonoBehaviour
{
    public List<Transform> positions;

    void Awake()
    {
        foreach(Transform child in transform)
        {
            positions.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetPosition()
    {
        return (positions[Random.Range(0, positions.Count - 1)]);
    }
}
