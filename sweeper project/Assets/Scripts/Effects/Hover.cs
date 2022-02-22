using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float distance = 1f;
    public float speed = 1f;
    private float timeOffset = 0; // makes it a bit random

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0, 1000);
    }

    private void Update()
    {
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * speed + timeOffset) * distance, 0);
    }
}
